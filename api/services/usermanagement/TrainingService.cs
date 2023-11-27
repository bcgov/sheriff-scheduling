﻿using System;
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
            var sheriffQuery = Db.Sheriff.AsNoTracking()
                .AsSplitQuery()
                .Where(s => 
                    s.IsEnabled &&
                    (trainingReportSearch.locationId == null || s.HomeLocationId==trainingReportSearch.locationId)
                )
                .Include(s => s.Training.Where(t => t.ExpiryDate == null))
                .ThenInclude(t => t.TrainingType)
                .Include(s => s.HomeLocation)
                .Where(s => trainingReportSearch.regionId == null || s.HomeLocation.RegionId==trainingReportSearch.regionId);

            var sheriffs = await sheriffQuery.ToListAsync();

            List<LookupCode> mandatoryTrainings = await ManageTypesService.GetAllForReports(trainingReportSearch.reportSubtypeId, LookupTypes.TrainingType, null, true, false);
            List<LookupCode> optionalTrainings = await ManageTypesService.GetAllForReports(trainingReportSearch.reportSubtypeId, LookupTypes.TrainingType, null, false, false);

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
                            trainingType = training.Description,
                            end = null,                          
                            expiryDate = null,
                            excluded = sheriff.Excused,
                            sheriffId = sheriff.Id,
                            status = TrainingStatusTypes.danger,
                            _rowVariant = "danger"
                        });
                    }
                    else
                    {
                        var takenTraining = sheriff.Training.Find(t => t.TrainingTypeId == training.Id);
                        var timezone = takenTraining.Timezone == null? "America/Vancouver" : takenTraining.Timezone; 
                        var trainingStatus = GetTrainingStatus(takenTraining.TrainingCertificationExpiry, timezone, training.AdvanceNotice);
                        
                        sheriffTrainings.Add(new TrainingReportDto()
                        {
                            name = sheriff.FirstName + ' ' + sheriff.LastName,
                            trainingType = training.Description,
                            end = takenTraining.EndDate.ConvertToTimezone(timezone),                          
                            expiryDate = takenTraining.TrainingCertificationExpiry != null ? ((DateTimeOffset)takenTraining.TrainingCertificationExpiry).ConvertToTimezone(timezone): null,
                            excluded = sheriff.Excused,
                            sheriffId = sheriff.Id,
                            status = trainingStatus.status,
                            _rowVariant = trainingStatus.rowType
                        });
                    }
                }
            }

            Logger.LogInformation("__________End_Creating_Reports____________");
            
            if(trainingReportSearch.startDate != null && trainingReportSearch.endDate != null)
                return sheriffTrainings.FindAll(t => 
                    t.end>=trainingReportSearch.startDate && 
                    t.end<=trainingReportSearch.endDate
                );             
            else
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
            
            foreach (var training in allTrainings)
            {
                if(training.TrainingType.ValidityPeriod > 0){   
                    var timezone = training.Timezone == null? "America/Vancouver" : training.Timezone; 
                    if(training.TrainingType.ValidityPeriod == 365){
                        training.TrainingCertificationExpiry = training.EndDate.EndOfYearWithTimezone(0,timezone);//moment(this.selectedEndDate).endOf('year').format("YYYY-MM-DD");                        
                    }else if(training.TrainingType.ValidityPeriod == 730){
                        training.TrainingCertificationExpiry = training.EndDate.EndOfYearWithTimezone(1,timezone);//moment(this.selectedEndDate).endOf('year').add(1,'year').format("YYYY-MM-DD");
                    }else if(training.TrainingType.ValidityPeriod == 1095){
                        training.TrainingCertificationExpiry = training.EndDate.EndOfYearWithTimezone(2,timezone);//moment(this.selectedEndDate).endOf('year').add(2,'year').format("YYYY-MM-DD");
                    }else{
                        training.TrainingCertificationExpiry = training.EndDate.AddDays(training.TrainingType.ValidityPeriod).ConvertToTimezone(timezone);//moment(this.selectedEndDate).add(this.selectedTrainingType.validityPeriod, 'days').format("YYYY-MM-DD");
                    }
                }
                else{
                    training.TrainingCertificationExpiry = null;
                }
                Db.Entry(training).Property(x => x.TrainingCertificationExpiry).IsModified = true;
                Db.SaveChanges();
            }
        
        }

        #endregion Training Expiry Adjustment


        #region Help Methods        

        private  TrainingStatus GetTrainingStatus(DateTimeOffset? requalificationDate, string timezone, int advanceNotice)
        {
            TrainingStatus trainingStatus = new TrainingStatus();
            
            var todayDate = DateTimeOffset.UtcNow.ConvertToTimezone(timezone);
            var advanceNoticeDate = DateTimeOffset.UtcNow.AddDays(advanceNotice).ConvertToTimezone(timezone);
            var expiryDate = requalificationDate?.AddYears(1);

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
            else if(advanceNoticeDate > requalificationDate) 
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

        #endregion Help Methods
    }
}