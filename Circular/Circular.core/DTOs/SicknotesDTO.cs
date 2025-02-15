namespace Circular.Core.DTOs
{
    public class SicknotesDTO
    {
        public long? CustomerId { get; set; }
        public long? ForCustomerId { get; set; }
        public DateTime? Fromdate { get; set; }
        public DateTime? Todate { get; set; }
        public string? Url { get; set; }
        public long? CommunityId { get; set; }
        public string? FullName { get; set; }
        public string? ForCustomerName { get; set; }
    }
}
