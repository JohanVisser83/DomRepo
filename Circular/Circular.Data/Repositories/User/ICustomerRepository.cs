using Circular.Core.Entity;
using System.Threading.Tasks;

namespace Circular.Data.Repositories.User
{
    public interface ICustomerRepository
    {
        #region "Customer"
        public Task<int> DeActivateMyAccount(string UserId);
        Customers UpdatePasscode(long id, string passcode, bool isRelatedEntityFill);
        bool VerifyPasscode(long id, string passcode);
        Task<long> Save(Customers customers);
        Customers? getcustomerByUserId(Guid usercode, bool isRelatedEntityFill);
        Customers getcustomerbyId(long id, bool isRelatedEntityFill);
        Customers getCommunityMemberDetailsById(long id);
        Customers getCommunityMemberDetailsByIdHQ(long id);
        Customers getcustomerByUserName(string userName, bool isRelatedEntityFill);
        List<NewsFeeds?> GetNewFeeds(long customerId, int IsArchiveUnArchiveOrAll, long Feedid);
        Task<int> LikeArticle(LikedFeeds likedFeeds);

        #endregion

        #region "Customer Details"
        Task<int> SaveDetails(CustomerDetails customerDetails);
        Customers UpdateUserType(long CustomerId, long userTypeId, long modifiedBy);
        int GetCustomerDetailsIdUsingCustomerId(long customerId);
        Customers UpdateCustomerBasicDetails(CustomerDetails customerDetail);
        Task<int> SaveDeviceDetails(CustomerDevices userDevices);

        #endregion

		#region "linked Members"
		public Task<IEnumerable<dynamic>?> GetLinkedMembers(string UserId);
        public Task<long> AddLinkedMembers(LinkedMembers linkedMember);
        public Task<int> RemoveLinkedMembers(LinkedMembers linkedMember);
        public Task<int> ConfirmLinkedMembers(LinkedMembers linkedMember);


        #endregion

        
        public int CheckValidEmail(string Email);
        CustomerDetails GetCustomerDetails(long Id, string Email);
        public Task<int> sendOTP(CustomerPasswordChangeRequest data);
        public Task<int> GetVerifyOTP(string PasswordActivationCode,long CustomerId);
        SponsorInformation GetSponsorInformation(long communityId);

        public  bool CheckIfExistingOwnerId(long userId);
        Task<List<ActiveOTPDetails>> GetOTPUser();
        List<Advertisement?> GetAdvertisement(long communityId, long advertisementId);
        Task<long> UpdateArticleViewCount(ArticleViews articleViews);
        public int CheckValidUser(string mobile);
        Task<CustomerDetails> CheckValidHQAdminEmail(string userName);
    }
}
