using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Framework.Utility;
using Circular.Services.Exeeder;
using Circular.Services.Finance;
using Circular.Services.User;
using Exceeder_xe_Community.Business;
using Exceeder_xe_Community.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using Stripe;
using Stripe.Checkout;
//using Exceeder_xe_Community.Filter

namespace Exceeder_xe_Community.Controllers
{
    public class JoinController: Controller
    {
        private readonly IHelper _helper;
        private readonly IMapper _mapper;
        private IServiceProvider _provider;
        private readonly IExeederService _ExeederServices;
        private ICustomerService _customerService;
        private readonly IConfiguration _config;
        private string OIDCUrl;
        Customers customer;
        private IGeneric _generic;
        IFinanceService financeService;
        public string UploadFolderPath { get; set; }
        public JoinController(IExeederService exeederServices, IMapper mapper, IServiceProvider provider
            , IConfiguration configuration, ICustomerService customerService, IHelper helper, IGeneric generic, IFinanceService _financeService)
        {
            _generic = generic ?? throw new ArgumentNullException(nameof(generic));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            if (string.IsNullOrEmpty(configuration["OIDCUrl"]))
                throw new ArgumentNullException("configuration : OIDCUrl is not defined in App Setting");
            OIDCUrl = Convert.ToString(configuration["OIDCUrl"]);
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            UploadFolderPath = "/" + _config["FileUpload:FileUploadPath"].ToString();
            _ExeederServices = exeederServices;
            financeService = _financeService;
        }


        [Route("Join/")]

        [ActionLog("Join", "{0} opened join.")]
        public IActionResult Join(string? returnUrl = null)
        {
            return View();
        }



        [HttpPost]

        [ActionLog("Join", "{0} Join Member.")]
        public async Task<ActionResult<int>> JoinMember(CommunityTemporaryMemberDTO communityTemporaryMemberDTO)
            {
            try
            {
                if (communityTemporaryMemberDTO.MediaFile != null)
                    communityTemporaryMemberDTO.Profile = _helper.SaveFile(communityTemporaryMemberDTO.MediaFile, UploadFolderPath, this.Request);

                TempData["Mobile"] = communityTemporaryMemberDTO.Mobile;
                TempData["FirstName"] = communityTemporaryMemberDTO.FirstName.ToString();
                TempData["LastName"] = communityTemporaryMemberDTO.LastName.ToString();
                TempData["Email"] = communityTemporaryMemberDTO.Email.ToString();
                TempData["Profile"] = communityTemporaryMemberDTO.Profile;
                TempData["AffiliateCode"] = communityTemporaryMemberDTO.AffiliateCode.ToString();

                string Mobile = communityTemporaryMemberDTO.Mobile;
                Mobile = Mobile.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "");
                if (Mobile.StartsWith("0") || Mobile.StartsWith("1"))
                {
                    Mobile = Mobile.Substring(1);
                    Mobile = "+1" + Mobile.Trim();
                }
                if (!Mobile.StartsWith("+"))
                {
                    Mobile = "+1" + Mobile.Trim();
                }
                if (Mobile.StartsWith("+1"))
                { 
                    communityTemporaryMemberDTO.Mobile = Mobile;
                    APIResponse objResponse = await _generic.SendOTPs(communityTemporaryMemberDTO.Mobile, true);
                    if (objResponse.StatusCode == 200)
                    {
                        return Json(objResponse);
                    }
                }
                return View("Join");
            }
            catch (Exception ex)
            {
                return View("Join");
            }
        }


        [HttpPost]

