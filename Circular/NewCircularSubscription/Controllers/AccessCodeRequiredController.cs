using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Framework.Utility;
using Circular.Services.Community;
using Circular.Services.User;
using Microsoft.AspNetCore.Mvc;
using NewCircularSubscription.Business;
using NewCircularSubscription.Models;
using Newtonsoft.Json;
using NuGet.Protocol;
using Stripe.Checkout;
using Stripe;
using Circular.Services.Finance;
using Circular.Services.CreateCommunity;

namespace NewCircularSubscription.Controllers
{
    public class AccessCodeRequiredController : Controller
    {
        private readonly ICommunityService _CommunityService;
        private readonly ICreateCommunityServices _CreateCommunityServices;
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        private readonly IHelper _helper;
        private readonly IConfiguration _config;
        private readonly IFinanceService _FinanceService;
        private IGeneric _generic;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CommunityDetailsModel communitydetails = new CommunityDetailsModel();
        public AccessCodeRequiredController(ICommunityService _communityService, ICreateCommunityServices createCommunityServices, ICustomerService customerService, IFinanceService financeService, IGeneric generic, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IHelper helper) 
        {
            _generic = generic ?? throw new ArgumentNullException(nameof(generic));
            _CommunityService = _communityService;
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _CreateCommunityServices = createCommunityServices;
            _mapper = mapper;
            _config = configuration;
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _httpContextAccessor = httpContextAccessor;
            _FinanceService = financeService ?? throw new ArgumentNullException(nameof(financeService));
        }


        [Route("RequestAccess/")]
        public async Task<IActionResult> Index()
        {
            if (TempData["CommunityId"] is null)
                return RedirectToAction("Discover", "Discover");

            ViewBag.LearnMoreCircularURl = _config["LearnMoreCircularURl"];
            ViewBag.CommunityPortalURl = _config["CommunityPortalURL"];
            ViewBag.TermsOfUse = _config["TermsOfUse"];
            ViewBag.PrivacyPolicy = _config["PrivacyPolicy"];

            ViewBag.LinkedIn = _config["LinkedIn"];
            ViewBag.WhatsApp = _config["WhatsApp"];
            ViewBag.Contactus = _config["Contactus"];

            var communityId = long.Parse(TempData["CommunityId"].ToString());
            TempData["CommunityId"] = TempData["CommunityId"].ToString();
            communitydetails.lstCommunitydetails = await _CommunityService.GetCommunities(communityId, "", 1, 10);
            communitydetails.lstCountryName = await _CreateCommunityServices.GetCountryName();
            return View("RequestAccess", communitydetails);
        }

        [Route("Join/")]
        public async Task<IActionResult> Join()
        {
            var CommunityId = TempData["CommunityId"].ToString();
            if (CommunityId is null)
                return RedirectToAction("Discover", "Discover");

            TempData["CommunityId"] = CommunityId.ToString();
            ViewBag.LearnMoreCircularURl = _config["LearnMoreCircularURl"];
            ViewBag.CommunityPortalURl = _config["CommunityPortalURL"];
            ViewBag.TermsOfUse = _config["TermsOfUse"];
            ViewBag.PrivacyPolicy = _config["PrivacyPolicy"];

            ViewBag.LinkedIn = _config["LinkedIn"];
            ViewBag.WhatsApp = _config["WhatsApp"];
            ViewBag.Contactus = _config["Contactus"];


            TempData["CommunityId"] = CommunityId.ToString();
            communitydetails.lstCountryName = await _CreateCommunityServices.GetCountryName();
            return View(communitydetails);
        }

        [Route("AccessCode/")]
        public async Task<IActionResult> AccessCode() 
        {
            var CommunityId = TempData["CommunityId"]?.ToString();
            if (CommunityId is null)
                return RedirectToAction("Discover", "Discover");

            TempData["CommunityId"] = CommunityId.ToString();

            ViewBag.LearnMoreCircularURl = _config["LearnMoreCircularURl"];
            ViewBag.CommunityPortalURl = _config["CommunityPortalURL"];
            ViewBag.TermsOfUse = _config["TermsOfUse"];
            ViewBag.PrivacyPolicy = _config["PrivacyPolicy"];

            ViewBag.LinkedIn = _config["LinkedIn"];
            ViewBag.WhatsApp = _config["WhatsApp"];
            ViewBag.Contactus = _config["Contactus"];


            var communityId = long.Parse(TempData["CommunityId"].ToString());
            TempData["CommunityId"] = CommunityId.ToString();
            communitydetails.lstCommunitydetails = await _CommunityService.GetCommunities(communityId, "", 1, 10);
            communitydetails.lstCountryName = await _CreateCommunityServices.GetCountryName();
            return View(communitydetails);
        }

