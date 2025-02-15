using Circular.Core.Entity;
using Circular.Data.Repositories.Account;
using Circular.Framework.Middleware.Emailer;
using Circular.Framework.Notifications;
using Circular.Services.Email;
using Circular.Services.Notifications;

namespace Circular.Services.Account
{
    public class AccountServices : IAccountServices
    {
        private readonly IAccountRepository _AccountRepository;
        INotificationService _notificationService;
        IMailService _mailService;        

        public AccountServices(IAccountRepository AccountRepository, INotificationService notificationService, IMailService mailService)
        {
            _AccountRepository = AccountRepository;
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        }

       
        #region "Community Portal Functions"
        public async  Task<IEnumerable<CollectionAggregate>> GetActiveAccount(long communityId)
        {
            return await _AccountRepository.GetActiveAccount(communityId);
        }
        public async Task<IEnumerable<CollectionAggregate>> GetClosedAccount(long communityId)
        {
            return await _AccountRepository.GetClosedAccount(communityId);
        }
        public async Task<int> DeleteActiveAccountitem(long id)
        {
            return await _AccountRepository.DeleteActiveAccountitem(id);
        }
        public async Task<int> ClosedAccount(long id)
        {
            return await _AccountRepository.ClosedAccount(id);
        }
        public async Task<IEnumerable<dynamic>> GetCollectionData(long collectionId)
        {
            return await _AccountRepository.GetCollectionData(collectionId);
        }
        public async Task<IEnumerable<dynamic>> GetViewAccountDetails(long Id)
        {
            return await _AccountRepository.GetViewAccountDetails(Id);
        }
        public async Task<CustomerDetails> GetIndividualDetails(long customerId)
        {
            return  _AccountRepository.GetIndividualDetails(customerId).Result.FirstOrDefault();
        }
        public async Task<long> AddAccountDetails(CollectionAggregate item)
        {
            item.FillDefaultValues();
            item.TotalMemberAtCreation = _notificationService.GetMemberCount(item.GroupId ?? 0, item.CommunityId ?? 0, item.Individual ?? 0);
            long result = await _AccountRepository.AddAccountDetails(item);
            if (result > 0)
            {
                if (item.Individual != 0)
                    item.GroupId = -1;

                BulkAccountsEmail bulkAccountsEmail = new BulkAccountsEmail();
                bulkAccountsEmail.AccountId = result;
                bulkAccountsEmail.Scheduledeliverydate = item.Scheduledeliverydate;
                bulkAccountsEmail.Scheduleddeliverytime = item.Scheduleddeliverytime;
                bulkAccountsEmail.communityId = item.CommunityId ?? 0;

                if ((item.GroupId ?? 0) == 0)
                {                    
                    bulkAccountsEmail.IsGroup = 0;
                    bulkAccountsEmail.GroupId = 0;
                }
                else if ((item.GroupId ?? 0) < 0)
                {
                    bulkAccountsEmail.IsGroup = 2;
                    bulkAccountsEmail.GroupId = item.Individual ?? 0;
                }
                else
                {
                    bulkAccountsEmail.IsGroup = 1;
                    bulkAccountsEmail.GroupId = item.GroupId ?? 0;
                }
                await SendBulkAccountsEmail(bulkAccountsEmail);
            }
            return result;
        }

