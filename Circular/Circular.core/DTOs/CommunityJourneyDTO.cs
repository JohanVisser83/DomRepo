namespace Circular.Core.DTOs
{
    public class CommunityJourneyDTO
    {
        public long OrgId { get; set; }
        public long DriverId { get; set; }
        public string Title { get; set; }
        public string? Time { get; set; }
        public string? Date { get; set; }
        public bool? IsRecurring { get; set; }
    }
}
