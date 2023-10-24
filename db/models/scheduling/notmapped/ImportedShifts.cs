using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Mapster;

namespace CAS.DB.models.scheduling.notmapped
{
    [AdaptTo("[name]Dto")]
    public class ImportedShifts
    {
        [NotMapped]
        public List<Shift> Shifts { get; set; } = new List<Shift>();
        [NotMapped]
        public List<string> ConflictMessages { get; set; } = new List<string>();
    }
}
