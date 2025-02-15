using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Core.Mapper;
using Circular.Data.Repositories.User;
using Circular.Framework.Emailer;
using Circular.Framework.Logger;
using Circular.Framework.Middleware.Emailer;
using Circular.Framework.Utility;
using Circular.Services.Community;
using Circular.Services.CommunityFeatures;
using Circular.Services.CreateCommunity;
using Circular.Services.Email;
using Circular.Services.Finance;
using Microsoft.AspNetCore.Mvc;
using NewCircularSubscription.Business;
using NewCircularSubscription.Models;
using Newtonsoft.Json;
using NuGet.Protocol;
using Stripe;
using Stripe.Checkout;



namespace NewCircularSubscription.Controllers
{
    public class CommunitySignUpController : Controller
    {
        private readonly ICommunityService _CommunityService;
        private readonly ICommunityFeaturesServices _CommunityFeaturesService;
        private readonly ICreateCommunityServices _CreateCommunityServices;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private IGeneric _generic;
        public string UploadFolderPath { get; set; }
        private readonly IHelper _helper;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        IFinanceService financeService;
        public CommunitySignUpModel communitySignUp = new CommunitySignUpModel();
        IMailService _mailService;
        public CommunitySignUpController(ICommunityFeaturesServices _communityFeatures, ICreateCommunityServices createCommunityServices,ILoggerManager logger, IConfiguration configuration,
             IFinanceService _financeService, IMapper mapper, IHttpContextAccessor httpContextAccessor, IHelper helper, ICommunityService _communityService, IMailService mailService, IGeneric generic) 
        {
            _CommunityService = _communityService;
            _CommunityFeaturesService = _communityFeatures;
            _CreateCommunityServices = createCommunityServices;
            _mapper = mapper;
            _config = configuration;
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _httpContextAccessor = httpContextAccessor;
            financeService = _financeService;
            _logger = logger;
            UploadFolderPath = "/" + _config["FileUpload:FileUploadPath"].ToString();
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _generic = generic ?? throw new ArgumentNullException(nameof(generic));
        }


       // [HttpGet]
        [Route("CommunitySignUp/")]
        public async Task<IActionResult> CommunitySignUp([FromQuery] string session_id, [FromQuery] string aUfmD = "0")
        {
            try
            {
                string strPlan = TempData["Plan"]?.ToString();
                string strCustomerName = TempData["Name"]?.ToString();
                string strEmail = TempData["UserName"]?.ToString();
                string CustomerId = TempData["CustomerId"]?.ToString();
                string AdminTransactionId = TempData["AdminTransactionId"]?.ToString();

                TempData["UserName"] = strEmail;
                TempData["CustomerId"] = CustomerId;
                TempData["Plan"] = strPlan;
                TempData["Name"] = strCustomerName;
                TempData["AdminTransactionId"] = AdminTransactionId;


                ViewBag.LearnMoreCircularURl = _config["LearnMoreCircularURl"];
                ViewBag.CommunityPortalURl = _config["CommunityPortalURL"];
                ViewBag.TermsOfUse = _config["TermsOfUse"];
                ViewBag.PrivacyPolicy = _config["PrivacyPolicy"];

                ViewBag.LinkedIn = _config["LinkedIn"];
                ViewBag.WhatsApp = _config["WhatsApp"];
                ViewBag.Contactus = _config["Contactus"];

                communitySignUp.SubscriptionTypes = await _CommunityFeaturesService.GetSubscriptionType();
                communitySignUp.CommunityAccessTypes = await _CommunityFeaturesService.GetCommunityAccessTypes();
                communitySignUp.lstCountryName = await _CreateCommunityServices.GetCountryName();
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

                    string subscriptionId = session.SubscriptionId;
                    if (!string.IsNullOrEmpty(subscriptionId))
                    {

                        var subscriptionService = new SubscriptionService();
                        Stripe.Subscription subscription = subscriptionService.Get(subscriptionId);

                        var newSubscription = new SubscriptionDetails
                        {
                            StripeSubscriptionId = subscription.Id,
                            SubscriptionStatus = subscription.Status,
                            CurrentPeriodStart = subscription.CurrentPeriodStart,
                            CurrentPeriodEnd = subscription.CurrentPeriodEnd,
                        };
                        newSubscription.CustomerId = long.Parse(CustomerId);
                        newSubscription.TransactionId = long.Parse(session.ClientReferenceId);
                        if (aUfmD.ToLower() == "xByEnTbYsOs".ToLower())
                        {
                            newSubscription.TransactionId = long.Parse(AdminTransactionId);
                        }
                        await financeService.SaveSubscriptionDetails(newSubscription);
                    };


                    if (session.PaymentStatus.ToString().ToLower() == "paid")
                    {
                        string resultTranstatus = "";

                        if (aUfmD.ToLower() == "xByEnTbYsOs".ToLower())
                        {
                             resultTranstatus = financeService.UpdateCommunityTransactionStatus(long.Parse(AdminTransactionId), "Complete", result);
                            TempData["TransactionId"] = AdminTransactionId.ToString();

                        }
                        else
                        {
                             resultTranstatus = financeService.UpdateCommunityTransactionStatus(long.Parse(session.ClientReferenceId), "Complete", result);
                             TempData["TransactionId"] = session.ClientReferenceId.ToString();

                        }



                        TempData["Plan"] = strPlan;
                        TempData["Name"] = strCustomerName;
                        TempData["UserName"] = strEmail;
                        TempData["CustomerId"] = CustomerId;

                        ViewBag.IsSucess = "true";
                        ViewBag.Message = "I'm here to make setting up your Circular community a breeze. From setting your public profile to optimizing your platform for success, I'll guide you every step of the way. Let's turn your business dreams into reality with ease and efficiency!";
                        return View(communitySignUp);
                    }
                    else
                    {
                        ViewBag.Greeting = "Oops...";
                        ViewBag.Status = "";
                        ViewBag.IsSucess = "false";
                        ViewBag.Message = "Something went wrong while fetching your payment status.Your transaction couldn’t be completed. While we are checking it at our end, you can double-check your payment information and try again. " +
                            "If the issue persists, contact our support team for assistance at support@circular.ooo. See you soon.";

                        return View(communitySignUp);
                    }
                }
                else
                {
                    ViewBag.Greeting = "Oops...";
                    ViewBag.Status = "";
                    ViewBag.IsSucess = "false";
                    ViewBag.Message = "Something went wrong while fetching your payment status.Your transaction couldn’t be completed. While we are checking it at our end, you can double-check your payment information and try again. " +
                        "If the issue persists, contact our support team for assistance at support@circular.ooo. See you soon.";


                   return View(communitySignUp);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Greeting = "Oops...";
                ViewBag.Status = "";
                ViewBag.IsSucess = "false";
                ViewBag.Message = "Something went wrong while processing your payment status.Your transaction couldn’t be completed. While we are checking it at our end, you can double-check your payment information and try again." +
                    "If the issue persists, contact our support team for assistance at support@circular.ooo. See you soon.";

                return View(communitySignUp);
            }
            
        }


