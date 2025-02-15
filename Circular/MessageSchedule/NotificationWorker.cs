using Circular.Data.Repositories.User;
using Circular.Services.User;
using Microsoft.Extensions.Configuration;

namespace ScheduledJobs
{
    public class NotificationWorker
    {
        private readonly ICustomerService _customerService;
        private readonly IConfiguration _configuration;

        public NotificationWorker(ICustomerService customerService, IConfiguration configuration)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        }

        public void PrintNumber()
        {
            Console.WriteLine($"My wonderful number is " + _configuration["Number:DefaultNumber"]);
        }
    }
}
