using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Filters;
using Circular.Framework.Logger;
using Circular.Framework.ShortMessages;
using Circular.Framework.Utility;
using Circular.Services.Finance;
using Circular.Services.User;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Stripe;
using Stripe.Checkout;
using Swashbuckle.AspNetCore.Annotations;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;

namespace Circular.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ICommon _common;
        private readonly IMapper _mapper;
        private readonly IFinanceService _FinanceService;
        Logger logger;
        private readonly IConfiguration _config;
        public PaymentController(IMapper mapper, ICustomerService customerService, ILoggerManager loggerManager, IHelper helper,
            IBulkSMS bulkSMS, IConfiguration configuration, IFinanceService financeService, ICommon common, IConfiguration config)
        {
            _common = common;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _FinanceService = financeService ?? throw new ArgumentNullException(nameof(financeService));
            logger = LogManager.GetLogger("database");
            _config = config;   
        }

        [AuthorizeOIDC]
        [HttpPost]
        [Route("New")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Payment", "{UserName} New Payment")]
		public async Task<ActionResult<APIResponse>> NewPayment(TransactionRequestDTO transactionRequest)
        {
            APIResponse apiResponse = new APIResponse();
            Customers currentCustomer = _common.CurrentUser();
            TransactionRequest transactions = _mapper.Map<TransactionRequest>(transactionRequest);
            transactions.TransactionTypeId = (long)TransactionTypeEnum.Transfer;
            transactions.TransactionStatusId = (long)TransactionStatusEnum.Success;
            long result = await _FinanceService.NewPayment(transactions, currentCustomer);

            if (result == -1)
            {
                apiResponse.StatusCode = (int)APIResponseCode.Invalid_Request;
                apiResponse.Message = "Payment_Restricted_To_Self";
            }
            else if (result == -2)
            {
                apiResponse.StatusCode = (int)APIResponseCode.Invalid_Request;
                apiResponse.Message = "Insufficient_Wallet_Balance";
            }
            else if (result <= 0)
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            else
            {
                apiResponse.StatusCode = (int)APIResponseCode.Success;
                apiResponse.Data = result;
            }
            return apiResponse;
        }

        [AuthorizeOIDC]
        [HttpPost]
        [Route("Upload")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Payment", "{UserName} Wallet Upload")]
		public async Task<ActionResult<APIResponse>> WalletUpload(TransactionRequestDTO transactionRequest)
        {
            APIResponse apiResponse = new APIResponse();
            try
            {
                    logger.Info("Inside Payment function");
                    Customers currentCustomer = _common.CurrentUser();
                    TransactionRequest transactions = _mapper.Map<TransactionRequest>(transactionRequest);
                    if (currentCustomer.IsPaymentRestricted)
                        transactions.TransactionTo = currentCustomer.PrimaryCommunity.OwnerCustomerId;
                    else
                        transactions.TransactionTo = transactions.TransactionFrom;
                    transactions.TransactionTypeId = (long)TransactionTypeEnum.ETopup;
                    transactions.TransactionStatusId = (long)TransactionStatusEnum.Pending;
                    long result = await _FinanceService.NewPayment(transactions, currentCustomer);
                    logger.Info("Transaction created:" + result.ToString());

                    Object GatewayResponse =null;

                    if(transactionRequest?.PaymentGateway?.ToLower()=="stripe")
                    {
                        logger.Info("Inside stripe option");

                    var options = new SessionCreateOptions
                    {
                        LineItems = new List<SessionLineItemOptions>
                        {
                            new SessionLineItemOptions
                            {
                                PriceData = new SessionLineItemPriceDataOptions
                                {
                                    UnitAmountDecimal = (transactionRequest.Amount + transactionRequest.ServiceCharges) * 100,
                                    Currency = transactionRequest.Currency,
                                    ProductData = new SessionLineItemPriceDataProductDataOptions
                                    {
                                        Name = transactions.ReferenceType,
                                    },

                                },
                                Quantity = 1,
                            },
                        },
                            Mode = "payment",
                            CustomerEmail = currentCustomer?.CustomerDetails?.Email??"",
                            Metadata = new Dictionary<string, string> { { "ReferenceType", transactions.ReferenceType } },
                            ClientReferenceId = result.ToString(),
                            SuccessUrl =transactionRequest.SuccessUrl,
                            CancelUrl = transactionRequest.FailureUrl
                        };
                        var service = new Stripe.Checkout.SessionService();
               
                        Stripe.Checkout.Session session = service.Create(options);
                        logger.Info("Stripe session created");

                        GatewayResponse = session.Url;
                
                    }
                    else if(transactionRequest?.PaymentGateway?.ToLower() == "peach")
                    {
                        var options = new SessionCreateOptions
                        {
                            LineItems = new List<SessionLineItemOptions>
                            {
                                new SessionLineItemOptions
                                {
                                    PriceData = new SessionLineItemPriceDataOptions
                                    {
                                        UnitAmountDecimal =transactionRequest.Amount,
                                        Currency = transactionRequest.Currency,
                                        ProductData = new SessionLineItemPriceDataProductDataOptions
                                        {
                                            Name = transactionRequest.PaymentDesc
                                        },

                                    },
                                    Quantity = 1,
                                },
                            },
                            Mode = "payment",
                            ClientReferenceId = result.ToString(),
                        };
                        var service = new Stripe.Checkout.SessionService();
                        Stripe.Checkout.Session session = service.Create(options);
                        apiResponse.Data = session;

                    }
                    if (result <= 0)
                        apiResponse.StatusCode = (int)APIResponseCode.Failure;
                    else
                    {
                        apiResponse.StatusCode = (int)APIResponseCode.Success;
                        apiResponse.Data = new { 
                                               TransactionId=result,
                                                Data=GatewayResponse
                                              };
                    }
                    logger.Info("Returning response");

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            }
            return apiResponse;

        }



        [AuthorizeOIDC]
        [HttpPost]
        [Route("MemberSubscription")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Payment", "{UserName} MemberSubscription Payment")]

        public async Task<ActionResult<APIResponse>> MemberSubscription(TransactionRequestDTO transactionRequest)
        {
            APIResponse apiResponse = new APIResponse();
            try
            {
               
                Customers currentCustomer = _common.CurrentUser();
                TransactionRequest transactions = _mapper.Map<TransactionRequest>(transactionRequest);
                transactions.TransactionFrom = (long)transactionRequest.TransactionFrom;
                transactions.TransactionTo = transactions.TransactionFrom;
                transactions.TransactionTypeId = (long)TransactionTypeEnum.MemberSubscription;
                transactions.TransactionStatusId = (long)TransactionStatusEnum.Pending;
                transactions.MobileNumber = transactionRequest.MobileNumber;
                transactions.Currency = transactionRequest.CurrencyCode;
                transactions.RequestedTransactionId = 0;
                transactions.ReferenceType = "MemberSubscription";
                transactions.ReferenceId = transactionRequest.TransactionFrom;
                transactions.PaymentDesc = transactionRequest.MobileNumber;
                transactions.Amount = (decimal)transactionRequest.Amount;
                transactions.CommunityId = transactionRequest.CommunityId;
                transactions.ServiceCharges = 0;
                long result = await _FinanceService.SubscriptionPayment(transactions);

                Object GatewayResponse = null;

                string CustomerId = "";
                CustomerId = _FinanceService.CheckSubscriptionCustomer(transactions.TransactionFrom).Result;
                if (string.IsNullOrEmpty(CustomerId))
                {
                    var options = new CustomerCreateOptions
                    {
                        Name = "",
                        Email = currentCustomer?.CustomerDetails?.Email ?? "",
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
                    Customer = CustomerId,
                    ClientReferenceId = result.ToString(),
                    SuccessUrl = transactionRequest.SuccessUrl,
                    CancelUrl = transactionRequest.FailureUrl,
                    Currency = transactionRequest.Currency,
                };

                sessionoptions.LineItems = new List<SessionLineItemOptions>();
                SessionLineItemOptions sessionLineItemOptionsMemberCharge = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmountDecimal = (transactions.Amount + transactions.ServiceCharges) * 100,
                        Currency = transactionRequest.Currency,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = transactions.ReferenceType,
                        },
                        Recurring = new SessionLineItemPriceDataRecurringOptions() { Interval = "month", IntervalCount = 1 },
                    },
                    Quantity = 1,
                };

                sessionoptions.LineItems.Add(sessionLineItemOptionsMemberCharge);
                var sessionservice = new SessionService();
                Session session = sessionservice.Create(sessionoptions);
                GatewayResponse = session.Url;


                apiResponse.StatusCode = (int)APIResponseCode.Success;
                apiResponse.Data = new
                {
                    TransactionId = result,
                    Data = GatewayResponse
                };
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                if (ex.Message.Contains("cannot combine currencies on a single customer"))
                    apiResponse.StatusCode = (int)APIResponseCode.Different_Currency;
                else
                    apiResponse.StatusCode = (int)APIResponseCode.Failure;
            }

            return apiResponse;
        }


        [HttpPost]
        [IgnoreAntiforgeryToken]
        [Route("ListeningSubscriptionEvents")]

        public async Task<ActionResult<APIResponse>> SubscriptionEvents()
        {
              string endpointSecret = _config["StripeventSecretKey"];

            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                
                var stripeEvent = EventUtility.ConstructEvent(json,
                   Request.Headers["Stripe-Signature"], endpointSecret);

                if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    
                }
                else if (stripeEvent.Type == Events.PaymentMethodAttached)
                {
                    var paymentMethod = stripeEvent.Data.Object as Stripe.PaymentMethod;
                   
                }
               
                else
                {
                    
                }

                return Ok();
            }
            catch(StripeException ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [AuthorizeOIDC]
        [HttpPost]
        [Route("Request")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Payment", "{UserName} Requested Payment")]
		public async Task<ActionResult<APIResponse>> RequestPayment(TransactionRequestDTO transactionRequest)
        {
            APIResponse apiResponse = new APIResponse();
            Customers currentCustomer = _common.CurrentUser();
            TransactionRequest transactions = _mapper.Map<TransactionRequest>(transactionRequest);
            transactions.TransactionTypeId = (long)TransactionTypeEnum.Requested;
            transactions.TransactionStatusId = (long)TransactionStatusEnum.Pending;
            long result = await _FinanceService.NewPayment(transactions, currentCustomer);

            if (result == -1)
            {
                apiResponse.StatusCode = (int)APIResponseCode.Invalid_Request;
                apiResponse.Message = "RequestPayment_Restricted_To_Self";
            }
			else if (result == -4)
				apiResponse.StatusCode = (int)APIResponseCode.Not_Allowed;
			else if (result <= 0)
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            else
            {
                apiResponse.StatusCode = (int)APIResponseCode.Success;
                apiResponse.Data = result;
            }
            return apiResponse;
        }



        [AuthorizeOIDC]
        [HttpPost]
        [Route("Order")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Payment", "{UserName} Requested Order CheckOut")]
		public async Task<ActionResult<APIResponse>> OrderCheckOut(TransactionRequestDTO transactionRequest)
        {
            APIResponse apiResponse = new APIResponse();
            Customers currentCustomer = _common.CurrentUser();
            TransactionRequest transactions = _mapper.Map<TransactionRequest>(transactionRequest);
            transactions.TransactionTypeId = (long)TransactionTypeEnum.Transfer;
            transactions.TransactionStatusId = (long)TransactionStatusEnum.Success;
            long result = await _FinanceService.NewPayment(transactions, currentCustomer);

            if (result == -1)
            {
                apiResponse.StatusCode = (int)APIResponseCode.Invalid_Request;
                apiResponse.Message = "Payment_Restricted_To_Self";
            }
            else if (result == -2)
            {
                apiResponse.StatusCode = (int)APIResponseCode.Invalid_Request;
                apiResponse.Message = "Insufficient_Wallet_Balance";
            }
            else if (result == -3)
            {
                apiResponse.StatusCode = (int)APIResponseCode.Invalid_Request;
                apiResponse.Message = "Missing_OrderDetails";
            }
            else if (result <= 0)
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            else
            {
                apiResponse.StatusCode = (int)APIResponseCode.Success;
                apiResponse.Data = result;
            }
            return apiResponse;
        }



        [AuthorizeOIDC]
        [HttpPost]
        [Route("Transactions")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Payment", "{UserName} Requested Transactions")]
		public async Task<ActionResult<APIResponse>> Transactions(CustomerIdRequestDTO customerIdRequest)
        {
            APIResponse apiResponse = new APIResponse();
            var result = await _FinanceService.GetTransactions(customerIdRequest.CustomerId);
            apiResponse.Data = result;
            if (result != null)
                apiResponse.StatusCode = (int)APIResponseCode.Success;
            else
                apiResponse.StatusCode = (int)APIResponseCode.Failure;

            return apiResponse;
        }

        [AuthorizeOIDC]
        [HttpPost]
        [Route("TransactionDetail")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Payment", "{UserName} Requested Transactions Detail")]
		public async Task<ActionResult<APIResponse>> TransactionDetail(TransactionDetailDTO transactionDetail)
        {
            APIResponse apiResponse = new APIResponse();
            var result = await _FinanceService.GetTransactionDetail(transactionDetail.TransactionId, transactionDetail.CustomerId);
            apiResponse.Data = result;
            if (result != null)
                apiResponse.StatusCode = (int)APIResponseCode.Success;
            else
                apiResponse.StatusCode = (int)APIResponseCode.Failure;

            return apiResponse;
        }


        [AuthorizeOIDC]
        [HttpPost]
        [Route("Withdraw")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Payment", "{UserName} Requested Withdraw Payment")]
		public async Task<ActionResult<APIResponse>> WithdrawPayment(CustomerWithdrawalRequestDTO withdrawalRequest)
        {
            APIResponse apiResponse = new APIResponse();
            Customers currentCustomer = _common.CurrentUser();
            CustomerWithdrawalRequest customerWithdrawalRequest = _mapper.Map<CustomerWithdrawalRequest>(withdrawalRequest);
            long result = await _FinanceService.CustomerWithdrawalRequest(customerWithdrawalRequest, currentCustomer);
            if (result == -1)
            {
                apiResponse.StatusCode = (int)APIResponseCode.Invalid_Request;
                apiResponse.Message = "Minimum_Withdrawal_Violation";
            }
            else if (result == -2)
            {
                apiResponse.StatusCode = (int)APIResponseCode.Invalid_Request;
                apiResponse.Message = "Insufficient_Wallet_Balance";
            }
            else if (result == -3)
            {
                apiResponse.StatusCode = (int)APIResponseCode.Invalid_Request;
                apiResponse.Message = "Insufficient_Wallet_Balance";
            }
            else if (result <= 0)
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            else
            {
                apiResponse.StatusCode = (int)APIResponseCode.Success;
                apiResponse.Data = result;
            }
            return apiResponse;
        }



       [AuthorizeOIDC]
        [HttpPost]
        [Route("Bank/Add")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Payment", "{UserName} Requested Add Bank")]
		public async Task<ActionResult<APIResponse>> AddBank(CustomerBankAccountsDTO customerBankAccount)
        {
            APIResponse apiResponse = new APIResponse();
            CustomerBankAccounts _customerBankAccount = _mapper.Map<CustomerBankAccounts>(customerBankAccount);
            _customerBankAccount.AccountOwnerTypeId = (int)AccountOwnerType.Customer;
            int result = await _FinanceService.SaveAsync(_customerBankAccount);
            if (result <= 0)
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            else
            {
                apiResponse.StatusCode = (int)APIResponseCode.Success;
            }
            return apiResponse;
        }

        [AuthorizeOIDC]
        [HttpPost]
        [Route("Bank/Remove")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Payment", "{UserName} Remove Bank")]
		public async Task<ActionResult<APIResponse>> RemoveBank(CustomerBankAccountsIdDTO customerBankAccount)
        {
            APIResponse apiResponse = new APIResponse();
            long result = await _FinanceService.DeleteAsync(customerBankAccount.customerBankAccountId);
            if (result <= 0)
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            else
            {
                apiResponse.StatusCode = (int)APIResponseCode.Success;
            }
            return apiResponse;
        }


        [AuthorizeOIDC]
        [HttpPost]
        [Route("Invoice")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Payment", "{UserName} Requested Invoice")]
		public async Task<ActionResult<APIResponse>> SendInvoice(EmailDetailDTO transactionDetail)
        {
            APIResponse apiResponse = new APIResponse();
            bool result = await _FinanceService.SendInvoice(transactionDetail.TransactionId, transactionDetail.CustomerId,
                transactionDetail.EmailId, _common.CurrentUser().WalletBalance.ToString());
            apiResponse.Data = result;
            if (result != false)
                apiResponse.StatusCode = (int)APIResponseCode.Success;
            else
                apiResponse.StatusCode = (int)APIResponseCode.Failure;

            return apiResponse;
        }

        [AuthorizeOIDC]
        [HttpPost]
        [Route("Decline")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Payment", "{UserName} Requested Decline Payment")]
		public async Task<ActionResult<APIResponse>> DeclinePayment(DeclineTransactionDTO transactionDetail)
        {
			APIResponse apiResponse = new APIResponse();
			Customers currentCustomer = _common.CurrentUser();
			TransactionRequest transactions = new TransactionRequest();
            transactions.TransactionTypeId = (long)TransactionTypeEnum.Declined;
			transactions.TransactionStatusId = (long)TransactionStatusEnum.Failed;
            transactions.RequestedTransactionId = transactionDetail.TransactionId;
            transactions.TransactionTo = transactionDetail.ToCustomerId;
            transactions.Amount = transactionDetail.Amount??0;
            transactions.PaymentDesc = "";
            transactions.MobileNumber = "";
			transactions.TransactionFrom = transactionDetail.FromCustomerId;
            transactions.CommunityId = currentCustomer.PrimaryCommunity.CommunityId;
            transactions.ServiceCharges = 0;
            transactions.IsPaymentFromMessageModule = false;
			long result = await _FinanceService.NewPayment(transactions, currentCustomer);

			if (result == -1)
			{
				apiResponse.StatusCode = (int)APIResponseCode.Invalid_Request;
				apiResponse.Message = "RequestPayment_Restricted_To_Self";
			}
			else if (result == -4)
				apiResponse.StatusCode = (int)APIResponseCode.Not_Allowed;
			else if (result <= 0)
				apiResponse.StatusCode = (int)APIResponseCode.Failure;
			else
			{
				apiResponse.StatusCode = (int)APIResponseCode.Success;
				apiResponse.Data = result;
			}
			return apiResponse;
		}
	}
}