        [Route("Member/")]
        public async Task<IActionResult> Member()
        {
            var CommunityId = TempData["CommunityId"].ToString();
            if (CommunityId is null)
                return RedirectToAction("Discover", "Discover");
            TempData["CommunityId"] = CommunityId.ToString();
            ViewBag.LearnMoreCircularURl = _config["LearnMoreCircularURl"];
            ViewBag.CommunityPortalURl = _config["CommunityPortalURL"];
            ViewBag.TermsOfUse = _config["TermsOfUse"];
            ViewBag.PrivacyPolicy = _config["PrivacyPolicy"];

            ViewBag.LinkedIn = _config["LinkedIn"];
            ViewBag.WhatsApp = _config["WhatsApp"];
            ViewBag.Contactus = _config["Contactus"];


            TempData["currency"] = _config["Currency"];
            TempData["CommunityId"] = CommunityId.ToString();
            communitydetails.lstCountryName = await _CreateCommunityServices.GetCountryName();
            return View(communitydetails);
        }


        [Route("Welcome/")]
        public async Task<IActionResult> Welcome([FromQuery] string session_id)
        {
            if(TempData["CommunityId"] is null)
                return RedirectToAction("Discover", "Discover");

            ViewBag.LearnMoreCircularURl = _config["LearnMoreCircularURl"];
            ViewBag.CommunityPortalURl = _config["CommunityPortalURL"];
            ViewBag.TermsOfUse = _config["TermsOfUse"];
            ViewBag.PrivacyPolicy = _config["PrivacyPolicy"];

            ViewBag.LinkedIn = _config["LinkedIn"];
            ViewBag.WhatsApp = _config["WhatsApp"];
            ViewBag.Contactus = _config["Contactus"];

            var communityId = long.Parse(TempData["CommunityId"]?.ToString());
            TempData["CommunityId"] = TempData["CommunityId"]?.ToString();
            communitydetails.lstCommunitydetails = await _CommunityService.GetCommunities(communityId, "", 1, 10);
            string strCustomerId = TempData["CustomerId"]?.ToString();
            TempData["CustomerId"] = strCustomerId;

            if (!string.IsNullOrEmpty(session_id))
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
                        //subs

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
                            newSubscription.CustomerId = long.Parse(strCustomerId);
                            newSubscription.TransactionId = long.Parse(session.ClientReferenceId);
                            newSubscription.CommunityMembershipId = communityId;
                            await _FinanceService.SaveSubscriptionDetails(newSubscription);
                        };


