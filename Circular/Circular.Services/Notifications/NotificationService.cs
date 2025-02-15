using AutoMapper.Execution;
using Circular.Core.Entity;
using Circular.Data.Repositories.Notification;
using Circular.Framework.Logger;
using Circular.Framework.Notifications;
using Circular.Services.Community;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Threading;


namespace Circular.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly ILoggerManager _logger;
        private readonly IConfiguration _configuration;
        private readonly INotificationRepository _notificationRepository;
        public NotificationService(IConfiguration configuration, ILoggerManager logger, 
            INotificationRepository notificationRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        }

        public int Notify(NotificationTypes Notification_Type, string NotificationTopic, string NotificationTitle,
            string NotificationBody, long ReferenceId, decimal ReferenceAmount, bool ReferenceOption, 
            string ReferenceOptionalValue,long SenderCustomerReferenceId, 
            long GroupReferenceId, string NotificationMedia, long ReceiverCustomerReferenceId)
        {
            try
            {
                NotificationTopic = (_configuration.GetSection("Environment").Value) + "_" + NotificationTopic;
                NotificationPayload _payload = Payload(Notification_Type, NotificationTopic, NotificationTitle,
                NotificationBody, ReferenceId, SenderCustomerReferenceId, ReferenceAmount,
                GroupReferenceId, NotificationMedia, ReferenceOption, ReceiverCustomerReferenceId);

                if(_payload.NotificationTypeId != (long)NotificationTypes.Message_Received && _payload.NotificationTypeId !=(long)NotificationTypes.Message_Sent )
                    _payload.NotificationId = SaveNotificationAsync(FillNotifications(_payload), _payload.NotificationReceiverId).Result;
               
                int result = new FirebaseNotification().Send(_payload);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private NotificationPayload Payload(NotificationTypes Notification_Type, string NotificationTopic, string NotificationTitle, 
            string NotificationBody,long ReferenceId, long senderId,decimal ReferenceAmount,long GroupReferenceId,
            string NotificationMedia,bool referenceOption, long ReceiverId)
        {
            NotificationPayload _payload = new NotificationPayload();
            _payload.NotificationPath = (_configuration.GetSection("NotificationJSONPath").Value);
            _payload.NotificationTopic = NotificationTopic;
            _payload.NotificationTitle = NotificationTitle;
            _payload.NotificationBody = NotificationBody;
            _payload.NotificationReferenceId = ReferenceId;
            _payload.NotificationTypeId = (int)Notification_Type;
            _payload.NotificationSenderId = senderId;
            _payload.NotificationMedia = NotificationMedia;
            _payload.NotificationGroupId = GroupReferenceId;
            _payload.NotificationAmount = ReferenceAmount;
            _payload.NotificationIsBroadcast = referenceOption;
            _payload.NotificationReceiverId = ReceiverId;
            return _payload;
        }
        private Core.Entity.Notification FillNotifications(NotificationPayload _payload)
        {
            Core.Entity.Notification notification = new Core.Entity.Notification();
            notification.NotificationBody = _payload.NotificationBody;
            notification.NotificationTitle = _payload.NotificationTitle;
            notification.NotificationTypeId = (int)_payload.NotificationTypeId;
            notification.Amount = _payload.NotificationAmount ?? 0;
            notification.CommunityId = _payload.NotificationGroupId;
            notification.NotificationMedia = _payload.NotificationMedia;
            notification.IsBroadcast = _payload.NotificationIsBroadcast;
            notification.TransactionId = _payload.NotificationReferenceId??0;
            notification.UserNotification = fillUserNotification(_payload);
            notification.FillDefaultValues();
            notification.CreatedBy = _payload.NotificationSenderId;
            return notification;
        }
        private List<UserNotifications> fillUserNotification(NotificationPayload _payload)
        {
            IEnumerable<UserNotifications> _userNotifications = new List<UserNotifications>();
            if ((_payload.NotificationTypeId == (long)NotificationTypes.New_Event) || (_payload.NotificationTypeId == (long)NotificationTypes.New_Account) || (_payload.NotificationTypeId == (long)NotificationTypes.New_Article) || (_payload.NotificationTypeId == (long)NotificationTypes.Poll))
            {
                if (_payload.NotificationReceiverId == 0)
                    _userNotifications =
                    _notificationRepository.GetCommunityMembers(_payload.NotificationGroupId)
                    .Result.Select(cc => new UserNotifications
                    {
                        CustomerId = cc.CustomerId,
                        IsRead = false,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        CreatedBy = 101,
                        ModifiedBy = 101,
                        IsActive = true,
                        GUID = new Guid()
                    });
                else if (_payload.NotificationReceiverId < 0)
                {
                    UserNotifications userNotification = new UserNotifications();
                    userNotification.IsRead = false;
                    userNotification.CustomerId = _payload.NotificationReceiverId;
                    userNotification.FillDefaultValues();
                    _userNotifications = _userNotifications.Append(userNotification);
                }
                else
                    _userNotifications =
                    _notificationRepository.GetCommunityGroupMembers(_payload.NotificationReceiverId)
                     .Result.Select(cc => new UserNotifications
                     {
                         CustomerId = cc.CustomerId ?? 0,
                         IsRead = false,
                         CreatedDate = DateTime.Now,
                         ModifiedDate = DateTime.Now,
                         CreatedBy = 101,
                         ModifiedBy = 101,
                         IsActive = true,
                         GUID = new Guid()
                     });
            }
            else
            {
                UserNotifications userNotification = new UserNotifications();
                userNotification.IsRead = false;
                userNotification.CustomerId = _payload.NotificationReceiverId;
                userNotification.FillDefaultValues();
                _userNotifications = _userNotifications.Append(userNotification);
            }
            return _userNotifications.ToList();
        }
        private List<UserNotificationUnreads> fillUserNotificationUnreads(NotificationPayload _payload)
        {
            IEnumerable<UserNotificationUnreads> _userNotificationUnreads = new List<UserNotificationUnreads>();
            if ((_payload.NotificationTypeId == (long)NotificationTypes.New_Event))
            {
                if (_payload.NotificationReceiverId <= 0)
                    _userNotificationUnreads =
                    _notificationRepository.GetCommunityMembers(_payload.NotificationGroupId)
                    .Result.Select(cc => new UserNotificationUnreads
                    {
                        CustomerId = cc.CustomerId,
                        UnreadNotificationCount = 1,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        CreatedBy = 101,
                        ModifiedBy = 101,
                        IsActive = true,
                        GUID = new Guid()
                    });
                else
                    _userNotificationUnreads =
                    _notificationRepository.GetCommunityGroupMembers(_payload.NotificationReceiverId)
                     .Result.Select(cc => new UserNotificationUnreads
                     {
                         CustomerId = cc.CustomerId ?? 0,
                         UnreadNotificationCount = 1,
                         CreatedDate = DateTime.Now,
                         ModifiedDate = DateTime.Now,
                         CreatedBy = 101,
                         ModifiedBy = 101,
                         IsActive = true,
                         GUID = new Guid()
                     });
            }
            else
            {
                UserNotificationUnreads userNotificationUnreads = new UserNotificationUnreads()
                {
                    CustomerId = _payload.NotificationReceiverId,
                    UnreadNotificationCount = 1
                };
                _userNotificationUnreads = _userNotificationUnreads.Append(userNotificationUnreads);
            }
            return _userNotificationUnreads.ToList();
        }
        public Task<long> SaveNotificationAsync(Core.Entity.Notification notifications, long ReferenceId)
        {
            return _notificationRepository.CreateAsync(notifications, ReferenceId);
        }
        public async Task<Core.Entity.NotificationListResponse> GetNotificationsAsync(long UserId, long userNotificationId, int IsRead, long pagenumber, long pagesize)
        {
            return await _notificationRepository.GetNotificationsAsync(UserId, userNotificationId, IsRead, pagenumber, pagesize);
        }
        public async Task<int> ReadNotificationAsync(long UserId, long Id, bool IsReadAll)
        {
            return await _notificationRepository.ReadNotificationAsync(UserId,Id,IsReadAll);
        }
        public async Task<int> DeleteNotificationAsync(long Id)
        {
            return await _notificationRepository.DeleteNotificationAsync(Id);
        }

        public int GetMemberCount(long GroupId, long communityId, long CustomerId)
        {
            if (CustomerId > 0)
                return 1;
            if (GroupId != 0)
                return _notificationRepository.GetTotalMemberCount(GroupId, 0).Result;
            else
                 return _notificationRepository.GetTotalMemberCount(0,communityId).Result;
        }

    }
}
