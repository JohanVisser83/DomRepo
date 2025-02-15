
using Circular.Core.Entity;
using Circular.Framework.Notifications;

namespace Circular.Services.Notifications
{
    public interface INotificationService
    {
        public int Notify(NotificationTypes Notification_Type, string NotificationTopic, string NotificationTitle,
            string NotificationBody, long ReferenceId, decimal ReferenceAmount, bool ReferenceOption, string ReferenceOptionalValue,
            long SenderCustomerReferenceId, long GroupReferenceId, string NotificationMedia, long ReceiverCustomerReferenceId);
        public Task<long> SaveNotificationAsync(Notification notifications, long ReferenceId);
        public Task<Core.Entity.NotificationListResponse> GetNotificationsAsync(long UserId, long userNotificationId, int IsRead, long pagenumber, long pagesize);
        Task<int> ReadNotificationAsync(long UserId, long Id, bool IsReadAll);
        Task<int> DeleteNotificationAsync(long Id);
        int GetMemberCount(long GroupId, long communityId, long customerId);
    }
}