        public async Task<bool> SendBulkEmails()
        {
            string msg = "";
            string topic = "";
            long receiverId = 0;
            try
            {
                List<BulkAccountsEmail?> result = await _AccountRepository.GetBulkEmaildetails();
                if (result != null)
                {
                    foreach (BulkAccountsEmail bulkEmail in result)
                    {
                         msg = "";
                         topic = "";
                         receiverId = 0;
                        List<CustomerDetails> customerList = new List<CustomerDetails>();
                        if (bulkEmail.IsGroup == 0)
                        {
                            topic = NotificationTopics.Circular_community_ReferenceId.ToString().Replace("ReferenceId", bulkEmail.communityId.ToString());
                            msg = bulkEmail.Title + " account is just added in your community.";
                            receiverId = 0;
                            customerList = await _AccountRepository.SendAddAccountEmail(bulkEmail.communityId, 0);
                        }
                        else if (bulkEmail.IsGroup == 1)
                        {

                            topic = NotificationTopics.Circular_communityGroups_ReferenceId.ToString().Replace("ReferenceId", bulkEmail.GroupId.ToString());
                            msg = bulkEmail.Title + " account is just added in your community group.";
                            receiverId = bulkEmail.GroupId;
                            customerList = await _AccountRepository.SendAddAccountEmail(bulkEmail.GroupId, 1);
                        }
                        else
                        {
                            topic = NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", bulkEmail.GroupId.ToString());
                            msg = bulkEmail.Title + "  account is just added for you.";
                            receiverId = bulkEmail.GroupId;
                            customerList = await _AccountRepository.SendAddAccountEmail(bulkEmail.GroupId, 2);
                        }
                        _notificationService.Notify(NotificationTypes.New_Account, topic, bulkEmail.CommunityName, msg
                            , bulkEmail.AccountId, bulkEmail.Amount??0, false, "", bulkEmail.CreatedBy, bulkEmail.communityId, "", receiverId);


                        if (customerList != null)
                        {
                            foreach (CustomerDetails cd in customerList)
                            {
                                MailRequest mailRequest = new MailRequest();
                                mailRequest.FromUserId = bulkEmail.Accountcustomer;
                                mailRequest.ReferenceId = bulkEmail.IsGroup == 0 ? bulkEmail.communityId : bulkEmail.GroupId;
                                MailSettings mailSettings = _mailService.EmailParameter(MailType.AccountSent, ref mailRequest);

                                mailRequest.To = cd.Email;
                                mailRequest.Body = mailRequest.Body.Replace("$community name", bulkEmail.CommunityName);
                                mailRequest.Body = mailRequest.Body.Replace("$Currency", bulkEmail.Currency);
                                mailRequest.Body = mailRequest.Body.Replace("$Community Name1", bulkEmail.CommunityName);
                                mailRequest.Body = mailRequest.Body.Replace("$userfirstname", cd.FirstName);
                                mailRequest.Body = mailRequest.Body.Replace("$Amount", bulkEmail.Amount.ToString());
                                mailRequest.Body = mailRequest.Body.Replace("$expiry date", bulkEmail.ExpirydateCollection?.ToString("dd MMM yyyy"));
                                mailRequest.Body = mailRequest.Body.Replace("$collection description", bulkEmail.Description);
                                mailRequest.Body = mailRequest.Body.Replace("$account name", bulkEmail.Title);
                                mailRequest.Body = mailRequest.Body.Replace("$Community Name2", bulkEmail.CommunityName);

                                 await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);
                            }
                            await _AccountRepository.UpdateIsSentForBukEmail(bulkEmail);
                        }
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            { 
                return false; 
            }
            
        }


        public async Task<int> SendBulkAccountsEmail(BulkAccountsEmail bulkAccountsEmail)
        {
            bulkAccountsEmail.FillDefaultValues();
            return await _AccountRepository.SendBulkAccountsEmail(bulkAccountsEmail);
        }

        #endregion

        #region "API Functions"
        public async Task<long> Pay(long CollectionReqId, long PayForUserId, decimal Amount, long LoggedInUserId, string Currency)
        {
            long result = _AccountRepository.Pay(CollectionReqId, PayForUserId, Amount, LoggedInUserId, Currency);

            try
            {
                MailRequest mailRequest = new MailRequest();
                var account_sentEmail = GetViewAccountDetails(CollectionReqId);
                var account_Pay = SendAccountPaidEmail(PayForUserId);
                mailRequest.FromUserId = LoggedInUserId;
                mailRequest.ReferenceId = PayForUserId;
                MailSettings mailSettings = _mailService.EmailParameter(MailType.AccountPaid, ref mailRequest);
                foreach (var items in account_Pay.Result)
                {
                    mailRequest.To = items.Email;
                }
                mailRequest.Body = mailRequest.Body.Replace("$community name", account_sentEmail.Result.FirstOrDefault().CommunityName);
                mailRequest.Body = mailRequest.Body.Replace("$Currency", account_sentEmail.Result.FirstOrDefault().Currency);
                mailRequest.Body = mailRequest.Body.Replace("$Community Name1", account_sentEmail.Result.FirstOrDefault().CommunityName);
                mailRequest.Body = mailRequest.Body.Replace("$userfirstname", account_sentEmail.Result.FirstOrDefault().Individual);
                mailRequest.Body = mailRequest.Body.Replace("$Amount", account_sentEmail.Result.FirstOrDefault().Amount.ToString());
                mailRequest.Body = mailRequest.Body.Replace("$expiry date", account_sentEmail.Result.FirstOrDefault().ExpirydateCollection.ToString("dd MMM yyyy"));
                mailRequest.Body = mailRequest.Body.Replace("$collection description", account_sentEmail.Result.FirstOrDefault().Description);
                mailRequest.Body = mailRequest.Body.Replace("$account name", account_sentEmail.Result.FirstOrDefault().Title);
                mailRequest.Body = mailRequest.Body.Replace("$Community Name2", account_sentEmail.Result.FirstOrDefault().CommunityName);
                 _mailService.SaveAndSendMailAsync(mailRequest, mailSettings).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {

            }



            return result;

        }
        public async Task<List<CustomerDetails>> SendAccountPaidEmail(long customerid)
        {
            return await _AccountRepository.SendAccountPaidEmail(customerid);
        }
        public AccountListResponse GetAccounts(long CustomerId, long CommunityId, int IsAllUpcomingOrPassed, long? Collectionid, int PageSize, int PageNumber)
        {
            return _AccountRepository.GetAccounts(CustomerId, CommunityId, IsAllUpcomingOrPassed, Collectionid, PageSize, PageNumber);
        }

        #endregion

    }
}
