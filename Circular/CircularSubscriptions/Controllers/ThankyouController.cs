using AutoMapper;
using Circular.Core.Entity;
using Circular.Framework.Logger;
using Circular.Framework.Utility;
using Circular.Services.Finance;
using Circular.Services.User;
using Stripe.Checkout;
using CircularSubscriptions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Circular.Core.Mapper;
using Circular.Services.CommunityFeatures;
using AutoMapper.Execution;
using Circular.Framework.Middleware.Emailer;
using Circular.Services.Email;
using Circular.Services.CreateCommunity;

namespace CircularSubscriptions.Controllers
{
    public class ThankyouController : Controller
    {
        private readonly ILoggerManager _logger;
        private readonly ICommunityFeaturesServices _CommunityFeaturesService;
        private readonly ICreateCommunityServices _CreateCommunityServices;

        private readonly IMapper _mapper;
        private readonly IHelper _helper;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CommunityFeaturesModel communityFeatures;
        public CreateCommunityViewModel createCommunityViewModel = new CreateCommunityViewModel();

        IFinanceService financeService;
        IMailService _mailService;
        public ThankyouController(ILoggerManager logger, IConfiguration configuration,
            ICommunityFeaturesServices _communityFeatures, ICreateCommunityServices createCommunityServices, IFinanceService _financeService, IMapper mapper, IHttpContextAccessor httpContextAccessor, IHelper helper, IMailService mailService) 
        {
            _CommunityFeaturesService = _communityFeatures;
            _CreateCommunityServices = createCommunityServices;
            _mapper = mapper;
            _config = configuration;
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _httpContextAccessor = httpContextAccessor;
            financeService = _financeService;
            _logger = logger;
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        }
        [Route("Status/")]
        public async Task<IActionResult> Index([FromQuery] string session_id)
        {
            try
            {
                var SecretKey = _config["SecretKey"];
                StripeConfiguration.ApiKey = SecretKey;
                var sessionService = new SessionService();
                Session session = sessionService.Get(session_id);
                if (session != null)
                {
                    string result = session.ToString();
                    if (session.CustomerId != null)
                    {
                        var customerService = new Stripe.CustomerService();
                        Customer customer = customerService.Get(session.CustomerId);
                    }
                    if (session.PaymentStatus.ToString().ToLower() == "paid")
                    {
                        string accesscode = financeService.UpdateSubscriptionStatus(long.Parse(session.ClientReferenceId), "Complete", result);
                        var firstName = TempData["FirstName"].ToString();
                        var lastName = TempData["LastName"].ToString();
                        var userName = TempData["UserName"].ToString();
                        var customerID = TempData["CustomerId"].ToString();
                        var communityName = TempData["CommunityName"].ToString();
                        ViewBag.Greeting = "Thank you";
                        ViewBag.Status = "We have processed your subscription.";
                        ViewBag.IsSucess = "true";
                        ViewBag.Message = "Welcome to the club. It's a pleasure to have you with us. You've now officially become a cherished member of the Circular family, standing shoulder to shoulder with numerous outstanding global communities that we're proud to be associated with.";


                        MailRequest mailRequest = new MailRequest();
                        mailRequest.FromUserId = long.Parse(TempData["CustomerId"].ToString());
                        mailRequest.ReferenceId = long.Parse(TempData["TempCommunityId"].ToString());
                        MailSettings mailSettings = _mailService.EmailParameter(MailType.Welcome_comm_portal, ref mailRequest);
                        mailRequest.To = userName;
                        if(!string.IsNullOrEmpty(_config["CC"])) { mailRequest.CC = _config["CC"]; }
                        if (!string.IsNullOrEmpty(_config["BCC"])) { mailRequest.BCC = _config["BCC"]; }
                        mailRequest.Body = mailRequest.Body.Replace("$FirstName", firstName);
                        mailRequest.Body = mailRequest.Body.Replace("$AccessCode", accesscode);
                        mailRequest.Body = mailRequest.Body.Replace("$communityName", communityName);
                        await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);

                        createCommunityViewModel.CommunityLogo = await _CreateCommunityServices.GetCommunityLogo(customerID);

                    }
                    else
                    {
                        ViewBag.Greeting = "Oops...";
                        ViewBag.Status = "Something went wrong while fetching your payment status.";
                        ViewBag.IsSucess = "false";
                        ViewBag.Message = "Your transaction couldn’t be completed. While we are checking it at our end, you can double-check your payment information and try again. If the issue persists, contact our support team for assistance at support@circular.ooo. See you soon.";

                        return View("Thankyou");
                    }
                }
                else
                {
                    ViewBag.Greeting = "Oops...";
                    ViewBag.Status = "Something went wrong while fetching your payment details.";
                    ViewBag.IsSucess = "false";
                    ViewBag.Message = "Your transaction couldn’t be completed. While we are checking it at our end, you can double-check your payment information and try again. If the issue persists, contact our support team for assistance at support@circular.ooo. See you soon.";

                    return View("Thankyou");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Greeting = "Oops...";
                ViewBag.Status = "Something went wrong while processing your payment.";
                ViewBag.IsSucess = "false";
                ViewBag.Message = "Your transaction couldn’t be completed. While we are checking it at our end, you can double-check your payment information and try again. If the issue persists, contact our support team for assistance at support@circular.ooo. See you soon.";

                return View("Thankyou");
            }
            return View("Thankyou");
        }
    }
}
