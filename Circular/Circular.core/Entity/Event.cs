using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblEvent")]
public class Event : BaseEntity
{
    public Event()
    {
        if (QRCode == null)
            QRCode = new QR();
    }
    public string Title { get; set; }
    public string Location { get; set; }
    public DateTime EventStartDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Description { get; set; }
    public string CoverImage { get; set; }
    public DateTime EventEndDate { get; set; }
    public bool IsPayEnabled { get; set; }
    public bool IsScheduleNotificationSent { get; set; }
    public bool IsCheckInEnabled { get; set; }
    public bool IsContactOrganiserEnabled { get; set; }
    public long? OrganiserId { get; set; }
    public long? CommunityId { get; set; }
    public string? ColorCode { get; set; }
    public long? GroupId { get; set; }
    public string? EventPdf { get; set; }
    public DateTime? ScheduleDate { get; set; }
    public TimeSpan? ScheduleTime { get; set; }
    public int TicketCount { get; set; }
    public decimal TicketPrice { get; set; }
    public string CommunityName { get; set; }
    public int? IsFree { get; set; }
    public string? OrganizerName { get; set; }
    public string? OrganizerProfilePic { get; set; }
    public string? Mobile { get; set; }
    public int? IsMemberPaid { get; set; }
    public int? IsMemberCheckedIn { get; set; }

    public int? IsMemberRSVP { get; set; }
    public int? IsMemberArrived { get; set; }
    public long? PaidById { get; set; }
    public string? PaidByName { get; set; }
    public string? PaidByMobile { get; set; }
    public string? InviteeMobile { get; set; }
    public decimal? Amount { get; set; }
    public decimal? CheckinLatitude { get; set; }
    public decimal? CheckInLongitude { get; set; }
    public string? CheckedinTime { get; set; }
    public long? TransactionId { get; set; }
    public int? ConfirmedtTicketCount { get; set; }
    public int? RemainingTicketCount { get; set; }
    public int? IsCompleted { get; set; }
    public long? InviteeId { get; set; }
    public QR QRCode { get; set; }

    public string? GroupName { get; set; }
    public string? InviteeName { get; set; }
    public long? InvitationId { get; set; }

    public string? InviteeProfilePic { get; set; }

    public decimal AmountCollected { get; set; }

    public long? PaidCount { get; set; }

    public long TotalMembersAtCreation { get; set; }


    public override void ApplyKeys()
    {

    }
}

public class EventListResponse
{
    public EventListResponse()
    {
        this.EventGroups = new List<EventGroupResponse>();
    }
    public List<EventGroupResponse> EventGroups { get; set; }

}
public class EventGroupResponse
{
    public EventGroupResponse()
    {
        this.EventList = new List<Event>();
    }
    public DateTime? EventDate { get; set; }
    public List<Event>? EventList { get; set; }
}



