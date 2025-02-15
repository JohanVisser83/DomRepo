using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("TblBookingDays")]
public class BookingDays : BaseEntity 
{
    public long? CommunityId { get; set; }
    public long? MasterBookingId { get; set; }
    public string? Title { get; set; }
    public string? Days { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public long? NoOfBooking { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? HostName { get; set; }
    public string? Email { get; set; }
    public bool? IsNotificationActive { get; set; }
    public string? CommunityName { get; set; }

    public int? BookedIn { get; set; }
    public long? CountOfBooking { get; set; }
    public int? IsFull { get; set; }
    public long? CustomerBookingId { get; set; }

    public override void ApplyKeys()
    {

    }
}
