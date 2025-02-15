namespace Circular.Core.DTOs
{
    public class BookingsDTO : BaseEntityDTO
    {
        public long CommunityId { get; set; }
        public string Title { get; set; }
        public string Days { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long NoOfBooking { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string HostName { get; set; }
        public string? Email { get; set; }
        public bool IsNotificationActive { get; set; }
        public string CommunityName { get; set; }
    }
}
