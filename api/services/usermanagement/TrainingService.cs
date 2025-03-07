using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SS.Api.helpers.extensions;
using SS.Api.infrastructure.exceptions;
using SS.Api.models.dto;
using SS.Db.models;
using SS.Db.models.lookupcodes;
using SS.Db.models.sheriff;
using SS.Api.models.types;
using SS.Common.helpers.extensions;
using ss.db.models;
using Microsoft.Extensions.Logging;

namespace SS.Api.services.usermanagement
{

    public class TrainingService
    {
        private SheriffDbContext Db { get; }
        private ManageTypesService ManageTypesService { get; }
        private ILogger<TrainingService> Logger { get; }
        public static readonly IList<int> YearsInDays = new List<int>{365, 730, 1095, 1461, 1826, 2191, 2556, 2922, 3287, 3652};         

        public TrainingService(ManageTypesService manageTypesService, SheriffDbContext db, ILogger<TrainingService> logger)
        {
            ManageTypesService = manageTypesService;
            Db = db;
            Logger = logger;
        }


        #region Training CronJob
        public async Task<List<SheriffTraining>> GetTrainings()
        {
            var sheriffTrainingQuery = Db.SheriffTraining.AsNoTracking()
                .AsSplitQuery()
                .Where(t => t.ExpiryDate == null)
                .Where(t => t.FirstNotice != true)
                .Where(t => t.TrainingType.AdvanceNotice > 0)
                .Include(t => t.TrainingType)
                .Include(t => t.Sheriff);

            return await sheriffTrainingQuery.ToListAsync();
        }

        public async Task UpdateTraining(int trainingId)
        {
            var training = await Db.SheriffTraining.FindAsync(trainingId);
            training.ThrowBusinessExceptionIfNull(
                $"{nameof(SheriffTraining)} with the id: {trainingId} could not be found. ");

            if (training.ExpiryDate.HasValue)
                throw new BusinessLayerException($"{nameof(SheriffTraining)} with the id: {trainingId} has been expired");

            training.FirstNotice = true;
            await Db.SaveChangesAsync();
        }

        #endregion Training CronJob



