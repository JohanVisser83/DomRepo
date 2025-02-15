using Circular.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Data.Repositories.Notification
{
    public interface INotificationRepository : IRepository<Core.Entity.Notification>
    {
        public Task<Core.Entity.NotificationListResponse> GetNotificationsAsync(long UserId, long userNotificationId,int IsRead, long pagenumber, long pagesize);
        Task<int> ReadNotificationAsync(long UserId, long Id, bool IsReadAll);
        Task<int> DeleteNotificationAsync(long Id);
        Task<long> CreateAsync(Core.Entity.Notification entity, long ReferenceId);
        Task<IEnumerable<CustomerCommunity>?> GetCommunityMembers(long communityId);
        Task<IEnumerable<CustomerGroups>?> GetCommunityGroupMembers(long groupId);
        Task<int> GetTotalMemberCount(long GroupId, long communityId);

    }
}
