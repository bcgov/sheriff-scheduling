using System.Collections.Generic;
using CAS.DB.models.scheduling;

namespace CAS.API.models
{
    public class ShiftConflict
    {
        public Shift Shift { get; set; }
        public List<string> ConflictMessages { get; set; } = new List<string>();
    }
}
