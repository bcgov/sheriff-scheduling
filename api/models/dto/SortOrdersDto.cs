using System.Collections.Generic;

namespace CAS.API.models.dto
{
    public class SortOrdersDto
    {
        public int? SortOrderLocationId { get; set; }
        public List<SortOrderDto> SortOrders { get; set; }
    }
}
