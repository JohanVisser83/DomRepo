using Circular.Core.DTOs;

namespace Circular.Core.DTOs
{
    public class UserNotificationsDTO
    {
        public long CustomerId { get; set; }
        public long NotificationId { get; set; }
        public bool IsRead { get; set; }
    }
}

public class UserNotificationUnreadsDTO : BaseEntityDTO
{
    public long CustomerId { get; set; }
    public long UnreadNotificationCount { get; set; }
}