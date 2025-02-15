using Circular.Framework.Logger;
using Circular.Services.Finance;
using CircularWeb.filters;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Circular.Core.Entity;
using CircularWeb.Models;

namespace CircularWeb.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ILoggerManager _logger;
        private readonly IFinanceService _financeService;
        private readonly IConfiguration _config;
        public PaymentController(ILoggerManager logger,IFinanceService financeService, IConfiguration configuration)
        {
           
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _financeService = financeService;
            _config = configuration;
        }
        [ActionLog("Payment", "{0} payment success.")]
        public IActionResult Success()
        {
            _logger.LogInfo("Inside Success info");
            
            return View();
        }

        [ActionLog("Payment", "{0} payment failure.")]
        public IActionResult Failure()
        {
            return View();
        }

        #region "Payment Status Page"




        #endregion

        [ActionLog("Payment", "{0} payment status.")]
        public ActionResult PaymentStatus([FromQuery] string session_id)
        {
            _logger.LogInfo("Inside PaymentSuccess info");
            try
            {
                _logger.LogInfo("QueryString-" + session_id.ToString());
                var SecretKey = _config["SecretKey"];
                StripeConfiguration.ApiKey = SecretKey;
                var sessionService = new SessionService();
                Session session = sessionService.Get(session_id);
                if (session != null)
                {
                    string result = session.ToString();
                    _logger.LogInfo("Session-" + result);
                    if (session.CustomerId != null)
                    {
                        var customerService = new CustomerService();
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
                        Transactions transactions = _financeService.GetTransactionCustomerId(long.Parse(session.ClientReferenceId));
                        newSubscription.CustomerId = (long)transactions.TransactionFrom;
                        newSubscription.TransactionId = long.Parse(session.ClientReferenceId);
                        newSubscription.CommunityMembershipId = (long)transactions.CommunityId;
                        _financeService.SaveSubscriptionDetails(newSubscription);
                    };




                    _logger.LogInfo("Status:" + session.PaymentStatus.ToString().ToLower());
                    if (session.PaymentStatus.ToString().ToLower() == "paid")
                    {
                        _financeService.UpdateTransactionStatus(long.Parse(session.ClientReferenceId), "Complete", result);
                        return RedirectToAction("Success", "Payment");
                    }
                    else
                        return RedirectToAction("Failure", "Payment");
                }
                else
                {
                    _logger.LogInfo("Inside No Session Object");
                    return RedirectToAction("Failure", "Payment");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in fetching status from payment gateway - " + ex.Message);
                return RedirectToAction("Failure", "Payment");
            }

        }


    }
}
