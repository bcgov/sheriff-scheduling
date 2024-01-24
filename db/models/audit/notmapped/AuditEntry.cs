using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CAS.DB.models.audit.notmapped
{
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }

        public EntityEntry Entry { get; }
        public string TableName { get; set; }
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();

        public bool HasTemporaryProperties => TemporaryProperties.Any();

        //I used System.Text.Json because I think the Npgsql EF Core provider has support to convert LINQ -> SQL.
        public Audit ToAudit(Guid? createdById)
        {
            var audit = new Audit();
            audit.TableName = TableName;
            audit.KeyValues = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(KeyValues));
            audit.OldValues = OldValues.Count == 0 ? null : JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(OldValues));
            audit.NewValues = NewValues.Count == 0 ? null : JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(NewValues));
            audit.CreatedById = createdById;
            return audit;
        }
    }
}
