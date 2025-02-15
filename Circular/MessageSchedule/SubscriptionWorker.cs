using Autofac.Core;
using Circular.Services.Finance;
using Circular.Services.Notifications;
using Microsoft.Extensions.Configuration;
using NLog;


namespace CircularScheduledJobs
{
    public class SubscriptionWorker
    {
        private readonly IFinanceService _Service;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;

        public SubscriptionWorker(IConfiguration configuration, INotificationService notificationService, IFinanceService Service) 
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _Service = Service ?? throw new ArgumentNullException(nameof(Service));
        }


        public void CheckExpiredSubscriptionAndNotify()
        {
            var logger = LogManager.GetLogger("database");
            logger.Info("Inside SubscriptionEventNotify");

            _Service.GetCommunityExpiredSubscriptionDetails().GetAwaiter().GetResult();
            logger.Info("Email Sent");
        }
    }
}
