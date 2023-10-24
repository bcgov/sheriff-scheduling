using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CAS.API.helpers.extensions;
using CAS.API.infrastructure.exceptions;
using CAS.DB.models;
using CAS.DB.models.courtAdmin;

namespace CAS.API.services.usermanagement
{

    public class TrainingService
    {
        private CourtAdminDbContext Db { get; }

        public TrainingService(CourtAdminDbContext db)
        {
            Db = db;
        }


        public async Task<List<CourtAdminTraining>> GetTrainings()
        {                                    
            var sheriffTrainingQuery = Db.SheriffTraining.AsNoTracking()
                .AsSplitQuery()
                .Where(t => t.ExpiryDate == null)
                .Where(t => t.FirstNotice != true)
                .Where(t => t.TrainingType.AdvanceNotice > 0)                
                .Include(t => t.TrainingType)
                .Include(t => t.CourtAdmin);      

            return await sheriffTrainingQuery.ToListAsync();
        }

        public async Task UpdateTraining(int trainingId)
        {
            var training = await Db.SheriffTraining.FindAsync(trainingId);
            training.ThrowBusinessExceptionIfNull(
                $"{nameof(CourtAdminTraining)} with the id: {trainingId} could not be found. ");

            if (training.ExpiryDate.HasValue)
                throw new BusinessLayerException($"{nameof(CourtAdminTraining)} with the id: {trainingId} has been expired");
            
            training.FirstNotice = true;
            await Db.SaveChangesAsync();            
        }        
    }
}