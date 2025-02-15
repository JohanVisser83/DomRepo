using AutoMapper.Internal;
using Circular.Core.Entity;
using Circular.Data.Repositories.User;
using Circular.Framework.Middleware.Emailer;
using Circular.Framework.Notifications;
using Circular.Framework.Utility;
using Circular.Services.Email;
using Circular.Services.Notifications;
using Microsoft.Extensions.Configuration;


namespace Circular.Services.User
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;
        IMailService _mailService;
        private readonly IHelper _helper;
        private readonly IConfiguration _config;
    




        public CustomerService(ICustomerRepository customerRepository,
            IConfiguration configuration, INotificationService notificationService, IMailService mailService, IHelper helper
            )
        {

            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
          
            _config = configuration;
        }

        #region "Customers"

        public Customers getcustomerByUserName(string userName, bool isRelatedEntityFill)
        {
            return _customerRepository.getcustomerByUserName(userName, isRelatedEntityFill);
        }

        public async Task<long> Save(Customers customer, bool IsCreateCustomerDetails)
        {
            customer.FillInitialValues("").FillDefaultValues();
            var customerId = await _customerRepository.Save(customer);
            if (IsCreateCustomerDetails)
            {
                await SaveDetails(new CustomerDetails() { CustomerId = customerId, CustomerTypeId = 103 });
            }
            return customerId;
        }

       

        public Customers UpdatePasscode(long id, string passcode, bool isRelatedEntityFill,string Name,bool sendWelcomeEmail, string email )
        {
            var result = _customerRepository.UpdatePasscode(id, passcode, isRelatedEntityFill);
            if (result != null)
            {
              if(sendWelcomeEmail)
                { 
                    MailRequest mailRequest = new MailRequest();
                    mailRequest.FromUserId = id;
                    mailRequest.To = email;
                    mailRequest.ReferenceId = id;
                    MailSettings mailSettings = _mailService.EmailParameter(MailType.Welcome_Message, ref mailRequest);

                    string body = mailRequest.Body;
                    string[] PlaceHolders = { "$FirstName" };
                    string[] Values = { Name };
                    if (!string.IsNullOrEmpty(body) && (PlaceHolders.Length == Values.Length))
                    {
                        for (int index = 0; index < PlaceHolders.Length; index++)
                            body = body.Replace(PlaceHolders[index], Values[index]);
                    }
                    mailRequest.Body = body;
                    var result1 = _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);
                  
                }
            }
            return result;
        }

        public bool VerifyPasscode(long id, string passcode)
        {
            return _customerRepository.VerifyPasscode(id, passcode);
        }

        public Customers getcustomerByUserId(Guid usercode, bool isRelatedEntityFill)
        {
            return _customerRepository.getcustomerByUserId(usercode, isRelatedEntityFill);
        }




        public bool CheckIfExistingOwnerId(long userId)
        {
            return _customerRepository.CheckIfExistingOwnerId(userId);
        }

        public Customers getcustomerbyId(long id, bool isRelatedEntityFill)
        {
            return _customerRepository.getcustomerbyId(id, isRelatedEntityFill);

        }
        public Customers getCommunityMemberDetailsById(long id)
        {
            return _customerRepository.getCommunityMemberDetailsById(id);

        }

        public Customers getCommunityMemberDetailsByIdHQ(long id)
        {
            return _customerRepository.getCommunityMemberDetailsByIdHQ(id);

        }

        public async Task<int> DeActivateMyAccount(string UserId)
        {
            return await _customerRepository.DeActivateMyAccount(UserId);
        }

        public List<NewsFeeds?> GetNewFeeds(long customerId, int IsArchiveUnArchiveOrAll, long Feedid)
        {
            return _customerRepository.GetNewFeeds(customerId, IsArchiveUnArchiveOrAll, Feedid);
        }

        public async Task<int> LikeArticle(LikedFeeds likedFeeds)
        {
            likedFeeds.FillDefaultValues();
            return await _customerRepository.LikeArticle(likedFeeds);
        }

        #endregion

        #region "CustomerDetails"
        public async Task<int> SaveDetails(CustomerDetails customerDetails)
        {
            customerDetails.FillDefaultValues();
            customerDetails.SubscriptionStatusId = 101;
            customerDetails.MembershipTypeId = 101;
            customerDetails.PaymentStatusId = 102;
            return await _customerRepository.SaveDetails(customerDetails);
        }

        public Customers UpdateUserType(long CustomerId, long userTypeId, long modifiedBy)
        {
            return _customerRepository.UpdateUserType(CustomerId, userTypeId, modifiedBy);
        }

        public Customers UpdateCustomerBasicDetails(CustomerDetails customerDetail)
        {
            return _customerRepository.UpdateCustomerBasicDetails(customerDetail);
        }

        public async Task<int> SaveDeviceDetails(CustomerDevices userDevices)
        {
            userDevices.FillDefaultValues();
            return await _customerRepository.SaveDeviceDetails(userDevices);
        }


        #endregion

        #region "Safety"    


        #endregion


        #region "Linked Members"


        public async Task<IEnumerable<dynamic>?> GetLinkedMembers(string UserId)
        {
            return await _customerRepository.GetLinkedMembers(UserId);
        }

        public async Task<long> AddLinkedMembers(LinkedMembers linkedMember, Customers currentCustomer)
        {
            long result = await _customerRepository.AddLinkedMembers(linkedMember);
            if (result > 5)
            {
                string senderName = currentCustomer.CustomerDetails?.FirstName + ' ' + currentCustomer.CustomerDetails?.LastName;
                _notificationService.Notify(Framework.Notifications.NotificationTypes.Linking_Request_Received,
                    NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", linkedMember.ToCustId.ToString()),
                senderName, "You received a new linking request from " + senderName, result, 0, false, "",
                linkedMember.FromCustId, currentCustomer.PrimaryCommunity.CommunityId ?? 0, "", linkedMember.ToCustId);

                _notificationService.Notify(Framework.Notifications.NotificationTypes.Linking_Request_Sent,
                    NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", linkedMember.FromCustId.ToString()),
                senderName, "Your linking request has been sent ", result, 0, false, "",
                linkedMember.FromCustId, currentCustomer.PrimaryCommunity.CommunityId ?? 0, "", linkedMember.FromCustId);
            }
            return result;
        }

        public async Task<int> RemoveLinkedMembers(LinkedMembers linkedMember)
        {
            return await _customerRepository.RemoveLinkedMembers(linkedMember);
        }

        public async Task<int> ConfirmLinkedMembers(LinkedMembers linkedMember, Customers currentCustomer)
        {
            int result = await _customerRepository.ConfirmLinkedMembers(linkedMember);
            if (result <= 0)
                return result;

            string senderName = currentCustomer.CustomerDetails?.FirstName + ' ' + currentCustomer.CustomerDetails?.LastName;
            string msg = "";

            if (linkedMember.LinkingStatusId == 101)
            {
                msg = "Your linking request has been approved";
                _notificationService.Notify(Framework.Notifications.NotificationTypes.Linking_Request_Accepted,
                NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", linkedMember.FromCustId.ToString()),
                senderName, msg, result, 0, false, "",
                linkedMember.ToCustId, currentCustomer.PrimaryCommunity.CommunityId ?? 0, "", linkedMember.FromCustId);
            }
            else if (linkedMember.LinkingStatusId == 102)
            {
                msg = "Your linking request has been rejected";
                _notificationService.Notify(Framework.Notifications.NotificationTypes.Linking_Request_Rejected,
                NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", linkedMember.FromCustId.ToString()),
                senderName, msg, result, 0, false, "",
                linkedMember.ToCustId, currentCustomer.PrimaryCommunity.CommunityId ?? 0, "", linkedMember.FromCustId);
            }
            else if (linkedMember.LinkingStatusId == 103)
            {
                msg = "Your linking request has been reported";
                _notificationService.Notify(Framework.Notifications.NotificationTypes.Linking_Request_Reported,
                NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", linkedMember.FromCustId.ToString()),
                senderName, msg, result, 0, false, "",
                linkedMember.ToCustId, currentCustomer.PrimaryCommunity.CommunityId ?? 0, "", linkedMember.FromCustId);
            }
            return result;

        }

        #endregion


        public int CheckValidEmail(string email)
        {
            return _customerRepository.CheckValidEmail(email);
        }



        public async Task<string> SendOTPMail(string Email, string otp)
        {
            var details = _customerRepository.CheckValidEmail(Email);
            var result = _customerRepository.GetCustomerDetails(details, Email);
            var redirection = _config["CommunityPortal"];
            MailRequest mailRequest = new MailRequest();
            MailSettings mailSettings = _mailService.EmailParameter(MailType.Login_OTP, ref mailRequest);
            mailRequest.To = Email;
            mailRequest.Body = mailRequest.Body.Replace("$Otp", otp);
            mailRequest.Body = mailRequest.Body.Replace("$Recipient's Name", result.FirstName);
            mailRequest.Body = mailRequest.Body.Replace("$redirect", redirection);
            var mail = await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);
            return mail.ToString().Trim();

        }


        public async Task<string> SendWithdrawalOTPMail(string Email, string otp)
        {
            var details = _customerRepository.CheckValidEmail(Email);
            var result = _customerRepository.GetCustomerDetails(details, Email);
            var redirection = _config["CommunityPortal"];
            MailRequest mailRequest = new MailRequest();
            MailSettings mailSettings = _mailService.EmailParameter(MailType.Withdrawal_OTP_Email, ref mailRequest);
            mailRequest.To = Email;
            mailRequest.Body = mailRequest.Body.Replace("$Otp", otp);
            mailRequest.Body = mailRequest.Body.Replace("$Recipient's Name", result.FirstName);
            mailRequest.Body = mailRequest.Body.Replace("$redirect", redirection);
            var mail = await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);
            return mail.ToString().Trim();

        }

        public async Task<string> SendOTPMailForSubscriptionPortal(string Email, string otp)
        {
            MailRequest mailRequest = new MailRequest();
            MailSettings mailSettings = _mailService.EmailParameter(MailType.Login_OTP, ref mailRequest);
            mailRequest.To = Email;
            mailRequest.Body = mailRequest.Body.Replace("$Otp", otp);
            mailRequest.Body = mailRequest.Body.Replace("$Recipient's Name", "There");
            var mail = await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);
            return mail.ToString().Trim();
        }

        public CustomerDetails GetCustomerDetails(long Id, string Email)
        {
            var details = _customerRepository.GetCustomerDetails(Id, Email);
            return details;
        }



        public Task<int> sendOTP(CustomerPasswordChangeRequest data)
        {
            throw new NotImplementedException();
        }


        public Task<int> GetVerifyOTP(string PasswordActivationCode, long CustomerId)
        {
            return _customerRepository.GetVerifyOTP(PasswordActivationCode, CustomerId);
        }

        public async Task<List<ActiveOTPDetails>> GetOTPUser()
        {
            return await _customerRepository.GetOTPUser();
        }

        public List<Advertisement?> GetAdvertisement(long CommunityId, long advertisementId)
        {
            return _customerRepository.GetAdvertisement(CommunityId,  advertisementId);
        }

        

        public async Task<long> UpdateArticleViewCount(ArticleViews articleViews)
        {
            articleViews.FillDefaultValues();

            return await _customerRepository.UpdateArticleViewCount(articleViews);
        }

        public  int CheckValidUser(string mobile)
        {
            return _customerRepository.CheckValidUser(mobile);
        }

        public async Task<CustomerDetails> CheckValidHQAdminEmail(string userName)
        {
            return await _customerRepository.CheckValidHQAdminEmail(userName);
        }
    }
}
