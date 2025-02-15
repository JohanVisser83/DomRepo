using Circular.Core.Entity;
using Circular.Data.Repositories.Finance;
using Circular.Framework.Notifications;
using Circular.Services.Message;
using Circular.Services.Notifications;
using Circular.Services.User;
using Circular.Services.Storefront;
using Circular.Services.Email;
using Circular.Framework.Middleware.Emailer;
using MailKit.BounceMail;
using Circular.Data.Repositories.Planners;
using Org.BouncyCastle.Tls;
using ZXing;

namespace Circular.Services.Finance
{
    public class FinanceService : IFinanceService
    {

        private readonly IFinanceRepository _financeRepository;
        private readonly IPlannerRepository _plannerRepository;
        private readonly IMessageService _messageService;
        private readonly INotificationService _notificationService;
        private readonly ICustomerService _customerService;
        private readonly IStorefrontServices _storefrontService;
        IMailService _mailService;


        public FinanceService(IFinanceRepository financeRepository, IMessageService messageService, INotificationService notificationService,
            ICustomerService customerService, IStorefrontServices storefrontService, IMailService mailService, IPlannerRepository plannerRepository)
        {
            _financeRepository = financeRepository;
            _plannerRepository = plannerRepository;
            _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(_customerService));
            _storefrontService = storefrontService ?? throw new ArgumentNullException(nameof(_storefrontService));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        }


        public async Task<long> SubscriptionPayment(TransactionRequest transactions)
        {
            long transactionId = await _financeRepository.NewPayment(transactions);
            return transactionId;
        }
        public async Task<string> CheckSubscriptionCustomer(long customerId)
        {
            return await _financeRepository.CheckSubscriptionCustomer(customerId);
        }
        public async Task<long> SaveSubscriptionCustomer(long customerId, string StripeCustomerId)
        {
            return await _financeRepository.SaveSubscriptionCustomer(customerId, StripeCustomerId);
        }
        public async Task<List<FeatureSubscriptionsFee>> GetSubscriptionFeatures(long customerId, long communityId)
        {
            return await _financeRepository.GetSubscriptionFeatures(customerId, communityId);
        }

