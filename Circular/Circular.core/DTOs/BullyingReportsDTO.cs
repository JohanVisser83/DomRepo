namespace Circular.Core.DTOs
{
    public class BullyingReportsDTO
    {
        public long CustomerId { get; set; }
		public long CommunityId { get; set; }
		public int IncidentTypeId { get; set; }
        public string? ReportingPersonName { get; set; }        
        public DateTime? IncidentTime { get; set; }
        public string? IncidentLocation { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
    }
}
