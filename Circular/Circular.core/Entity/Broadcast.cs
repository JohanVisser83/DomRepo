using RepoDb.Attributes;

namespace Circular.Core.Entity;
[Map("tblBroadcast")]

public class Broadcast : BaseEntity
{

    public long? CommunityId { get; set; }

    public long? IsGroup { get; set; }

    public long? ReferenceId { get; set; }

    public string? Title { get;set; }

    public string? Text { get; set; }

    public string? MessageMedia { get; set; }
    public string? MessageMediaThumbnail { get; set; }

    public DateTime? ScheduleDateTime { get; set; }

    public DateTime? ScheduleTime { get; set; }

    public long? MessageTypeId { get; set; }

    public string Email { set; get; }

    public string CommunityName { get; set; }

    public long CustomerId { get; set; }

    public string Audience { get; set; }

    public bool IsScheduleNotificationSent { get; set; }

    public string BroadcastStatus { 
        get 
        {
            if (IsScheduleNotificationSent)
                return "Broadcast Sent";
            else
                return "In Queue";
        } 
    }
    public override void ApplyKeys()
    {

    }
}
