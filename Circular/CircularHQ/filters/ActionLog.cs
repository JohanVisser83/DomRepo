using Circular.Data.Repositories.Audit;
using Circular.Services.Audit;
using Circular.Services.User;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace CircularHQ.filters
{

    public class AppSettings
    {
        private static AppSettings _appSettings;

        public string AppConnection { get; set; }

        public AppSettings(IConfiguration config)
        {
            this.AppConnection = config.GetValue<string>("CircularDbConnectionString");

            // Now set Current
            _appSettings = this;
        }

        public static AppSettings Current
        {
            get
            {
                if (_appSettings == null)
                {
                    _appSettings = GetCurrentSettings();
                }

                return _appSettings;
            }
        }

        public static AppSettings GetCurrentSettings()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();

            var settings = new AppSettings(configuration.GetSection("ConnectionStrings"));

            return settings;
        }
    }

    public class ActionLog : ActionFilterAttribute
    {
        string _pattern;
        string _action;
        private readonly IAuditService _auditService;


        public ActionLog(IAuditService auditService)
        {
            _auditService = auditService;
        }

        

        public ActionLog(string Action, string pattern)
        {
            _pattern = pattern;
            _action = Action;

            // Add services to the container.
            var settings = AppSettings.Current;
             var defaultConnectionString = settings.AppConnection;

 
            _auditService = new AuditService(new AuditRepository(defaultConnectionString));


        }

        
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.HttpContext != null && filterContext.HttpContext.User != null &&
                    filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var sid = Convert.ToInt64(filterContext.HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                       .Select(c => c.Value).SingleOrDefault());
                string message = string.Format(_pattern, filterContext.HttpContext.User.Identity.Name);
                if (sid > 0)
                {
                    Action act = () =>
                    {

                        _auditService.SaveAudit(sid, _action, message, "", "");
                    };
                    Task.Run(act);
                }

            }
         
        }
    }

     
}
