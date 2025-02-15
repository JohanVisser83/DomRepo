using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblNotifications")]
public class Notification : BaseEntity
{
    public long CommunityId { get; set; }
    public long NotificationTypeId { get; set; }
    public string NotificationTitle { get; set; }
    public string NotificationBody { get; set; }
    public string? NotificationMedia { get; set; }
    public long TransactionId { get; set; }
    public string? YesButtonLink { get; set; }
    public decimal Amount { get; set; }
    public bool IsBroadcast { get; set; } = false;
    public long? BroadcastType { get; set; }
    public long? BroadcastValue { get; set; }
    public long? UserNotificationId { get; set; }
    public bool IsRead { get; set; } = false;

    public long? SenderId { get; set; }
    public string? SenderName { get; set; }
    public string? SenderMobile { get; set; }
    public string? PaymentDesc { get; set; }

    public List<UserNotifications> UserNotification { get; set; }

    public override void ApplyKeys()
    {
        if (UserNotification != null)
            foreach (var User in UserNotification)
                User.NotificationId = Id;
    }
}


public class NotificationListResponse
{
    public NotificationListResponse()
    {
        this.NotificationGroups = new List<NotificationGroupResponse>();
    }
    public List<NotificationGroupResponse> NotificationGroups { get; set; }

}
public class NotificationGroupResponse
{
    public NotificationGroupResponse()
    {
        this.NotificationList = new List<Notification>();
    }
    public DateTime? NotificationDate { get; set; }
    public List<Notification>? NotificationList { get; set; }
}