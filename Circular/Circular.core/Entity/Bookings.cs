using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblBookings")]
public class Bookings : BaseEntity 
{
    public long CommunityId { get; set; }
    public string? Title { get; set; }
    public string? Days { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public long NoOfBooking { get; set; }
    public long BookingCount { get; set; }
    public long TotalSpaces { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? HostName { get; set; }
    public string? Email { get; set; }
    public string? WeekDays { get; set; }
    public bool IsNotificationActive { get; set; }

    

    public string? CommunityName { get; set; }
    public override void ApplyKeys()
    {

    }
}
