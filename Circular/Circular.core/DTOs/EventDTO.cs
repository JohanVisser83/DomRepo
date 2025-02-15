using Circular.Core.Entity;
using Microsoft.AspNetCore.Http;
namespace Circular.Core.DTOs
{
    public class EventDTO : BaseEntityDTO
    {
        public string? Title { get; set; }
        public string? Location { get; set; }
        //public DateTime EventStartDate { get; set; }
        public DateTime EventStartDate { get; set; }

        //public DateTime GetEventStartDateAsDateTime()
        //{
        //    return DateTime.Parse(EventStartDate);
        //}
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
       // public string ScheduleDate { get; set; }
        public string? Description { get; set; }
        public bool IsScheduleNotificationSent { get; set; }
        public string? CoverImage { get; set; }
        //public DateTime EventEndDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public bool IsPayEnabled { get; set; }
        public bool IsCheckInEnabled { get; set; }
        public bool IsContactOrganiserEnabled { get; set; }
        public long? OrganiserId { get; set; }
        public long CommunityId { get; set; }
        public string? ColorCode { get; set; } = "";
        public long? GroupId { get; set; }
        public IFormFile? Mediafile { get; set; }
        public IFormFile? MediaCoverfile { get; set; }
        public string? EventPdf { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public TimeSpan? ScheduleTime { get; set; }
        public int TicketCount { get; set; }
        public decimal TicketPrice { get; set; }
        public string? CommunityName { get; set; }
        public int? IsFree { get; set; }
        public string? OrganizerName { get; set; }
        public string? OrganizerProfilePic { get; set; }
        public string? Mobile { get; set; }
        public int? IsMemberPaid { get; set; }
        public int? IsMemberCheckedIn { get; set; }
        public int? IsMemberRSVP { get; set; }
        public int? IsMemberArrived { get; set; }
        public long? PaidById { get; set; }
        public decimal? Amount { get; set; }
        public decimal? CheckinLatitude { get; set; }
        public decimal? CheckInLongitude { get; set; }
        public string? CheckedinTime { get; set; }
        public long? TransactionId { get; set; }
        public int? ConfirmedtTicketCount { get; set; }
        public int? RemainingTicketCount { get; set; }
        public int? IsCompleted { get; set; }
        public string? PaidByMobile { get; set; }
        public string? InviteeMobile { get; set; }
        public QR QRCode { get; set; }

        public string? GroupName { get; set; }

        public string? InviteeName { get; set; }

        public long? InvitationId { get; set; }

        public string? InviteeProfilePic { get; set; }

        public string? PaidByName { get; set; }


    }
}
