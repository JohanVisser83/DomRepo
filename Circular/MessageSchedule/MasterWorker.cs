using Circular.Services.Master;
using Microsoft.Extensions.Configuration;
using NLog;

namespace CircularScheduledJobs
{
    public class MasterWorker
    {
        private readonly IMasterService _Service;
        public MasterWorker(IMasterService Service)
        {
            _Service = Service ?? throw new ArgumentNullException(nameof(Service));
          
        }
        public void ClearUsedOTP()
        {
            var logger = LogManager.GetLogger("database");
            logger.Info("loggerTest OTP");
            _Service.DeleteOTP().GetAwaiter().GetResult();
        }
        public void UpdateDevices()
        {
            var logger = LogManager.GetLogger("database");
            logger.Info("loggerTest Device");
            _Service.UniqueDevices().GetAwaiter().GetResult();
        }

    }
}
