using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CAS.API.Models.DB;
using CAS.DB.models;
using CAS.DB.models.scheduling;
using tests.api.Helpers;
using Xunit;

namespace tests.db
{
    //Test the ConcurrencyException that comes back. 
    public class ConcurrencyExceptionTest
    {
        protected CourtAdminDbContext Db { get; }
        public ConcurrencyExceptionTest()
        {
            Db = new CourtAdminDbContext(EnvironmentBuilder.SetupDbOptions(false));
        }

        [Fact(Skip="Adhoc test that writes to the database.")]
        public async Task CauseDbConcurrentUpdateException()
        {
            Db.Shift.Remove(Db.Shift.First(l => l.Id == 9000000));
            Db.Location.Remove(Db.Location.First(l => l.Id == 9000000));
            await Db.SaveChangesAsync();

            await Db.Location.AddAsync(new Location {Id = 9000000, AgencyId = "zz"});
            await Db.Shift.AddAsync(new Shift { Id = 9000000, LocationId = 9000000 });
            await Db.SaveChangesAsync();

            foreach (var entity in Db.ChangeTracker.Entries())
                entity.State = EntityState.Detached;

            var shift = await Db.Shift.AsNoTracking().FirstOrDefaultAsync(s => s.Id == 9000000);
            var shift2 = await Db.Shift.AsNoTracking().FirstOrDefaultAsync(s => s.Id == 9000000);

            Db.Shift.Update(shift);
            await Db.SaveChangesAsync();

            foreach (var entity in Db.ChangeTracker.Entries())
                entity.State = EntityState.Detached;

            Debug.WriteLine($"{shift.ConcurrencyToken}");
            Debug.WriteLine($"{shift2.ConcurrencyToken}");

            shift2.StartDate = DateTimeOffset.UtcNow;
            shift2.EndDate = DateTimeOffset.UtcNow;

            Db.Shift.Update(shift2);
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () => await Db.SaveChangesAsync());

            foreach (var entity in Db.ChangeTracker.Entries())
                entity.State = EntityState.Detached;

            //This should have reverted because we had a DbUpdateException. 
            shift2 = await Db.Shift.FindAsync(9000000);
            Assert.Equal(1 ,shift2.StartDate.Year);
            Assert.Equal(1, shift2.EndDate.Year);
        }
    }
}
