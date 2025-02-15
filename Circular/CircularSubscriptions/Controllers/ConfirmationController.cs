using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Core.Mapper;
using Circular.Framework.Utility;
using Circular.Services.CommunityFeatures;
using Circular.Services.CreateCommunity;
using Circular.Services.Finance;
using CircularSubscriptions.Models;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Tweetinvi.Core.Events;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Stripe;
using Org.BouncyCastle.Ocsp;

namespace CircularSubscriptions.Controllers
{ 
    public class ConfirmationController : Controller
{
        private readonly ICommunityFeaturesServices _CommunityFeaturesService;
        private readonly IMapper _mapper;
        private readonly IHelper _helper;

        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CommunityFeaturesModel communityFeatures;
        IFinanceService financeService;

        public ConfirmationController(ICommunityFeaturesServices _communityFeatures, IFinanceService _financeService, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IHelper helper)
        {
            _CommunityFeaturesService = _communityFeatures;
            _mapper = mapper;
            _config = configuration;
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _httpContextAccessor = httpContextAccessor;
            financeService = _financeService;
        }
        [HttpGet]
        public async Task<IActionResult> Confirmation()
        {
            communityFeatures = new CommunityFeaturesModel();
            if (TempData["TempCommunityId"] == null || TempData["TempCommunityId"] == "")
                return RedirectToAction("EmailVerification", "EmailVerification");
            TempData["TempCommunityId"] = TempData["TempCommunityId"];
            communityFeatures.ActualCost = TempData["ActualCostThisMonth"].ToString();
            if (_config["IncludeServiceCharge"] == "TRUE")
            {
                decimal ActualCost = (decimal.Parse(communityFeatures.ActualCost));
                ActualCost = ActualCost + ((ActualCost * decimal.Parse(_config["ServiceCharge"] ??"0")) / 100);
                if (ActualCost > 0)
                {
                    ActualCost = ActualCost + decimal.Parse(_config["FixedCharge"] ?? "0");
                    communityFeatures.PayButtonText = "Pay " + _config["Currency"] + Decimal.Round(ActualCost,2).ToString();
                }
                else
                    communityFeatures.PayButtonText = "Setup Payment Method";
                communityFeatures.ActualCost = ActualCost.ToString();
            }
            TempData["ActualCostThisMonth"] = TempData["ActualCostThisMonth"];
            TempData["FirstName"] = TempData["FirstName"];
            TempData["LastName"] = TempData["LastName"];
            TempData["CommunityName"] = TempData["CommunityName"];
            TempData["UserName"] = TempData["UserName"];
            TempData["CustomerId"] = TempData["CustomerId"];
            TempData["Mobile"] = TempData["Mobile"];
            return View(communityFeatures);
        }