        #region Training Reports        
        public async Task<List<TrainingReportDto>> GetSheriffsTrainingReports(TrainingReportSearchDto trainingReportSearch)
        {
            Logger.LogInformation("__________Start_Creating_Reports___________");
            var sheriffQuery = Db.Sheriff.AsNoTracking();

            if (trainingReportSearch.sheriffId != null)
            {
                sheriffQuery = sheriffQuery.Where(s => s.Id == trainingReportSearch.sheriffId);
            }

            sheriffQuery = sheriffQuery.AsSplitQuery()
                .Where(s => 
                    s.IsEnabled &&
                    (trainingReportSearch.locationId == null || s.HomeLocationId==trainingReportSearch.locationId)
                )
                .Include(s => s.Training.Where(t => t.ExpiryDate == null))
                .ThenInclude(t => t.TrainingType)
                .Include(s => s.HomeLocation)
                .ThenInclude(h => h.Region)
                .Where(s => trainingReportSearch.regionId == null || s.HomeLocation.RegionId==trainingReportSearch.regionId);

            var sheriffs = await sheriffQuery.ToListAsync();

            List<LookupCode> mandatoryTrainings = await ManageTypesService.GetAllForReports(trainingReportSearch.reportSubtypeIds, LookupTypes.TrainingType, null, true, false);
            List<LookupCode> optionalTrainings = await ManageTypesService.GetAllForReports(trainingReportSearch.reportSubtypeIds, LookupTypes.TrainingType, null, false, false);

            var sheriffTrainings = new List<TrainingReportDto>();

            foreach (var sheriff in sheriffs)
            {
                var sheriffTrainingsId = sheriff.Training.Select(t => t.TrainingTypeId).ToArray();
                List<LookupCode> optionalSheriffTrainings = optionalTrainings.FindAll(t => sheriffTrainingsId.Contains(t.Id));
                List<LookupCode> allSheriffTrainings = new List<LookupCode>();
                allSheriffTrainings.AddRange(optionalSheriffTrainings);
                allSheriffTrainings.AddRange(mandatoryTrainings);

                foreach (var training in allSheriffTrainings)
                {
                    if (!sheriffTrainingsId.Contains(training.Id))
                    {
                        sheriffTrainings.Add(new TrainingReportDto()
                        {
                            name = sheriff.FirstName + ' ' + sheriff.LastName,
                            location = sheriff.HomeLocation.Name,
                            trainingType = training.Description,
                            end = null,                          
                            expiryDate = null,
                            excluded = sheriff.Excused,
                            sheriffId = sheriff.Id,
                            status = TrainingStatusTypes.danger,
                            _rowVariant = "danger",
                            region = sheriff.HomeLocation.Region.Name
                        });
                    }
                    else
                    {
                        var takenTrainings = sheriff.Training.FindAll(t => t.TrainingTypeId == training.Id).OrderByDescending(t => t.EndDate).ToList();
                        var takenTraining = new SheriffTraining();
                        var timezone = "";

                        TrainingStatus trainingStatus = new TrainingStatus();
                        if(trainingReportSearch.startDate != null && trainingReportSearch.endDate != null)
                        {
                            var takenTrainingInRange = takenTrainings.Find(t => t.EndDate < trainingReportSearch.endDate);
                            takenTraining = takenTrainingInRange !=null ? takenTrainingInRange : takenTrainings.LastOrDefault();
                            timezone = takenTraining.Timezone == null? "America/Vancouver" : takenTraining.Timezone;
                            trainingStatus = GetTrainingStatusOnDateRange(trainingReportSearch, takenTraining.TrainingCertificationExpiry, timezone, training, takenTraining.EndDate);
                        }                            
                        else
                        {
                            takenTraining = takenTrainings[0];
                            timezone = takenTraining.Timezone == null? "America/Vancouver" : takenTraining.Timezone;
                            trainingStatus = GetTrainingStatus(takenTraining.TrainingCertificationExpiry, timezone, training.AdvanceNotice, training, takenTraining.FirstNotice);
                        }                            
                        
                        sheriffTrainings.Add(new TrainingReportDto()
                        {
                            name = sheriff.FirstName + ' ' + sheriff.LastName,
                            location = sheriff.HomeLocation.Name,
                            trainingType = training.Description,
                            end = takenTraining.EndDate.ConvertToTimezone(timezone),                          
                            expiryDate = takenTraining.TrainingCertificationExpiry != null ? ((DateTimeOffset)takenTraining.TrainingCertificationExpiry).ConvertToTimezone(timezone): null,
                            excluded = sheriff.Excused,
                            sheriffId = sheriff.Id,
                            status = trainingStatus.status,
                            _rowVariant = trainingStatus.rowType,
                            region = sheriff.HomeLocation.Region.Name
                        });
                    }
                }
            }

            Logger.LogInformation("__________End_Creating_Reports____________");
            
            // if(trainingReportSearch.startDate != null && trainingReportSearch.endDate != null)
            //     return sheriffTrainings.FindAll(t => 
            //         t.end>=trainingReportSearch.startDate && 
            //         t.end<=trainingReportSearch.endDate
            //     );             
            // else
            return sheriffTrainings;
        }

        #endregion Training Reports


