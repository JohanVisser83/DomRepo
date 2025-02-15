using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Framework.Utility;
using Circular.Services.CommunityFeatures;
using Circular.Services.Finance;
using Microsoft.AspNetCore.Mvc;
using NewCircularSubscription.Models;
using Stripe.Checkout;
using Stripe;

namespace NewCircularSubscription.Controllers
{
    public class FeatureController : Controller
    {

        private readonly ICommunityFeaturesServices _CommunityFeaturesService;
        private readonly IMapper _mapper;
        private readonly IHelper _helper;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SubscriptionFeaturesModel subscriptionFeatures = new SubscriptionFeaturesModel();
        IFinanceService financeService;
        public FeatureController(ICommunityFeaturesServices _communityFeatures, IMapper mapper, IFinanceService _financeService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IHelper helper)
        {
            _CommunityFeaturesService = _communityFeatures;
            _mapper = mapper;
            _config = configuration;
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _httpContextAccessor = httpContextAccessor;
            financeService = _financeService;
        }




        [Route("Features/")]
        public async Task<IActionResult> Features()
        {
            if (TempData["AuthCode"] == null || TempData["AuthCode"] == "")
                return RedirectToAction("CreateCommunity", "CreateCommunity");

            ViewBag.LearnMoreCircularURl = _config["LearnMoreCircularURl"];
            ViewBag.CommunityPortalURl = _config["CommunityPortalURL"];
            ViewBag.TermsOfUse = _config["TermsOfUse"];
            ViewBag.PrivacyPolicy = _config["PrivacyPolicy"];

            ViewBag.LinkedIn = _config["LinkedIn"];
            ViewBag.WhatsApp = _config["WhatsApp"];
            ViewBag.Contactus = _config["Contactus"];

            TempData["AuthCode"] = TempData["AuthCode"];
            TempData["CustomerId"] = TempData["CustomerId"].ToString();
            TempData["Name"] = TempData["Name"].ToString();
            TempData["UserName"] = TempData["UserName"].ToString();
            subscriptionFeatures.currency = _config["Currency"];
            var subscriptionTiers = await _CommunityFeaturesService.GetCommunityTierFeatures(0);
            subscriptionFeatures.startUpsFeatures = subscriptionTiers.Where(t=> t.Id == 101);
            subscriptionFeatures.essentialFeatures = subscriptionTiers.Where(t => t.Id == 102);
            subscriptionFeatures.revenueMaxFeatures = subscriptionTiers.Where(t => t.Id == 103);
            subscriptionFeatures.startUpsFeaturesYearly = subscriptionTiers.Where(t => t.Id == 104);
            subscriptionFeatures.essentialFeaturesYearly = subscriptionTiers.Where(t => t.Id == 105);
            subscriptionFeatures.revenueMaxFeaturesYearly = subscriptionTiers.Where(t => t.Id == 106);
            return View(subscriptionFeatures);
        }


        [HttpPost]
        public async Task<IActionResult> SaveSelectedFeatureDetails(SubscriptionFeaturesSelectedPlanDTO selectedFeaturesDTO)
        {
            try
            {
                selectedFeaturesDTO.CustomerId = long.Parse(TempData["CustomerId"].ToString());
                TempData["Price"] = selectedFeaturesDTO.Price.ToString();
                TempData["CustomerId"] = TempData["CustomerId"].ToString();
                TempData["Plan"] = selectedFeaturesDTO.Plan.ToString();
                TempData["Name"] = TempData["Name"].ToString();
                SubscriptionFeaturesSelectedPlan selectedCommunityFeatures = _mapper.Map<SubscriptionFeaturesSelectedPlan>(selectedFeaturesDTO);
                var result = await _CommunityFeaturesService.SaveSelectedFeatureDetails(selectedCommunityFeatures);
                TempData["ReferenceId"] = result.ToString();

                return Json(new { success = true, message = "", data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, data = 0 });
            }
        }



        public async Task<IActionResult> Payment()
        {
            try
            {
                string price = "";
                TransactionRequest transactions = new TransactionRequest();
                string strName =  TempData["Name"].ToString();
                TempData["Name"] = strName;
                transactions.TransactionFrom = long.Parse(TempData["CustomerId"].ToString());
                TempData["CustomerId"] = TempData["CustomerId"];
                transactions.TransactionTo = transactions.TransactionFrom;
                transactions.TransactionTypeId = (long)TransactionTypeEnum.Subscription;
                transactions.TransactionStatusId = (long)TransactionStatusEnum.Pending;
                transactions.MobileNumber = "";
                transactions.Currency = _config["Currency"];
                transactions.RequestedTransactionId = 0;
                transactions.ReferenceType = "NewSubscription";
                transactions.ReferenceId = long.Parse(TempData["ReferenceId"].ToString());
                TempData["ReferenceId"] = TempData["ReferenceId"];
                transactions.PaymentDesc = TempData["UserName"].ToString();
                TempData["UserName"] = TempData["UserName"];
                transactions.Amount = decimal.Parse(TempData["Price"].ToString());
                TempData["Price"] = TempData["Price"];
                transactions.CommunityId = 97;
                transactions.ServiceCharges = 0;
                if (_config["IncludeServiceCharge"] == "TRUE")
                {
                    transactions.ServiceCharges = ((transactions.Amount * decimal.Parse(_config["ServiceCharge"] ?? "0")) / 100);
                    if (transactions.ServiceCharges > 0)
                        transactions.ServiceCharges = transactions.ServiceCharges + decimal.Parse(_config["FixedCharge"] ?? "0");
                }
                long result = await financeService.SubscriptionPayment(transactions);
                TempData["AdminTransactionId"] = result.ToString();
                string CustomerId = "";
                CustomerId = financeService.CheckSubscriptionCustomer(transactions.TransactionFrom).Result;
                if (string.IsNullOrEmpty(CustomerId))
                {
                    var options = new CustomerCreateOptions
                    {
                        Name = strName,
                        Email = transactions.PaymentDesc,
                        Phone = "",
                        Description = transactions.TransactionFrom.ToString(),
                    };
                    
                    var service = new CustomerService();
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
                List<SubscriptionFeaturesSelectedPlan> features = financeService.GetSubscriptionSelectedFeatures(transactions.ReferenceId??0).Result;
                sessionoptions.LineItems = new List<SessionLineItemOptions>();

                if (features != null && features.Count > 0)
                {
                    foreach (SubscriptionFeaturesSelectedPlan feature in features)
                    {
                        price = feature.StripePriceId;
                        SessionLineItemOptions sessionLineItemOptions;
                        sessionLineItemOptions = new SessionLineItemOptions
                        {
                            Price = price,
                            
                            Quantity = 1,
                        };
                        if(feature.TrialDays > 0)
                            sessionoptions.SubscriptionData = new SessionSubscriptionDataOptions() { TrialPeriodDays = feature.TrialDays };
                        
                        sessionoptions.LineItems.Add(sessionLineItemOptions);
                    }
                }

               
                TempData["Name"] = strName;
                TempData["CustomerId"] = TempData["CustomerId"]?.ToString();
                TempData["UserName"] = TempData["UserName"];
                TempData["AdminTransactionId"] = TempData["AdminTransactionId"];
                var sessionservice = new SessionService();
                Session session = sessionservice.Create(sessionoptions);

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(_config["CombineCurrencyError"]))
                {
                    TempData["ErrorMessage"] = "Your default currency in stripe is other than current currency you are trying to use.";
                    return RedirectToAction("Error", "Error");
                }
                    return RedirectToAction("Features", "Feature");
            }
        }
    }
}
