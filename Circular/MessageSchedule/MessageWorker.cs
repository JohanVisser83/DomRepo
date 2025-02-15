using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Framework.Notifications;
using Circular.Services.Message;
using Circular.Services.Notifications;
using Microsoft.Extensions.Configuration;
using NLog;


namespace CircularScheduledJobs
{
    public class MessageWorker
    {
        private readonly IMessageService _Service;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;

        public MessageWorker(IMessageService Service, IConfiguration configuration, INotificationService notificationService)
        {
            _Service = Service ?? throw new ArgumentNullException(nameof(Service));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }


        public void SendScheduleMessages()
        {
            var logger = LogManager.GetLogger("database");
            logger.Info("Inside SendScheduleMessages");

            List<ScheduledMessage?> _messagesList = _Service.GetScheduledConversations().Result;
            logger.Info(_messagesList.Count().ToString());
        }


        public void SendBroadcastMessages()
        {
            var logger = LogManager.GetLogger("database");
            logger.Info("Inside SendBroadcastMessages");
            List<BroadcastMessage?> _messagesList = _Service.sendBroadcast().Result;
        }
    }
}



