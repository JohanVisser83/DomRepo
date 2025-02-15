using Circular.Core.Entity;
using Circular.Framework.Middleware.Emailer;
using Circular.Services.User;
namespace Circular
{
    public class Common : ICommon
    {
        private ICustomerService _customerService;
        public string UserId { get; set; }
        private readonly IConfiguration _configuration;

        public Common(IHttpContextAccessor _httpContextAccessor, ICustomerService customerService, IConfiguration configuration)
        {
            var httpContext = _httpContextAccessor?.HttpContext;
            UserId = httpContext?.Items["UserId"]?.ToString() ?? "";
            _customerService = customerService;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        }
        public Customers CurrentUser()
        {
            if (string.IsNullOrEmpty(_configuration["Environment"]))
                throw new ArgumentNullException("configuration : Environment is not defined in App Setting");

            Customers cust = _customerService.getcustomerByUserId(new Guid(UserId), true);
            cust.Environment = Convert.ToString(_configuration["Environment"]);
            return cust;
        }
    }
}
