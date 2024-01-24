using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using db.models;
using Mapster;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CAS.DB.models.audit
{
    public class Audit : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string TableName { get; set; }
        [AdaptIgnore]
        public JsonDocument KeyValues { get; set; }
        [AdaptIgnore]
        public JsonDocument OldValues { get; set; }
        [AdaptIgnore]
        public JsonDocument NewValues { get; set; }
        
        [NotMapped] 
        public JObject KeyValuesJson => (JObject)JsonConvert.DeserializeObject(System.Text.Json.JsonSerializer.Serialize(KeyValues?.RootElement));
        [NotMapped]
        public JObject OldValuesJson => (JObject)JsonConvert.DeserializeObject(System.Text.Json.JsonSerializer.Serialize(OldValues?.RootElement));
        [NotMapped]
        public JObject NewValuesJson => (JObject)JsonConvert.DeserializeObject(System.Text.Json.JsonSerializer.Serialize(NewValues?.RootElement));
    }
}
