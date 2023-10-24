namespace CAS.API.models.dto.generated
{
    public partial class LookupSortOrderDto
    {
        public int Id { get; set; }
        public int LookupCodeId { get; set; }
        public int? LocationId { get; set; }
        public int SortOrder { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}