        public async Task<IActionResult> Payment()
        {
            try
            {
                TransactionRequest transactions = new TransactionRequest();
                transactions.TransactionFrom = long.Parse(TempData["CustomerId"].ToString());
                TempData["CustomerId"] = TempData["CustomerId"];
                transactions.TransactionTo = transactions.TransactionFrom;
                transactions.TransactionTypeId = (long)TransactionTypeEnum.Subscription;
                transactions.TransactionStatusId = (long)TransactionStatusEnum.Pending;
                transactions.MobileNumber = TempData["Mobile"].ToString();
                TempData["Mobile"] = TempData["Mobile"];
                transactions.Currency = _config["CurrencyCode"];
                transactions.RequestedTransactionId = 0;
                transactions.ReferenceType = "NewSubscription";
                transactions.ReferenceId = long.Parse(TempData["TempCommunityId"].ToString());
                TempData["TempCommunityId"] = TempData["TempCommunityId"];
                transactions.PaymentDesc = TempData["UserName"].ToString();
                TempData["UserName"] = TempData["UserName"];
                transactions.Amount = decimal.Parse(TempData["ActualCostThisMonth"].ToString());
                transactions.CommunityId = 97;
                transactions.ServiceCharges = 0;
                if (_config["IncludeServiceCharge"] == "TRUE")
                {
                    transactions.ServiceCharges = ((transactions.Amount * decimal.Parse(_config["ServiceCharge"] ?? "0")) / 100);
                        if(transactions.ServiceCharges > 0)
                        transactions.ServiceCharges = transactions.ServiceCharges + decimal.Parse(_config["FixedCharge"] ?? "0");
                }
                long result = await financeService.SubscriptionPayment(transactions);

                string CustomerId = "";
                CustomerId =  financeService.CheckSubscriptionCustomer(transactions.TransactionFrom).Result;
                if (string.IsNullOrEmpty(CustomerId))
                {
                    var options = new CustomerCreateOptions
                    {
                        Name = TempData["FirstName"] + " " + TempData["LastName"],
                        Email = transactions.PaymentDesc,
                        Phone = transactions.MobileNumber,
                        Description = transactions.TransactionFrom.ToString(),
                    };
                    TempData["FirstName"] = TempData["FirstName"];
                    TempData["LastName"] = TempData["LastName"];
                    TempData["CommunityName"] = TempData["CommunityName"];
                    TempData["ActualCostThisMonth"] = TempData["ActualCostThisMonth"];
                    TempData["UserName"] = TempData["UserName"];
                    TempData["CustomerId"] = TempData["CustomerId"];
                    TempData["Mobile"] = TempData["Mobile"];
                    TempData["TempCommunityId"] = TempData["TempCommunityId"];
                    TempData.Keep();
                    var service = new CustomerService();
                    Customer stripeCustomer = service.Create(options);
                    CustomerId = stripeCustomer.Id;
                    financeService.SaveSubscriptionCustomer(transactions.TransactionFrom, CustomerId);
                }

                //List<FeatureSubscriptionsFee> features = financeService.GetSubscriptionFeatures(transactions.TransactionFrom, transactions.ReferenceId ?? 0).Result;

                var sessionoptions = new SessionCreateOptions
                {
                    Mode = "subscription",
                    SuccessUrl = _config["SuccessUrl"],
                    CancelUrl = _config["CancelUrl"],
                    Customer = CustomerId,
                    ClientReferenceId = result.ToString(),
                    Currency = _config["CurrencyCode"],
                    //SubscriptionData = new SessionSubscriptionDataOptions
                    //{
                    //    BillingCycleAnchor = DateTimeOffset.FromUnixTimeSeconds(1572580800).UtcDateTime,
                    //},
                };
                sessionoptions.LineItems = new List<SessionLineItemOptions>();


                //if (features != null && features.Count > 0)
                //{
                //    foreach (FeatureSubscriptionsFee feature in features)
                //    {
                //        string price = feature.StripePriceId;
                //        SessionLineItemOptions sessionLineItemOptions;
                //        if (!feature.Code.Contains("HelpReq-001"))
                //        {
                //            sessionLineItemOptions = new SessionLineItemOptions
                //            {
                //                Price = price,
                //            };
                //        }
                //        else
                //        {
                //            sessionLineItemOptions = new SessionLineItemOptions
                //            {
                //                Price = price,
                //                Quantity = 1,
                //            };
                //        }
                //        sessionoptions.LineItems.Add(sessionLineItemOptions);
                //    }
                //}
                //Add free Product
                SessionLineItemOptions sessionLineItemOptionsFree = new SessionLineItemOptions
                {
                    Price = "price_1OOfOmKOkPQHDl2PGP9R5EKJ",
                };
                sessionoptions.LineItems.Add(sessionLineItemOptionsFree);

                //Add first month charge
                SessionLineItemOptions sessionLineItemOptionsFirstMonthCharge = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmountDecimal = (transactions.Amount+ transactions.ServiceCharges) * 100,
                        Currency = _config["CurrencyCode"],
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = _config["FirstMonthChargeDescription"],
                        },
                    },
                    Quantity = 1,
                };
                sessionoptions.LineItems.Add(sessionLineItemOptionsFirstMonthCharge);
                var sessionservice = new SessionService();
                Session session = sessionservice.Create(sessionoptions);

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            catch (Exception ex)
            {
                return RedirectToAction("EmailVerification", "EmailVerification");
            }
        }
    }
}