using AutoMapper;
using Circular.Core.Entity;
using Circular.Framework.Logger;
using Circular.Framework.Middleware.Emailer;
using Circular.Framework.Utility;
using Circular.Services.CommunityFeatures;
using Circular.Services.Email;
using Circular.Services.Finance;
using Exceeder_xe_Community.Filters;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace Exceeder_xe_Community.Controllers
{
    public class ThankyouController : Controller
    {
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IHelper _helper;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        IFinanceService financeService;
        IMailService _mailService;
        public ThankyouController(ILoggerManager logger, IConfiguration configuration,
             IFinanceService _financeService, IMapper mapper, IHttpContextAccessor httpContextAccessor, IHelper helper, IMailService mailService)
        {
            _mapper = mapper;
            _config = configuration;
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _httpContextAccessor = httpContextAccessor;
            financeService = _financeService;
            _logger = logger;
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        }


        [HttpGet]
        [Route("Thankyou/")]

        [ActionLog("Join", "{0} opened Thankyou.")]
        public async Task<IActionResult> Thankyou([FromQuery] string session_id)
        {
            try
            {
                StripeConfiguration.ApiKey = _config["SecretKey"];
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
                        string accesscode = financeService.UpdateMemberSubscriptionStatus(long.Parse(session.ClientReferenceId), "Complete", result);

                        ViewBag.Greeting = "Thank you";
                        ViewBag.Status = "We have processed your subscription.";
                        ViewBag.IsSucess = "true";
                        ViewBag.Message = "Welcome to the Green Club. You're now an esteemed member of the Exeeder Xe Community family, joining the ranks of remarkable e-bike resources and sustainable campaigns we proudly align with. Please download the mobile app and set up your profile.";
                        try
                        {
                            var firstName = TempData["FirstName"].ToString();
                            var lastName = TempData["LastName"].ToString();
                            var userName = TempData["Email"].ToString();
                            var customerID = TempData["CustomerId"].ToString();
                            var communityName = "Exeeder Xe";
                            MailRequest mailRequest = new MailRequest();
                            mailRequest.FromUserId = long.Parse(TempData["CustomerId"].ToString());
                            mailRequest.ReferenceId = long.Parse(TempData["CommunityId"].ToString());
                            MailSettings mailSettings = _mailService.EmailParameter(MailType.Welcome_comm_portal, ref mailRequest);
                            mailRequest.To = userName;
                            mailRequest.Body = mailRequest.Body.Replace("$FirstName", firstName);
                            mailRequest.Body = mailRequest.Body.Replace("$AccessCode", accesscode);
                            mailRequest.Body = mailRequest.Body.Replace("$communityName", communityName);
                            await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);
                        }
                        catch (Exception ex)
                        {

                       }


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

