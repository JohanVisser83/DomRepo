using Circular.Core.DTOs;
using Circular.Core.Entity;
namespace Circular.Services.User
{
    public interface ICustomerService
    {
        #region "Customer"
        Task<int> DeActivateMyAccount(string UserId);
        Task<long> Save(Customers customers, bool IsCreateCustomerDetails);
        Customers UpdatePasscode(long id, string passcode, bool isRelatedEntityFill,string Name,bool sendWelcomeEmail, string email);
        bool VerifyPasscode(long id, string passcode);
        Customers getcustomerByUserId(Guid usercode, bool isRelatedEntityFill);
        Customers getcustomerbyId(long id, bool isRelatedEntityFill);
        Customers getCommunityMemberDetailsById(long id);
        Customers getCommunityMemberDetailsByIdHQ(long id);
        Customers getcustomerByUserName(string userName, bool isRelatedEntityFill);

        List<NewsFeeds?> GetNewFeeds(long customerId, int IsArchiveUnArchiveOrAll, long Feedid);
        Task<int> LikeArticle(LikedFeeds likedFeeds);
        public int CheckValidEmail(string email);

       public bool CheckIfExistingOwnerId (long UserId);    
        #endregion

        #region "Customer Details"

        Task<int> SaveDetails(CustomerDetails customerDetails);
        Customers UpdateUserType(long CustomerId, long userTypeId, long modifiedBy);
        Customers UpdateCustomerBasicDetails(CustomerDetails customerDetail);
        Task<int> SaveDeviceDetails(CustomerDevices userDevices);

        #endregion

        #region "Linked Members"
        public Task<IEnumerable<dynamic>?> GetLinkedMembers(string UserId);
        public Task<long> AddLinkedMembers(LinkedMembers linkedMember, Customers currentCustomer);
        public Task<int> RemoveLinkedMembers(LinkedMembers linkedMember);
        public Task<int> ConfirmLinkedMembers(LinkedMembers linkedMember, Customers currentCustomer);


        #endregion

        Task<string> SendOTPMail(string Email, string otp);
        Task<string> SendOTPMailForSubscriptionPortal(string Email, string otp);

        CustomerDetails GetCustomerDetails(long Id, string Email); 

        public Task<int> sendOTP(CustomerPasswordChangeRequest data);
        public Task<int> GetVerifyOTP(string otp, long CustomerId);
        public Task<List<ActiveOTPDetails>> GetOTPUser();
        List<Advertisement?> GetAdvertisement(long communityId, long advertisementId);
        Task<long> UpdateArticleViewCount(ArticleViews viewsCount);

        Task<string> SendWithdrawalOTPMail(string Email, string otp);
        public int CheckValidUser(string mobile);
        Task<CustomerDetails> CheckValidHQAdminEmail(string userName);
    }

}
