using System.IO;
using System.Text;
using System.Text.Json;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using CAS.DB.models;
using CAS.DB.models.audit;
using tests.api.Helpers;

namespace tests.api.helpers
{
    public class WrapTransactionScopeInMemory
    {
        private readonly TransactionScope _scope;
        public bool CommitTxn { get; set; }

        protected MemorySheriffDbContext Db { get; }

        public WrapTransactionScopeInMemory(bool useMemoryDatabase)
        {
            Db = new MemorySheriffDbContext(EnvironmentBuilder.SetupDbOptions(true));
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

    public class MemorySheriffDbContext : CourtAdminDbContext
    {
        public MemorySheriffDbContext(DbContextOptions<CourtAdminDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Audit>().Property(p => p.KeyValues)
                .HasConversion(
                    v => JsonDocumentToString(v),
                    v => JsonDocument.Parse(v, new JsonDocumentOptions()));

            modelBuilder.Entity<Audit>().Property(p => p.NewValues)
                .HasConversion(
                    v => JsonDocumentToString(v),
                    v => JsonDocument.Parse(v, new JsonDocumentOptions()));

            modelBuilder.Entity<Audit>().Property(p => p.OldValues)
                .HasConversion(
                    v => JsonDocumentToString(v),
                    v => JsonDocument.Parse(v, new JsonDocumentOptions()));
            base.OnModelCreating(modelBuilder);
        }

        private static string JsonDocumentToString(JsonDocument document)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
                document.WriteTo(writer);
                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }

}