        public async Task<IActionResult> SaveCommunityDetails(CommunitySetUpInfoDTO communitySetUpInfoDTO)
        {
            if (communitySetUpInfoDTO.OrgLogo != null)
                communitySetUpInfoDTO.CommunityLogo = _helper.SaveFile(communitySetUpInfoDTO.OrgLogo, UploadFolderPath, this.Request);

            if (communitySetUpInfoDTO.CommunityDashboardBanner != null)
                communitySetUpInfoDTO.DashboardBanner = _helper.SaveFile(communitySetUpInfoDTO.CommunityDashboardBanner, UploadFolderPath, this.Request);
            communitySetUpInfoDTO.PlanType = TempData["PlanType"]?.ToString();
            communitySetUpInfoDTO.Plan = TempData["Plan"]?.ToString();

            TempData["PlanType"] = TempData["PlanType"]?.ToString();
            TempData["Plan"] = TempData["Plan"]?.ToString();

            string strName = TempData["Name"]?.ToString();
            TempData["Name"] = strName;

            string strEmail = TempData["UserName"]?.ToString();
            TempData["UserName"] = strEmail;
            string CustomerId = TempData["CustomerId"]?.ToString();
            TempData["CustomerId"] = CustomerId;

            if (string.IsNullOrEmpty(strName))
                strName = "";
            if (string.IsNullOrEmpty(communitySetUpInfoDTO.CurrencyToken))
                communitySetUpInfoDTO.CurrencyToken = "USD";
            if(string.IsNullOrEmpty(strEmail))
                strEmail = "";
            if(string.IsNullOrEmpty(CustomerId))
                CustomerId = "";
            if (string.IsNullOrEmpty(communitySetUpInfoDTO.Plan))
                communitySetUpInfoDTO.Plan = "";


            long Transactionid = long.Parse(TempData["TransactionId"].ToString());
            string result =  _CommunityFeaturesService.PostCommunityDetails(communitySetUpInfoDTO.CommunityLogo,
            communitySetUpInfoDTO.DashboardBanner, communitySetUpInfoDTO.CommunityName, communitySetUpInfoDTO.MembershipType,
            communitySetUpInfoDTO.MembershipAmount, communitySetUpInfoDTO.AccessType, communitySetUpInfoDTO.About,
            communitySetUpInfoDTO.Website, communitySetUpInfoDTO.physicalAddress,
            communitySetUpInfoDTO.Plan, 0, 0, Transactionid, _helper.GenerateRandomCustomerId(6),communitySetUpInfoDTO.Country,communitySetUpInfoDTO.CountryId,communitySetUpInfoDTO.CurrencyCode,communitySetUpInfoDTO.CurrencyToken, strName);
            if (!string.IsNullOrEmpty(result))
            {
                string CommunityURL = result.Split('|')[0];
                string accCode = result.Split('|')[1];
                long comId = long.Parse(result.Split('|')[2]);

                //New Changes

                TempData["CommunityId"] = comId.ToString();
                //string CommunityURL = await _CommunityService.GetCommunityURL(result);
                if (!string.IsNullOrEmpty(CommunityURL)) 
                    TempData["CommunityURL"] = CommunityURL;
                await SendCommunitySetupEmail(strName, communitySetUpInfoDTO.CommunityName, 
                    CustomerId, strEmail, comId, accCode);
                return Json(new { success = true, message = "", data = result });
            }
            else
                return Json(new { success = false, message = "", data = result });
        }



