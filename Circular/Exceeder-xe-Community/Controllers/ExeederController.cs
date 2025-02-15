using AutoMapper;
using Circular.Framework.Utility;
using Circular.Services.User;
using Exceeder_xe_Community.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Exceeder_xe_Community.Controllers
{
    public class ExeederController: Controller
    {

        public ExeederController(IMapper mapper, IServiceProvider provider
            , IConfiguration configuration, ICustomerService customerService, IHelper helper) 
        {
            //_generic = generic ?? throw new ArgumentNullException(nameof(generic));
            //_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            //_provider = provider ?? throw new ArgumentNullException(nameof(provider));
            //_helper = helper ?? throw new ArgumentNullException(nameof(helper));
            //if (string.IsNullOrEmpty(configuration["OIDCUrl"]))
            //    throw new ArgumentNullException("configuration : OIDCUrl is not defined in App Setting");
            //OIDCUrl = Convert.ToString(configuration["OIDCUrl"]);
            //_customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            //_configuration = configuration ?? throw new ArgumentNullException(nameof(_configuration));

        }



        [HttpGet]

        [ActionLog("Exeeder", "{0} opened Exeeder.")]
        public IActionResult Exeeder(string? returnUrl = null)
        {
            return View();
        }


    }
}
