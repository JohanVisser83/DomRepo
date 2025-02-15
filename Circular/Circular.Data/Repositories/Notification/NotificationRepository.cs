using Circular.Core.Entity;
using Circular.Framework.Notifications;
using Microsoft.Data.SqlClient;
using RepoDb;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Circular.Data.Repositories.Notification
{
    public class NotificationRepository : DbRepository<SqlConnection>, INotificationRepository
    {
        public NotificationRepository(string connectionString) : base(connectionString)
        {
        }
        public async Task<long> CreateAsync(Core.Entity.Notification entity, long ReferenceId)
        {
            try
            {
                int result = 0;
                long key = await InsertAsync<Core.Entity.Notification, int>(entity);
                entity.Id = (long)key;
                entity.ApplyKeys();
                if (entity.UserNotification != null && entity.UserNotification.Count > 0)
                    result = await InsertAllAsync<UserNotifications>(entity.UserNotification);

                InsertAndUpdateUserUnreadsInBulk(entity.NotificationTypeId, ReferenceId,
                    entity.CommunityId, ReferenceId, 1);

                return key;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<Core.Entity.NotificationListResponse?> GetNotificationsAsync
            (long UserId,long userNotificationId,int IsRead, long pagenumber, long pagesize)
        {
            List<Core.Entity.Notification> lstNotifications =  GetNotificationsRawList(UserId, userNotificationId,IsRead,pagenumber,pagesize);
            NotificationListResponse notificationListResponse = null;
            if (lstNotifications != null)
            {
                notificationListResponse = new NotificationListResponse();
                lstNotifications.ForEach(n =>
                {
                    // group the data in dates
                    int index = notificationListResponse.NotificationGroups.FindIndex(ng => ng.NotificationDate == n.CreatedDateOnly);
                    if (index == -1)
                    {
                        NotificationGroupResponse notificationGroupResponse = new NotificationGroupResponse();
                        notificationGroupResponse.NotificationDate = n.CreatedDateOnly;
                        notificationGroupResponse.NotificationList?.Add(n);
                        notificationListResponse.NotificationGroups.Add(notificationGroupResponse);
                    }
                    else
                        notificationListResponse.NotificationGroups[index].NotificationList?.Add(n);
                }
                );
            }
            return notificationListResponse;
        }

        public  List<Core.Entity.Notification> GetNotificationsRawList
            (long UserId, long userNotificationId, int IsRead, long pagenumber, long pagesize)
        {
            return ExecuteQueryAsync<Core.Entity.Notification>("Exec [dbo].[USP_Customer_Notifications] "
                    + UserId + "," + IsRead + "," + pagesize + "," + pagenumber + "," + userNotificationId).Result.ToList<Core.Entity.Notification>();
        }
        public async Task<int> ReadNotificationAsync(long UserId, long Id, bool IsReadAll)
        {
            IEnumerable<Core.Entity.Notification> notifications = new List<Core.Entity.Notification>();
            IEnumerable<Core.Entity.UserNotifications> userNotifications = new List<Core.Entity.UserNotifications>();

            if (IsReadAll)
                notifications = GetNotificationsRawList(UserId,0, 0, 1, 1000000);
            else
                notifications = GetNotificationsRawList(UserId, Id, 0, 1, 10);
            var fields = Field.Parse<UserNotifications>(e => new
            {
                e.Id,
                e.IsRead,
                e.ModifiedDate
            });
            var Qfields = Field.Parse<UserNotifications>(e => new
            {
                e.Id
            });
            userNotifications = notifications.Select(n => new UserNotifications
                                {
                                    Id = n.UserNotificationId ?? 0,
                                    CustomerId = UserId,
                                    IsRead = true,
                                    ModifiedDate = DateTime.Now
                                });
            int updatedRows = await UpdateAllAsync<UserNotifications>(entities: userNotifications, qualifiers: Qfields,  fields: fields);
            UpdateUserUnreads(UserId,true,updatedRows);
            return updatedRows;
        }
        public async Task<int> DeleteNotificationAsync(long Id)
        {
            int updatedRows = 0;
            UserNotifications entity = QueryAsync<UserNotifications?>(e => e.Id == Id && e.IsActive == true).Result.FirstOrDefault() ?? null;
            if (entity != null)
            {
                entity.IsActive = false;
                entity.ModifiedDate = DateTime.Now;
                var fields = Field.Parse<UserNotifications>(e => new
                {
                    e.IsActive,
                    e.ModifiedDate
                });
                updatedRows = await UpdateAsync<UserNotifications>(entity: entity, fields: fields);
                if(!entity.IsRead)
                    UpdateUserUnreads(entity.CustomerId, true, 1);
            }
            return updatedRows;
        }
       
        public async void UpdateUserUnreads(long CustomerId, bool IsRead, int readCount = 1)
        {
            UserNotificationUnreads userNotificationUnreads
             = QueryAsync<UserNotificationUnreads?>(e => e.CustomerId == CustomerId && e.IsActive == true).Result.FirstOrDefault() ?? null;
            if (userNotificationUnreads != null)
            {
                var fields = Field.Parse<UserNotificationUnreads>(e => new
                {
                    e.UnreadNotificationCount,
                    e.ModifiedDate
                });
                if(!IsRead)
                userNotificationUnreads.UnreadNotificationCount = userNotificationUnreads.UnreadNotificationCount + readCount;
                else
                    userNotificationUnreads.UnreadNotificationCount = userNotificationUnreads.UnreadNotificationCount - readCount;
                if (userNotificationUnreads.UnreadNotificationCount < 0)
                    userNotificationUnreads.UnreadNotificationCount = 0;
                userNotificationUnreads.ModifiedDate = DateTime.Now;
                await UpdateAsync<UserNotificationUnreads>(entity: userNotificationUnreads, fields: fields);
            }
            else
            {
                if (!IsRead)
                {
                    userNotificationUnreads = new UserNotificationUnreads()
                    {
                        CustomerId = CustomerId,
                        UnreadNotificationCount = 1
                    };
                    userNotificationUnreads.FillDefaultValues();
                    await InsertAsync<UserNotificationUnreads>(userNotificationUnreads);
                }
            }
        }
        
        //For Bulk inserts in UserUnreads
        public async void InsertUserUnreads(List<UserNotificationUnreads> userNotificationUnreads)
        {
            await InsertAllAsync<UserNotificationUnreads>(userNotificationUnreads);
        }
        public void InsertAndUpdateUserUnreadsInBulk
        (long NotificationTypeId, long UserId, long CommunityId, long GroupId, int UnreadCount)
        {
            ExecuteNonQueryAsync("Exec [dbo].[Usp_Notifications_Unreads] " +
              NotificationTypeId + "," + UserId + "," + CommunityId + "," + GroupId + "," + UnreadCount);
        }

        public async Task<IEnumerable<CustomerGroups>?> GetCommunityGroupMembers(long groupId)
        {
            return QueryAll<CustomerGroups>().Where(x => x.GroupId == groupId && x.IsActive == true).ToList();
        }
        public async Task<IEnumerable<CustomerCommunity>?> GetCommunityMembers(long communityId)
        {
            return QueryAll<CustomerCommunity>().Where(x => x.CommunityId == communityId && x.IsActive == true).ToList();
        }

        public async Task<int> GetTotalMemberCount(long GroupId,long communityId)
        {
            return ExecuteScalarAsync<int>("Exec [dbo].[USP_GetTotalMemberCount] " + GroupId  +"," + communityId).Result;
        }


        #region "Not Implemented"
        public Task<IEnumerable<Core.Entity.Notification>?> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        public Task<Core.Entity.Notification?> GetAsync(long id)
        {
            throw new NotImplementedException();
        }
        public Task<int> UpdateAsync(Core.Entity.Notification entity)
        {
            throw new NotImplementedException();
        }
        public Task<int> DeleteAsync(Core.Entity.Notification entity)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<Core.Entity.Notification>?> GetAsync(Expression<Func<Core.Entity.Notification, bool>> where)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CreateAsync(Core.Entity.Notification entity)
        {
            throw new NotImplementedException();
        }
        #endregion

    }

}
