using System.Collections.Generic;
using CAS.API.models.dto.generated;

namespace CAS.API.models.dto.generated
{
    public partial class ImportedShiftsDto
    {
        public List<ShiftDto> Shifts { get; set; }
        public List<string> ConflictMessages { get; set; }
    }
}