        #region Training Expiry Adjustment
        public async Task TrainingExpiryAdjustment()
        {
            var trainingsQuery = Db.SheriffTraining
                .AsNoTracking()
                .Where(t => t.ExpiryDate == null)
                .Include(t => t.TrainingType);
            var allTrainings = await trainingsQuery.ToListAsync();
            int processCounter = 0;
            int allTrainingsCounts = allTrainings.Count();

            foreach (var training in allTrainings)
            {
                if(training.TrainingType.ValidityPeriod > 0){   
                    var timezone = training.Timezone == null? "America/Vancouver" : training.Timezone;                     
                    if(!IsRotatingTraining(training.TrainingType)){
                        int years = (training.TrainingType.ValidityPeriod / 365) - 1;
                        training.TrainingCertificationExpiry = training.EndDate.EndOfYearWithTimezone(years, timezone);
                    }else{
                        training.TrainingCertificationExpiry = training.EndDate.AddDays(training.TrainingType.ValidityPeriod).ConvertToTimezone(timezone);//moment(this.selectedEndDate).add(this.selectedTrainingType.validityPeriod, 'days').format("YYYY-MM-DD");
                    }
                    var noticeDate = DateTimeOffset.UtcNow.AddDays(training.TrainingType.AdvanceNotice);
                    if(training.TrainingCertificationExpiry > noticeDate){
                        training.FirstNotice = false;
                    }
                }
                else{
                    training.TrainingCertificationExpiry = null;
                    training.FirstNotice = false;
                }
                Db.Entry(training).Property(x => x.TrainingCertificationExpiry).IsModified = true;
                Db.Entry(training).Property(x => x.FirstNotice).IsModified = true;
                Db.SaveChanges();

                processCounter++;                
                Logger.LogInformation($"_______Training Expiry Adjustment__{processCounter}_of_{allTrainingsCounts}_______");
            }
        
        }

        #endregion Training Expiry Adjustment


        #region Help Methods        

        private  TrainingStatus GetTrainingStatus(DateTimeOffset? requalificationDate, string timezone, int advanceNotice, LookupCode trainingType, bool firstNotice)
        {
            TrainingStatus trainingStatus = new TrainingStatus();
            
            var todayDate = DateTimeOffset.UtcNow.ConvertToTimezone(timezone);
            var advanceNoticeDate = DateTimeOffset.UtcNow.AddDays(advanceNotice).ConvertToTimezone(timezone);
            var expiryDate = IsRotatingTraining(trainingType)? requalificationDate : requalificationDate?.AddYears(1);

            if(todayDate > expiryDate)
            {
                trainingStatus.rowType = "alert";
                trainingStatus.status = TrainingStatusTypes.alert;
            }
            else if(todayDate > requalificationDate)
            {
                trainingStatus.rowType = "warning";
                trainingStatus.status = TrainingStatusTypes.warning;
            }
            else if((advanceNoticeDate > requalificationDate) && firstNotice) 
            {
                trainingStatus.rowType = "notify";
                trainingStatus.status = TrainingStatusTypes.notify;
            }
            else
            {
                trainingStatus.rowType = "white";
                trainingStatus.status = "";
            }
            
            return trainingStatus;
        }

        private  TrainingStatus GetTrainingStatusOnDateRange(TrainingReportSearchDto trainingReportSearch, DateTimeOffset? requalificationDate, string timezone, LookupCode trainingType, DateTimeOffset trainingCompletionDate)
        {
            TrainingStatus trainingStatus = new TrainingStatus();
            
            var reportEndDate = ((DateTimeOffset)trainingReportSearch.endDate).ConvertToTimezone(timezone);
            var expiryDate = IsRotatingTraining(trainingType)? requalificationDate : requalificationDate?.AddYears(1);

            if(reportEndDate > expiryDate)
            {
                trainingStatus.rowType = "alert";
                trainingStatus.status = TrainingStatusTypes.alert;
            }
            else if(reportEndDate > requalificationDate)
            {
                trainingStatus.rowType = "warning";
                trainingStatus.status = TrainingStatusTypes.warning;
            }
            else if(reportEndDate >= trainingCompletionDate && (reportEndDate <= requalificationDate || requalificationDate == null) )
            {
                trainingStatus.rowType = "white";
                trainingStatus.status = "";
            }
            else
            {                            
                trainingStatus.rowType = "danger";
                trainingStatus.status = TrainingStatusTypes.danger;
            }
            
            return trainingStatus;
        }

        public bool IsRotatingTraining(LookupCode trainingType){
            return trainingType.Rotating || !YearsInDays.Contains(trainingType.ValidityPeriod);               
        }

        #endregion Help Methods
    }
}