        [ActionLog("Join", "{0} verify otp.")]
        public async Task<IActionResult> VerifyOTP(CommunityTemporaryMemberDTO communityTemporaryMemberDTO)
        {
                    communityTemporaryMemberDTO.Mobile = TempData["Mobile"].ToString();
                    communityTemporaryMemberDTO.FirstName = TempData["FirstName"].ToString();
                    communityTemporaryMemberDTO.LastName = TempData["LastName"].ToString();
                    communityTemporaryMemberDTO.Email = TempData["Email"].ToString();
                    communityTemporaryMemberDTO.Profile = TempData["Profile"].ToString();
                    communityTemporaryMemberDTO.AffiliateCode = TempData["AffiliateCode"].ToString();


            string Mobile = communityTemporaryMemberDTO.Mobile;
                    Mobile = Mobile.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "");
                    if (Mobile.StartsWith("0") || Mobile.StartsWith("1"))
                    {
                        Mobile = Mobile.Substring(1);
                        Mobile = "+1" + Mobile.Trim();
                    }
                    if (!Mobile.StartsWith("+"))
                    {
                        Mobile = "+1" + Mobile.Trim();
                    }
                    communityTemporaryMemberDTO.Mobile = Mobile;
                    var resp = await _generic.GetTokenByOtpAsync(communityTemporaryMemberDTO.Mobile, communityTemporaryMemberDTO.Otp, true, "+1");
                    string data = resp.ToJson();
                    APIResponse objResponse = JsonConvert.DeserializeObject<APIResponse>(data);
                    if (objResponse.StatusCode == 2000)
                    {
                        
                        
                        CustomerResponse customerResponse = JsonConvert.DeserializeObject<CustomerResponse>(objResponse.Data.ToJson());
                        communityTemporaryMemberDTO.CustomerId = customerResponse.Customer.Id;
                        communityTemporaryMemberDTO.CommunityId = Convert.ToInt64(_config["CommunityId"]);
                        CommunityTemporaryMember communityTemporaryMember = _mapper.Map<CommunityTemporaryMember>(communityTemporaryMemberDTO);
                        
                        
                        var result = await _ExeederServices.SaveCommunityTempDetails(communityTemporaryMember);
                    if (result > 0)
                    {
                        TempData["CustomerId"] = customerResponse.Customer.Id.ToString();
                        TempData["AuthCode"] = customerResponse.AccessToken.ToString();
                        TempData["Mobile"] = communityTemporaryMemberDTO.Mobile.ToString();
                        TempData["CommunityId"] = communityTemporaryMemberDTO.CommunityId.ToString();
                        TempData["FirstName"] = communityTemporaryMemberDTO.FirstName.ToString();
                        TempData["LastName"] = communityTemporaryMemberDTO.LastName.ToString();
                        TempData["Email"] = communityTemporaryMemberDTO.Email.ToString();
                        TempData["Profile"] = communityTemporaryMemberDTO.Profile.ToString();
                        TempData["AffiliateCode"] = communityTemporaryMemberDTO.AffiliateCode.ToString();
                        TempData.Keep();
                        string stripeurl = CreateMemberSubscription(customerResponse.Customer.Id, communityTemporaryMemberDTO.Mobile, communityTemporaryMemberDTO.CommunityId
                            , communityTemporaryMemberDTO.Email.ToString(), communityTemporaryMemberDTO.FirstName.ToString(), communityTemporaryMemberDTO.LastName.ToString()).Result;
                        if (!string.IsNullOrEmpty(stripeurl))
                        {
                            Response.Headers.Add("Location", stripeurl);
                            return new StatusCodeResult(303);
                        }
                          return View("Join");
                    }
                    else
                    return View("Join");
                    }
                    else
                    {
                        return View("Join");
                    }
          
        }

        [ActionLog("Join", "{0} Create Member.")]
        private async Task<string> CreateMemberSubscription(long customerId, string Mobile, long communityId, string email, string FirstName, string LastName)
        {
            try
            {
                TransactionRequest transactions = new TransactionRequest();
                transactions.TransactionFrom = customerId;
                transactions.TransactionTo = transactions.TransactionFrom;
                transactions.TransactionTypeId = (long)TransactionTypeEnum.Subscription;
                transactions.TransactionStatusId = (long)TransactionStatusEnum.Pending;
                transactions.MobileNumber = Mobile;
                transactions.Currency = _config["Currency"];
                transactions.RequestedTransactionId = 0;
                transactions.ReferenceType = "NewMemberSubscription";
                transactions.ReferenceId = customerId;
                transactions.PaymentDesc = email;
                transactions.Amount = decimal.Parse("100.00");
                transactions.CommunityId = communityId;
                transactions.ServiceCharges = 0;
                long result = await financeService.SubscriptionPayment(transactions);

                string CustomerId = "";
                CustomerId = financeService.CheckSubscriptionCustomer(transactions.TransactionFrom).Result;
                if (string.IsNullOrEmpty(CustomerId))
                {
                    var options = new CustomerCreateOptions
                    {
                        Name = FirstName + " " + LastName,
                        Email = transactions.PaymentDesc,
                        Phone = transactions.MobileNumber,
                        Description = transactions.TransactionFrom.ToString(),
                    };
                    var service = new Stripe.CustomerService();
                    Customer stripeCustomer = service.Create(options);
                    CustomerId = stripeCustomer.Id;
                    financeService.SaveSubscriptionCustomer(transactions.TransactionFrom, CustomerId);
                }


                var sessionoptions = new SessionCreateOptions
                {
                    Mode = "subscription",
                    SuccessUrl = _config["SuccessUrl"],
                    CancelUrl = _config["CancelUrl"],
                    Customer = CustomerId,
                    ClientReferenceId = result.ToString(),
                    Currency = _config["CurrencyCode"],
                };
                sessionoptions.LineItems = new List<SessionLineItemOptions>();
                SessionLineItemOptions sessionLineItemOptionsYearSubs = new SessionLineItemOptions
                {
                    Price = "price_1OTsHDHMEFtS5R4BNWUKT2iP",
                    Quantity = 1,
                };
                sessionoptions.LineItems.Add(sessionLineItemOptionsYearSubs);
                SessionLineItemOptions sessionLineItemOptionsPreOrderCharge = new SessionLineItemOptions
                {
                    Price = "price_1ONLsJHMEFtS5R4BcrElwmmP",
                    Quantity = 1,
                };
                sessionoptions.LineItems.Add(sessionLineItemOptionsPreOrderCharge);
                var sessionservice = new SessionService();
                Session session = sessionservice.Create(sessionoptions);
               return session.Url;
            }
            catch (Exception ex)
            {
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
            APIResponse objResponse = await _generic.SendOTPs(Mobile, loginflow ?? true);
            if (objResponse.StatusCode == 200)
                return Json(new { success = true, message = "OTP Sent Successfully" });
            else
                return Json(new { success = false, message = "" });
        }

    }
}
