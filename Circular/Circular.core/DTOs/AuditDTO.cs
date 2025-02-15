namespace Circular.Core.DTOs
{
    public class AuditDTO
    {
        public long? CustomerId { get; set; }
        public long? LinkedCustomerId { get; set; }
        public long? Communityid { get; set; }
        public long? TransactionId { get; set; }
        public string? Activity { get; set; }
        public string? ActivityDesc { get; set; }
        public long? InterfaceTypeId { get; set; }
        public string? Device { get; set; }
        public string? ClientDetail { get; set; }
    }
}