        public async Task<long> NewPayment(TransactionRequest transactions, Customers currentCustomer)
        {
            Customers ToCustomer = null;
            Core.Entity.Order order = null;
            if (transactions.TransactionTo > 0)
                ToCustomer = _customerService.getcustomerbyId(transactions.TransactionTo ?? 0, true);
            else
                ToCustomer = _customerService.getcustomerByUserName(transactions.MobileNumber ?? "", true);

            transactions.TransactionTo = ToCustomer.Id;
            transactions.Currency = currentCustomer.PrimaryCommunity.currencyCode;


            if (transactions.TransactionTypeId == (long)TransactionTypeEnum.StoreFront)
                order = await _storefrontService.GetOrder(transactions.OrderId ?? 0);

            if (currentCustomer.Id == ToCustomer.Id &&
                transactions.TransactionTypeId != (long)TransactionTypeEnum.ETopup &&
                 transactions.TransactionTypeId != (long)TransactionTypeEnum.Declined &&
                transactions.TransactionTypeId != (long)TransactionTypeEnum.Withdrawal
                )
                return -1;
            else if ((currentCustomer.WalletBalance - transactions.Amount <= 0) &&
                transactions.TransactionTypeId != (long)TransactionTypeEnum.Requested &&
                 transactions.TransactionTypeId != (long)TransactionTypeEnum.Declined &&
                transactions.TransactionTypeId != (long)TransactionTypeEnum.ETopup
                )

                return -2;
            else if ((order == null || order.IsPaid == true)
                && transactions.TransactionTypeId == (long)TransactionTypeEnum.StoreFront)
                return -3;
            else if (
               transactions.TransactionTypeId == (long)TransactionTypeEnum.Transfer &&
               currentCustomer.IsPaymentRestricted
               )
                return -4;
            else
            {
                long transactionId = await _financeRepository.NewPayment(transactions);
                if (transactions.IsPaymentFromMessageModule)
                {
                    Messages message = new Messages();
                    message.IsNewMessage = true;
                    message.FromId = transactions.TransactionFrom;
                    message.ToId = transactions.TransactionTo ?? 0;
                    message.ReferenceId = transactionId;
                    message.IsPaid = (transactions.TransactionStatusId == (int)TransactionStatusEnum.Success);
                    if (transactions.TransactionTypeId == (long)TransactionTypeEnum.Requested)
                    {
                        message.MessageTypeId = (int)MessageTypes.Payment_Request;
                        message.Message = "Payment requested";
                    }
                    else
                    {
                        message.MessageTypeId = (int)MessageTypes.Payment_Done;
                        message.Message = "Payment done";
                    }
                    await _messageService.Save(message);

                }
                if (order != null && order.IsPaid != true && transactions.TransactionTypeId == (long)TransactionTypeEnum.StoreFront)
                {
                    order.ModifiedBy = currentCustomer.Id;
                    await _storefrontService.PayOrder(order);
                }
                string Currency = currentCustomer.PrimaryCommunity.currencyCode;
                string SenderName = currentCustomer.CustomerDetails.FirstName + " " + currentCustomer.CustomerDetails.LastName;
                string ReceipientName = ToCustomer.CustomerDetails.FirstName + " " + ToCustomer.CustomerDetails.LastName;
                string senderMobile = currentCustomer.Mobile;
                string RecipientMobile = ToCustomer.Mobile;
                long communityId = currentCustomer.PrimaryCommunity.CommunityId ?? 0;

                if (transactions.TransactionTypeId == (long)TransactionTypeEnum.Requested)
                {
                    _notificationService.Notify(NotificationTypes.Request_Payment,
                        NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", transactions.TransactionTo.ToString()), SenderName,
                        "You received a request for payment " + Currency + "" + transactions.Amount + " from " + SenderName + " (" + senderMobile + ")"
                        , transactionId, transactions.Amount, false, "", transactions.TransactionFrom, communityId, "", transactions.TransactionTo ?? 0);
                }
                else if (transactions.TransactionTypeId == (long)TransactionTypeEnum.Declined)
                {
                    _notificationService.Notify(NotificationTypes.Payment_Decline,
                        NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", transactions.TransactionTo.ToString()), SenderName,
                        "Your payment request is declined by " + SenderName + " (" + senderMobile + ")"
                        , transactionId, transactions.Amount, false, "", transactions.TransactionFrom, communityId, "", transactions.TransactionTo ?? 0);
                }
                else if (transactions.TransactionTypeId == (long)TransactionTypeEnum.Transfer)
                {
                    _notificationService.Notify(NotificationTypes.Money_Received,
                        NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", transactions.TransactionTo.ToString()), SenderName,
                        "Your wallet is  credited with " + Currency + "" + transactions.Amount + " from " + SenderName + " (" + senderMobile + ")"
                        , transactionId, transactions.Amount, false, "", transactions.TransactionFrom, communityId, "", transactions.TransactionTo ?? 0);

                    _notificationService.Notify(NotificationTypes.Money_Sent,
                        NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", transactions.TransactionFrom.ToString()), ReceipientName,
                        "You have successfully transferred " + Currency + "" + transactions.Amount + " to " + ReceipientName + " (" + RecipientMobile + ")"
                        , transactionId, transactions.Amount, false, "", transactions.TransactionFrom, communityId, "", transactions.TransactionFrom);
                }
                else if (transactions.OrderId >= 0 && transactions.TransactionTypeId == (long)TransactionTypeEnum.StoreFront)
                {
                    _notificationService.Notify(NotificationTypes.Money_Received,
                    NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", transactions.TransactionTo.ToString()),
                    "Storefront sale",
                    "Your wallet is  credited with " + Currency + "" + transactions.Amount + " from " + SenderName + " (" + senderMobile + ")"
                    , transactionId, transactions.Amount, false, "", transactions.TransactionFrom, communityId, "", transactions.TransactionTo ?? 0);

                    _notificationService.Notify(NotificationTypes.Money_Sent,
                    NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", transactions.TransactionFrom.ToString()),
                    "Storefront purchase",
                    "You have successfully made payment of  " + Currency + "" + transactions.Amount + " from " + ReceipientName + " (" + RecipientMobile + ")"
                    , transactionId, transactions.Amount, false, "", transactions.TransactionFrom, communityId, "", transactions.TransactionFrom);
                }

                return transactionId;
            }
        }
        public async Task<dynamic> GetTransactions(long customerId)
        {
            return await _financeRepository.GetTransactions(customerId);
        }
        public async Task<dynamic> GetTransactionDetail(long transactionId, long customerId)
        {
            return await _financeRepository.GetTransactionDetail(transactionId, customerId);
        }
        public async Task<int> SaveAsync(CustomerBankAccounts item)
        {
            item.FillDefaultValues();
            return await _financeRepository.SaveAsync(item);
        }
        public async Task<int> DeleteAsync(long customerBankAccountId)
        {
            return await _financeRepository.DeleteAsync(customerBankAccountId);
        }
        public async Task<long> CustomerWithdrawalRequest(CustomerWithdrawalRequest customerWithdrawalRequest, Customers currentCustomer)
        {
            string MinWithdrawl = currentCustomer.PrimaryCommunity.CommunitySettings.Where(cs => cs.Key == "Minimum_Withdrawal")?.FirstOrDefault<Settings>()?.Value ?? "0";
            if (customerWithdrawalRequest.Amount < Convert.ToDecimal(MinWithdrawl))
                return -1;
            else if (currentCustomer.WalletBalance < 0 || customerWithdrawalRequest.Amount > currentCustomer.WalletBalance)
                return -2;
            else if (currentCustomer.BankAccounts.Where(BA => BA.Id == customerWithdrawalRequest.SavedBankId) == null)
                return -3;
            else
            {
                long result = await MakeWithdrawal(customerWithdrawalRequest);
                _notificationService.Notify(NotificationTypes.Withdrawal_Request,
                                            NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", customerWithdrawalRequest.CustomerId.ToString()), "Withdrawal Request",
                                            "You have successfully requested a withdrawal of " + (currentCustomer.PrimaryCommunity.currencyCode ?? "") + "" + customerWithdrawalRequest.Amount
                                            , result, customerWithdrawalRequest.Amount ?? 0, false, "",
                                            customerWithdrawalRequest.CustomerId ?? 0, currentCustomer.PrimaryCommunity.CommunityId ?? 0, "", customerWithdrawalRequest.CustomerId ?? 0);

                //Send Email
                return result;
            }
        }
        public async Task<long> MakeWithdrawal(CustomerWithdrawalRequest Transaction)
        {

            Transaction.FillDefaultValues();
            var result = await _financeRepository.MakeWithdrawal(Transaction);
            var Emails = await _plannerRepository.SendEmailPlanner((long)Transaction.CustomerId);



            if (result > 0)
            {

                MailRequest mailRequest = new MailRequest();
                mailRequest.FromUserId = Transaction.CustomerId ?? 0;
                mailRequest.To = Emails[0].Email;
                mailRequest.ReferenceId = Transaction.Id;
                MailSettings mailSettings = _mailService.EmailParameter(MailType.withdrawal_request, ref mailRequest);

                string body = mailRequest.Body;
                string[] PlaceHolders = { "$Amount", "$account", "$created", "$customerName" };
                string[] Values = { Transaction.Amount.ToString(), Transaction.BankName, Transaction.CreatedDate.ToString("dd MMM yyyy"), Emails[0].Name };
                if (!string.IsNullOrEmpty(body) && (PlaceHolders.Length == Values.Length))
                {
                    for (int index = 0; index < PlaceHolders.Length; index++)
                        body = body.Replace(PlaceHolders[index], Values[index]);
                }
                mailRequest.Body = body;

                int result1 = Convert.ToInt16(await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings));
                return result1;

            }
            else
            {
                return 0;
            }
        }



        public async Task<bool> SendInvoice(long transactionId, long customerId, string ToEmailId, string WalletBalance)
        {

            MailRequest mailRequest = new MailRequest();
            mailRequest.FromUserId = customerId;
            mailRequest.To = ToEmailId;
            mailRequest.ReferenceId = transactionId;
            MailSettings mailSettings = _mailService.EmailParameter(MailType.Transaction, ref mailRequest);
            //Parse the body
            string body = mailRequest.Body;

            var transactionDetails = GetTransactionDetail(transactionId, customerId).Result;
            if (transactionDetails == null) return false;

            IDictionary<string, Object> data = transactionDetails[0];
            if (data == null) return false;

            string[] PlaceHolders = { "$UserName", "$mobile", "$TransactionDate", "$withdrawamount",
                "$RecipientName","$WalletBalance"};
            string[] Values = { data["TransactionFromCustomerName"].ToString(), data["Mobile"].ToString(), ((DateTime)data["TransactionDate"]).ToString("dddd, dd MMMM yyyy HH:mm"),
                data["Amount"].ToString(), data["TransactionWithCustomerName"].ToString(),WalletBalance};
            if (!string.IsNullOrEmpty(body) && (PlaceHolders.Length == Values.Length))
            {
                for (int index = 0; index < PlaceHolders.Length; index++)
                    body = body.Replace(PlaceHolders[index], Values[index]);
            }

            mailRequest.Body = body;
            return await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);
        }
        public async Task<List<CustomerBankAccounts>> PostBankAccAsync()
        {
            return await _financeRepository.PostBankAccAsync();
        }
        public async Task<IEnumerable<dynamic>> GetBankDetailsAsync(long Id, long primaryId)
        {
            return await _financeRepository.GetBankDetailsAsync(Id, primaryId);

        }
        public async Task<List<Country>> GetMasterCountryAsync()
        {
            return await _financeRepository.GetMasterCountryAsync();
        }
        public async Task<List<Banks>> GetMasterBank(string Code)
        {
            return await _financeRepository.GetMasterBank(Code);
        }
        public async Task<dynamic> GetCommunityTransactions(DateTime startdate, DateTime enddate, long count, long primarycommunity)
        {
            return await _financeRepository.GetCommunityTransactions(startdate, enddate, count, primarycommunity);
        }
        public Task<IEnumerable<WithdrawalFeature>> GetWithdrawalFeature()
        {
            return _financeRepository.GetWithdrawalFeature();
        }
        public async Task<long> RefundAsync(TransactionRequest transactions)
        {
            return await _financeRepository.RefundAsync(transactions);
        }
        public async Task<IEnumerable<dynamic>> GetRefundDetails(long CommunityId)
        {
            return await _financeRepository.GetRefundDetails(CommunityId);
        }
        public async Task<IEnumerable<dynamic>> GetEventName(string EventName,long CommunityId)
        {
            return await _financeRepository.GetEventName(EventName, CommunityId);
        }
        public async Task<IEnumerable<dynamic>> GetAvailabelBalance(string EventName, long Id)
        {
            return await _financeRepository.GetAvailabelBalance(EventName, Id);
        }
        public async Task<IEnumerable<dynamic>> BankListWithdrawal(long BankId, long communityId)
        {
            return await _financeRepository.BankListWithdrawal(BankId, communityId);

        }

        public async Task<dynamic> PendingWithdrawal(long CommunityId, long customerId)
        {
            return await _financeRepository.PendingWithdrawal(CommunityId, customerId);
        }

        public bool UpdateTransactionStatus(long transactionid, string gatewaystatus, string result)
        {
            return _financeRepository.UpdateTransactionStatus(transactionid, gatewaystatus, result);
        }

        public string UpdateSubscriptionStatus(long transactionid, string gatewaystatus, string result)
        {
            return _financeRepository.UpdateSubscriptionStatus(transactionid, gatewaystatus, result);
        }
        public string UpdateMemberSubscriptionStatus(long transactionid, string gatewaystatus, string result)
        {
            return _financeRepository.UpdateMemberSubscriptionStatus(transactionid, gatewaystatus, result);
        }
        public string UpdateCommunityMemberSubscriptionStatus(long transactionid, string gatewaystatus, string result)
        {
            return _financeRepository.UpdateCommunityMemberSubscriptionStatus(transactionid, gatewaystatus, result);
        }
        
        public string UpdateCommunityTransactionStatus(long transactionid, string gatewaystatus, string result)
        {
            return _financeRepository.UpdateCommunityTransactionStatus(transactionid, gatewaystatus, result);
        }
        
        public async Task<dynamic> PendingWithdrawalView(long TransactionTypeId, long Id)
        {
            return await _financeRepository.PendingWithdrawalView(TransactionTypeId, Id);
        }

        public async Task<IEnumerable<dynamic>> PaidWithdrawal(long transactionStatusId, long TransactionTypeId)
        {
            return await _financeRepository.PaidWithdrawal(transactionStatusId, TransactionTypeId);
        }

        public async Task<int> DeleteBankAccount(long id)

        {

            return await _financeRepository.DeleteBankAccount(id);


        }

        public async Task<List<Banks>> GetUserBankListAsync(string Code, string Search)
        {
            return await _financeRepository.GetUserBankListAsync(Code, Search);
        }
        public async Task<Ewallet> WalletBalance(long CommunityId)
        {
            return await _financeRepository.WalletBalance(CommunityId);

        }


        public async Task<long> SaveBillingAddress(SubscriptionBilling subscriptionBilling)
        {
            subscriptionBilling.FillDefaultValues();
            return await _financeRepository.SaveBillingAddress(subscriptionBilling);
        }

        public async Task<List<SubscriptionFeaturesSelectedPlan>> GetSubscriptionSelectedFeatures(long Id)
        {
            return await _financeRepository.GetSubscriptionSelectedFeatures(Id);
        }

        public async Task<long> CreateSubscriptionUser(string email, string mobile, string firstName, string lastName)
        {
            return await _financeRepository.CreateSubscriptionUser(email, mobile, firstName, lastName);
        }

        public async Task<int> SaveSubscriptionDetails(SubscriptionDetails newSubscription)
        {
            newSubscription.FillDefaultValues();    
            return await _financeRepository.SaveSubscriptionDetails(newSubscription);
        }

        public Transactions GetTransactionCustomerId(long TransactionId)
        {
            return _financeRepository.GetTransactionCustomerId(TransactionId);
        }


        public async Task<bool> GetCommunityExpiredSubscriptionDetails()
        {
            try
            {
                ExpiredSubscriptionsNotifications expiredSubscriptionsNotifications = await _financeRepository.GetCommunityExpiredSubscriptionDetails();
                if(expiredSubscriptionsNotifications.actualExpiredSubscriptions != null)
                {
                    foreach (ActualExpiredSubscriptions cd in expiredSubscriptionsNotifications.actualExpiredSubscriptions)
                    {
                        //1
                            string msg = "";
                            string topic = "";
                            long receiverId = 0;

                            _notificationService.Notify(Circular.Framework.Notifications.NotificationTypes.ExpiredSubscription,
                            NotificationTopics.Circular_CancelSubscription.ToString().Replace("ReferenceId",cd.CustomerId.ToString()),
                            "", "Your circular community subscription has expired due to a missed payment. Please reach out to your community administrator for any clarifications.. " + "", 0, 0, false, "",
                            cd.CustomerId, cd.CommunityMembershipId, "", cd.CustomerId);

                        MailRequest mailRequest = new MailRequest
                        {
                            FromUserId = cd.CustomerId,
                            ReferenceId = cd.CustomerId
                        };

                        MailSettings mailSettings = _mailService.EmailParameter(MailType.Cancle_Subscription_Commportal, ref mailRequest);
                        mailRequest.To = cd.Email;
                        mailRequest.Body = mailRequest.Body.Replace("$Email", cd.Email);
                        await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);
                    }
                }
                //2

                if (expiredSubscriptionsNotifications.subscriptionDetails != null)
                {
                    foreach (SubscriptionDetails cd in expiredSubscriptionsNotifications.subscriptionDetails)
                    {

                        string msg = "";
                        string topic = "";
                        long receiverId = 0;

                        _notificationService.Notify(Circular.Framework.Notifications.NotificationTypes.ExpiredSubscription,
                        NotificationTopics.Circular_CancelSubscription.ToString().Replace("ReferenceId", cd.CustomerId.ToString()),
                        "", "Your circular community subscription has expired due to a missed payment. Please reach out to your community administrator for any clarifications.. " + "", 0, 0, false, "",
                        cd.CustomerId, cd.CommunityMembershipId, "", cd.CustomerId);

                        MailRequest mailRequest = new MailRequest
                        {
                            FromUserId = cd.CustomerId,
                            ReferenceId = cd.CustomerId
                        };

                        MailSettings mailSettings = _mailService.EmailParameter(MailType.Cancle_Subscription_Commportal, ref mailRequest);
                        mailRequest.To = cd.Email;
                        mailRequest.Body = mailRequest.Body.Replace("$Email", cd.Email);
                        await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
             
            
        }

        public async Task<IEnumerable<HQCommunityTransactionDetails>> GetHQCommunityTransactions(long communityId)
        {
            return await _financeRepository.GetHQCommunityTransactions(communityId);
        }
    }
}
