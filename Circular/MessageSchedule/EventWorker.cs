using Circular.Services.Account;
using Circular.Services.Planners;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircularScheduledJobs
{
    public class EventWorker
    {
        private readonly IPlannerService _Service;

        public EventWorker(IPlannerService Service)
        {
            _Service = Service ?? throw new ArgumentNullException(nameof(Service));
        }

        public void SendScheduleEventEmails()
        {
            var logger = LogManager.GetLogger("database");
            logger.Info("Inside SendScheduleEventEmails");

            _Service.SendBulkEmails().GetAwaiter().GetResult();
            logger.Info("Event emails sent");
        }

    }
}

