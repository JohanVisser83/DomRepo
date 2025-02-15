using Circular.Services.Account;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NLog;

namespace CircularScheduledJobs
{
    public class AccountWorker
    {
        private readonly IAccountServices _Service;

        public AccountWorker(IAccountServices Service)
        {
            _Service = Service ?? throw new ArgumentNullException(nameof(Service));
        }

        public void SendScheduleAccountEmails()
        {
            var logger = LogManager.GetLogger("database");
            logger.Info("loggerTest emailaccount");
            logger.Info("Inside SendScheduleAccountEmails");

            _Service.SendBulkEmails().GetAwaiter().GetResult();
            logger.Info("Account emails sent");
        }

    }
}