                        if (session.PaymentStatus.ToString().ToLower() == "paid")
                        {
                            string resultTranstatus = _FinanceService.UpdateCommunityMemberSubscriptionStatus(long.Parse(session.ClientReferenceId), "Complete", result);
                            ViewBag.IsSucess = "true";
                            ViewBag.Message = "You have joined the community successfully.";
                        }
                        else
                        {
                            ViewBag.IsSucess = "false";
                            ViewBag.Message = "Something went wrong while fetching your payment status.Your transaction couldn’t be completed. While we are checking it at our end, you can double-check your payment information and try again. ";
                        }
                    }
                    else
                    {
                        ViewBag.IsSucess = "false";
                        ViewBag.Message = "Something went wrong while fetching your payment status.Your transaction couldn’t be completed. While we are checking it at our end, you can double-check your payment information and try again. ";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.IsSucess = "false";
                    ViewBag.Message = "Something went wrong while processing your payment status.Your transaction couldn’t be completed. While we are checking it at our end, you can double-check your payment information and try again.";
                }

            }
            return View("Welcome",communitydetails);
        }



        [HttpPost]
        public async Task<IActionResult> SendOTPOnMobile(CommunityAccessDTO communityAccessCodeDTO)
        {
            try
            {
                
                TempData["FullName"] = communityAccessCodeDTO.Fullname;
                TempData["Mobile"] = communityAccessCodeDTO.CountryCode.Trim() + communityAccessCodeDTO.Mobile ;
                TempData["AccessCode"] = communityAccessCodeDTO.AccessCode;
                TempData["Email"] = communityAccessCodeDTO.Email;
                TempData["Countrycode"] = communityAccessCodeDTO.CountryCode.Trim();
                var Mobileno = communityAccessCodeDTO.CountryCode.Trim() + communityAccessCodeDTO.Mobile;
                if (!string.IsNullOrEmpty(communityAccessCodeDTO.Mobile) )
                {
                    APIResponse objResponse = await _generic.SendOTPOnMobile(Mobileno, true);
                    if (objResponse.StatusCode == 200)
                    {
                        TempData["CommunityId"] = TempData["CommunityId"].ToString();
                        TempData["MembershipType"] = TempData["MembershipType"].ToString();
                        TempData["AccessType"] = TempData["AccessType"].ToString();
                        TempData["Price"] = TempData["Price"].ToString();

                        return Json(objResponse);
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
        public async Task<IActionResult> VerifyOTP(string Otp)
        {
            try
            {
                var fullname = TempData["FullName"].ToString();
                TempData["FullName"] = TempData["FullName"]?.ToString();

                var CommunityId  = long.Parse(TempData["CommunityId"].ToString());
                TempData["CommunityId"] = TempData["CommunityId"].ToString();

                var Mobile = TempData["Mobile"]?.ToString();
                TempData["Mobile"] = TempData["Mobile"]?.ToString();

                string Price = TempData["Price"]?.ToString(); 
                TempData["Price"] = TempData["Price"]?.ToString();

                string Email = TempData["Email"]?.ToString();
                TempData["Email"] = TempData["Email"]?.ToString();

                var AccessCode = TempData["AccessCode"]?.ToString();
                TempData["AccessCode"] = TempData["AccessCode"];

                var MembershipType = TempData["MembershipType"]?.ToString();
                TempData["MembershipType"] = TempData["MembershipType"];

                var AccessType = TempData["AccessType"]?.ToString();
                TempData["AccessType"] = TempData["AccessType"];

                var countryCode = TempData["Countrycode"]?.ToString();
                TempData["Countrycode"] = TempData["Countrycode"]?.ToString();

                var Currencycode = TempData["Currencycode"]?.ToString();
                TempData["Currencycode"] = TempData["Currencycode"]?.ToString();

                var Currencytoken = TempData["CurrencyToken"]?.ToString();
                TempData["CurrencyToken"] = Currencytoken;

                CommunityAccessDTO communityAccessCodeDTO = new CommunityAccessDTO();
                communityAccessCodeDTO.Fullname = fullname;
                communityAccessCodeDTO.CommunityId = CommunityId;
                communityAccessCodeDTO.Price = Price;
                communityAccessCodeDTO.Email = Email;



                var resp = await _generic.GetTokenByOtpAsync(Mobile, Otp, true, countryCode);
                string data = resp.ToJson();
                APIResponse objResponse = JsonConvert.DeserializeObject<APIResponse>(data);
                if (objResponse.StatusCode == 2000)
                {
                    CustomerResponse customerResponse = JsonConvert.DeserializeObject<CustomerResponse>(objResponse.Data.ToJson());
                    communityAccessCodeDTO.CustomerId = customerResponse.Customer.Id;
                    var userDetails = await _CommunityService.UpdateuserDetails(communityAccessCodeDTO);

                    if (!string.IsNullOrEmpty(MembershipType) && MembershipType.ToLower() == "Free".ToLower())
                    {
                        if (!string.IsNullOrEmpty(AccessType) && AccessType.ToString().ToLower() == "Request access".ToLower())
                        {
                            communityAccessCodeDTO.StatusId = 102;
                            CommunityAccessRequests communityAccessRequests = _mapper.Map<CommunityAccessRequests>(communityAccessCodeDTO);
                            var result = await _CommunityService.SaveCommunityAccessRequest(communityAccessRequests);
                            if (result > 0)
                                return Json(new { success = true, redirectUrl = Url.Action("Welcome", "AccessCodeRequired"), data = result });
                            else if (result == -1)
                                return Json(new { success = false, message = "You are either an existing member or your earlier request is pending." });
                            else
                                return Json(new { success = false, redirectUrl = Url.Action("Index", "AccessCodeRequired"), data = result });
                        }
                        else if (!string.IsNullOrEmpty(AccessType) && (AccessType.ToString().ToLower() == "join".ToLower()
                            || AccessType.ToString().ToLower() == "Access code".ToLower()))
                        {
                            int accesscode = 1;
                            if (AccessType.ToString().ToLower() == "Access code".ToLower())
                                accesscode = _CommunityService.CheckValidAccessCode(AccessCode, CommunityId);
                            if (accesscode > 0)
                            {
                                CustomerCommunity customerCommunity = _mapper.Map<CustomerCommunity>(communityAccessCodeDTO);
                                var result = await _CommunityService.SaveNewCustomerCommunity(customerCommunity);
                                if (result > 0)
                                    return Json(new { success = true, redirectUrl = Url.Action("Welcome", "AccessCodeRequired"), data = result });
                                else
                                    return Json(new { success = false, redirectUrl = Url.Action("JoinCommunity", "AccessCodeRequired"), data = result });
                            }
                            else
                                return Json(new { success = false, message = "Access code you have entered is incorrect. Please try again." });
                        }
                        else
                            return RedirectToAction("Discover", "Discover");
                    }
                    else
                    {
                        //Paid
                        String url = await Payment(customerResponse.Customer.Id, Mobile, Currencycode,
                            customerResponse.Customer.Id, decimal.Parse(Price), CommunityId, fullname, Email, Currencytoken);
                        if(url.Equals(""))
                            return Json(new { success = false, message = "Your default currency in stripe is other than current currency you are trying to use." });
                        else
                            return Json(new { success = true, redirectUrl = url, data = "" });

                    }
                }
                else
                    return Json(new { success = false, message = "Invalid OTP." });
            }
            catch(Exception ex)
            {
                return RedirectToAction("Discover", "Discover");
            }
        }
        public async Task<string> Payment(long customerId , string Mobile, string Currencycode, long? ReferenceId, 
            decimal Price, long CommunityId, string CustomerName, string Email, string Currencytoken)
        {
            try
            {
                TransactionRequest transactions = new TransactionRequest();
                transactions.TransactionFrom = customerId;
                transactions.TransactionTo = transactions.TransactionFrom;
                TempData["CustomerId"] = customerId.ToString();

                transactions.TransactionTypeId = (long)TransactionTypeEnum.MemberSubscription;
                transactions.TransactionStatusId = (long)TransactionStatusEnum.Pending;
                transactions.MobileNumber = Mobile;
                transactions.Currency = Currencycode;
                transactions.RequestedTransactionId = 0;
                transactions.ReferenceType = "MemberSubscription";
                transactions.ReferenceId = ReferenceId;
                transactions.PaymentDesc = Mobile;
                transactions.Amount = Price;
                transactions.CommunityId = CommunityId;
                transactions.ServiceCharges = 0;
                if (_config["IncludeServiceCharge"] == "TRUE")
                {
                    transactions.ServiceCharges = ((transactions.Amount * decimal.Parse(_config["ServiceCharge"] ?? "0")) / 100);
                    if (transactions.ServiceCharges > 0)
                        transactions.ServiceCharges = transactions.ServiceCharges + decimal.Parse(_config["FixedCharge"] ?? "0");
                }
                long result = await _FinanceService.SubscriptionPayment(transactions);

                string CustomerId = "";
                CustomerId = _FinanceService.CheckSubscriptionCustomer(transactions.TransactionFrom).Result;
                if (string.IsNullOrEmpty(CustomerId))
                {
                    var options = new CustomerCreateOptions
                    {
                        Name = CustomerName,
                        Email = Email,
                        Phone = transactions.MobileNumber,
                        Description = transactions.TransactionFrom.ToString(),
                    };
                    var service = new Stripe.CustomerService();
                    Customer stripeCustomer = service.Create(options);
                    CustomerId = stripeCustomer.Id;
                    _FinanceService.SaveSubscriptionCustomer(transactions.TransactionFrom, CustomerId);
                }

                var sessionoptions = new SessionCreateOptions
                {
                    Mode = "subscription",
                    SuccessUrl = _config["MemberSuccessUrl"],
                    CancelUrl = _config["MemberCancelUrl"],
                    Customer = CustomerId,
                    ClientReferenceId = result.ToString(),
                    Currency = Currencytoken,
                   
                };
                sessionoptions.LineItems = new List<SessionLineItemOptions>();

                SessionLineItemOptions sessionLineItemOptionsMemberCharge = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmountDecimal = (transactions.Amount + transactions.ServiceCharges) * 100,
                        Currency = Currencytoken,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = transactions.ReferenceType,
                        },
                         Recurring =  new SessionLineItemPriceDataRecurringOptions() { Interval="month", IntervalCount = 1 },
                    },
                    Quantity = 1,
                };
                sessionoptions.LineItems.Add(sessionLineItemOptionsMemberCharge);

                var sessionservice = new SessionService();
                Session session = sessionservice.Create(sessionoptions);

                return session.Url;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(_config["CombineCurrencyError"]))
                    return "Different_Currency";
                else
                    return "";
            }
        }


        public async Task<IActionResult> ResendOTP(bool? loginflow = null)
        {
            
            string Mobile = TempData["Mobile"].ToString();
            TempData["Mobile"] = Mobile;
            
            //if (TempData["ReturnUrl"] != null)
            //{
            //    string returnUrl = TempData["ReturnUrl"].ToString();
            //    TempData["ReturnUrl"] = returnUrl;
            //}
            APIResponse objResponse = await _generic.SendOTP(Mobile, loginflow ?? true);
            if (objResponse.StatusCode == 200)
                return Json(new { success = true, message = "OTP Sent Successfully" });
            else
                return Json(new { success = false, message = "" });
        }
    }
}
