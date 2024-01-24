using System.Transactions;
using CAS.DB.models;
using tests.api.Helpers;
using Xunit;

namespace tests.db
{
    /// <summary>
    /// Be careful running this test against any database, it will attempt to write data and have it persist.
    /// TransactionScope allows you to have nested transactions, where as Database.BeginTransaction() does not. 
    /// This should allow us to wrap multiple C# service calls into a TransactionScope later on.
    /// </summary>
    public class TransactionScopeTest
    {
        [Fact]
        public void RollbackDataTest()
        {
            var lookupCode = new LookupCode
            {
                Description = "desc"
            };
            using (var dbContext = new CourtAdminDbContext(EnvironmentBuilder.SetupDbOptions()))
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (TransactionScope scope2 = new TransactionScope())
                    {
                        dbContext.LookupCode.Add(lookupCode);
                        dbContext.SaveChanges();
                        scope2.Complete();
                    }
                    //No scope complete, should rollback. 
                }
            }

            using (var dbContext = new CourtAdminDbContext(EnvironmentBuilder.SetupDbOptions()))
            {
                var workSectionCode2 = dbContext.LookupCode.Find(lookupCode.Id);
                Assert.Null(workSectionCode2);
            }
        }

        [Fact]
        public void PersistingDataTest()
        {
            var lookupCode = new LookupCode
            {
                Description = "desc"
            };

            using (var dbContext = new CourtAdminDbContext(EnvironmentBuilder.SetupDbOptions()))
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (TransactionScope scope2 = new TransactionScope())
                    {
                        dbContext.LookupCode.Add(lookupCode);
                        dbContext.SaveChanges();
                        scope2.Complete();
                        scope.Complete();
                    }
                }
            }

            using (var dbContext = new CourtAdminDbContext(EnvironmentBuilder.SetupDbOptions()))
            {
                var workSectionCode2 = dbContext.LookupCode.Find(lookupCode.Id);
                Assert.NotNull(workSectionCode2);
                dbContext.Remove(workSectionCode2);
                dbContext.SaveChanges();
            }
        }
    }
}