        public async Task<bool> SendCommunitySetupEmail(string strName, string communityname, 
            string CustomerId, string strEmail, long communityid, string accesscode)
        {
                try
                {
                    MailRequest mailRequest = new MailRequest();
                    mailRequest.FromUserId = long.Parse(CustomerId);
                    mailRequest.ReferenceId = communityid;
                    MailSettings mailSettings = _mailService.EmailParameter(MailType.Welcome_Subscription_Commportal, ref mailRequest);
                    mailRequest.To = strEmail;
                    if (!string.IsNullOrEmpty(_config["CC"])) { mailRequest.CC = _config["CC"]; }
                    if (!string.IsNullOrEmpty(_config["BCC"])) { mailRequest.BCC = _config["BCC"]; }
                    mailRequest.Body = mailRequest.Body.Replace("$FirstName", strName);
                    mailRequest.Body = mailRequest.Body.Replace("$AccessCode", accesscode);
                    mailRequest.Body = mailRequest.Body.Replace("$communityName", communityname);
                    return await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);
                }
                catch (Exception)
                {
                    return false;
                }
        }



        [HttpPost]
        public async Task<IActionResult> SendOTPOnMobile(CommunityUserDTO communityUserDTO)
        {
            try
            {
                TempData["Mobile"] = communityUserDTO.CountryCode.Trim() + communityUserDTO.Mobile;
                TempData["Countrycode"] = communityUserDTO.CountryCode.Trim();
                var Mobileno = communityUserDTO.CountryCode.Trim() + communityUserDTO.Mobile;
                if (!string.IsNullOrEmpty(communityUserDTO.Mobile))
                {
                    APIResponse objResponse = await _generic.SendOTPOnMobile(Mobileno, true);
                    if (objResponse.StatusCode == 200)
                    {
                        TempData["Mobile"] = TempData["Mobile"]?.ToString();
                        TempData["Name"] = TempData["Name"]?.ToString();
                        TempData["CommunityId"] = TempData["CommunityId"]?.ToString();
                        TempData["UserName"] = TempData["UserName"];
                        TempData["CommunityURL"] = TempData["CommunityURL"] ;
                        return Json(new { success = true, message = "", data = objResponse });
                    }
                }
                return Json(new { success = false, message = "", data = 0 });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, data = 0 });
            }

        }



        [HttpPost]
        public async Task<IActionResult> VerifyOTP(string Otp, string Mobile, string countryCode)
        {
            try
            {
                TempData["Name"] = TempData["Name"]?.ToString();
                TempData["CommunityId"] = TempData["CommunityId"]?.ToString();
                TempData["UserName"] = TempData["UserName"];
                TempData["CommunityURL"] = TempData["CommunityURL"];
                string strName = TempData["Name"]?.ToString();
                TempData["Name"] = strName;
                string strEmail = TempData["UserName"]?.ToString();
                TempData["UserName"] = strEmail;
                if (!long.TryParse(TempData["CommunityId"]?.ToString(), out long communityId))
                {
                    return Json(new { success = false, message = "Something Wents wrong." });
                }

                if (string.IsNullOrEmpty(strName) || string.IsNullOrEmpty(strEmail))
                {
                    return Json(new { success = false, message = "Something Wents wrong." });
                }

                TempData["PlanType"] = "Test";
                var Mobileno = countryCode.Trim() + Mobile;
                TempData["Mobileno"] = Mobileno;

                var resp = await _generic.GetTokenByOtpAsync(Mobileno, Otp, true, countryCode);
                string data = resp.ToJson();

                APIResponse objResponse = JsonConvert.DeserializeObject<APIResponse>(data);
                if (objResponse.StatusCode == 2000)
                {
                    CustomerResponse customerResponse = JsonConvert.DeserializeObject<CustomerResponse>(objResponse.Data.ToJson());
                        string result = await _CommunityFeaturesService.CreateCommunityAppUser(customerResponse.Customer.Id, communityId, strName, strEmail);
                        if (!string.IsNullOrEmpty(result))
                        {
                          return Json(new { success = true, data = objResponse });
                        }
                        return Json(new { success = false, message = "", data = "" });
                   
                }
                else
                    return Json(new { success = false, message = "Invalid OTP." });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Discover", "Discover");
            }
        }




        //public async Task<IActionResult> ResendOTP(bool? loginflow = null)
        //{

        //    string Mobile = TempData["Mobileno"]?.ToString();
        //    TempData["Mobileno"] = Mobile;


        //    APIResponse objResponse = await _generic.SendOTP(Mobile, loginflow ?? true);
        //    if (objResponse.StatusCode == 200)
        //        return Json(new { success = true, message = "OTP Sent Successfully" });
        //    else
        //        return Json(new { success = false, message = "" });
        //}

    }

   
}
