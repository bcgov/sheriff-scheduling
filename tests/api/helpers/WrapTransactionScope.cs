using System;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using CAS.DB.models;
using tests.api.Helpers;

namespace tests.api.helpers
{
    /// <summary>
    /// Simple wrapper, that wraps our unit tests.
    /// </summary>
    public class WrapInTransactionScope : IDisposable
    {
        private readonly TransactionScope _scope;
        public bool CommitTxn { get; set; }

        protected CourtAdminDbContext Db { get; }

        public WrapInTransactionScope(bool useMemoryDatabase)
        {
            Db = new CourtAdminDbContext(EnvironmentBuilder.SetupDbOptions(useMemoryDatabase: useMemoryDatabase));
            _scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        }
        
        protected void Detach()
        {
            foreach (var entity in Db.ChangeTracker.Entries())
                entity.State = EntityState.Detached;
        }

        public void Dispose()
        {
            if (CommitTxn) _scope.Complete();
            _scope.Dispose();
        }

    }
}
