using Circular.Services.Planners;
using Circular.Services.User;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Circular.Core.DTOs;
using Circular.Framework.Notifications;
using Circular.Services.Notifications;
using NLog;

namespace CircularScheduledJobs
{
    public class EventNotification
    {
        private readonly IPlannerService _plannerService;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;

        public EventNotification(IPlannerService plannerService, IConfiguration configuration , INotificationService notificationService)
        {
            _plannerService = plannerService ?? throw new ArgumentNullException(nameof(plannerService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }


        public void SendEventScheduleNotification()
        {
            var logger = LogManager.GetLogger("database");
            logger.Info("Inside SendEventScheduleNotification");

            List<EventNotificationDTO> x = _plannerService.EventScheduleNotification().Result;
            logger.Info(x.Count().ToString());
            foreach (EventNotificationDTO item in x)
            {
                if ((item.GroupId ?? 0) <= 0)
                {
                    logger.Info("Sending Notification for all");
                    _notificationService.Notify(NotificationTypes.New_Event,
                    NotificationTopics.Circular_community_ReferenceId.ToString().Replace("ReferenceId", item.CommunityId.ToString()),
                    item.CommunityName, item.Title + " event is just added in your community."
                    , item.Id??0, item.TicketPrice??0, false, "", item.CreatedBy??0, item.CommunityId ?? 0, "", item.GroupId ?? 0);
                }
                else
                {
                    logger.Info("Sending Notification for a group");
                    _notificationService.Notify(NotificationTypes.New_Event,
                    NotificationTopics.Circular_communityGroups_ReferenceId.ToString().Replace("ReferenceId", item.GroupId.ToString()),
                    item.CommunityName, item.Title + " event is just added in your community group."
                    , item.Id ?? 0, item.TicketPrice??0, false, "", item.CreatedBy??0, item.CommunityId ?? 0, "", item.GroupId ?? 0);
                }
                logger.Info("Updating IsSentNotification flag");
                _plannerService.updateScheduleNotify(item.Id??0);
            }
        }
    }
}
