using Circular.Core.Entity;

namespace Circular.Core.DTOs
{
    public class NotificationsDTO
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
    }

    public class NotificationsRequestDTO
    {
        public long customerId { get; set; }
        public long userNotificationId { get; set; }
        public int IsRead { get; set; } = 2;
        public long pageNumber { get; set; } = 1;
        public long pageSize { get; set; } = 10;
    }
    public class ReadNotificationsRequest
    {
        public long customerId { get; set; }
        public long userNotificationId { get; set; }
        public bool IsReadAll { get; set; } = false;
    }

    public class UserNotificationsRequest
    {
        public long userNotificationId { get; set; }
    }
}
