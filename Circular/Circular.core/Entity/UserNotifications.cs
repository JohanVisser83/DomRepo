using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblUserNotifications")]
public class UserNotifications : BaseEntity
{
    public long CustomerId { get; set; }
    public long NotificationId { get; set; }
    public bool IsRead { get; set; }
    public override void ApplyKeys()
    {

    }
}
[Map("tblUserNotificationUnreads")]
public class UserNotificationUnreads : BaseEntity
{
    public long CustomerId { get; set; }
    public long UnreadNotificationCount { get; set; }
    public override void ApplyKeys()
    {

    }
}