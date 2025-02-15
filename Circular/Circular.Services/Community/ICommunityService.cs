using Circular.Core.DTOs;
using Circular.Core.Entity;
using MailKit;
using System.Drawing.Printing;
using Tweetinvi.Models;

namespace Circular.Services.Community
{
    public interface ICommunityService
    {
        Task<long> GetCommunityId(string URL);
        Task<string> GetCommunityURL(long Id);

        Task<long> AddIsBlocked(long communityId, long customerId, bool Isblocked);
        Task<long> UpdateIsBlocked(long communityId,long customerId, bool Isblocked);
        Task<IEnumerable<CustomerDetails>> GetCommunityOrganizers(long communityId);
        Task<IEnumerable<CustomerCommunity>?> GetCommunityMembers(long communityId);
        Task<IEnumerable<CustomerDetails>?> GetCommunityMemberDetails(long communityId, int UserTypeId, long CustomerId, int IncludeCurrentCustomer,int showInactive);
        Task<IEnumerable<CustomerGroups>?> GetCommunityGroupMembers(long GroupId);
        Task<int> CommunityAsync(Communities communities);
        Task<Communities?> GetCommunityInfo(long id);
        Task<long> UpdateCommunityInfo(Communities communities);
        Task<int> AddSocialMedia(Communities communities);
        Task<Communities?> GetSocialMedia(long id);
        Task<long> UpdateSocialMedia(Communities communities);
        Task<int> AddStaff(CommunityTeamProfile communityTeamProfile);
        Task<List<CommunityTeamProfile>> GetCommunityStaffAsync(long primaryId);
        Task<List<CommunityTeamProfile>> GetCommunityTeamProfile(CommunityTeamProfile communityTeamProfile);
        Task<int> DeleteCommunityStaffAsync(long id);
        Task<CommunityTeamProfile> GetCommunityStaffById(long Id);
        Task<long> UpdateCommunityStaff(CommunityTeamProfile communityTeamProfile);
        List<CustomerCommunity>? SearchCommunity(string communityName, long? CommunityId,long pagesize, long pagenumber, string search, long customerId);
        Task<List<Customers>> SendEmailSigupUser(long Id);
        Task<List<CustomerDetails>> GetUserName(long Id);
        Task<int> SaveCustomerCommunity(CustomerCommunity customerCommunity);
        Task<IEnumerable<CustomerCommunity>?> CommunityListByCustomerId(long customerId);
        Task<IEnumerable<Features>?> Features (long communityId, long loggedInUserId) ;
        Task<IEnumerable<Features>?> Features_App(long communityId, long loggedInUserId);

        Task<IEnumerable<AdminFeature>?> AdminFeature(long LoggedInUser, long communityId);

        Task<CustomerCommunity> GetFirstCommunityOfCustomerId(long CustomerId);
        Task<List<Groups>> GetCommunityGroups(long communityId);

        Task<List<CustomerDetails>> GetCommunityUser(long CommunityId);
        Task<int> DeleteCustomerCommunity(long communityId, string UserId);
        Task<IEnumerable<CommunityStaffs>?> Staff(long communityId);
        Task<IEnumerable<CommunityTeamProfile>?> CommunityTeamProfile(long communityId);
        Task<IEnumerable<CustomerSubscriptionStatus>> GetcustomerSubscriptionStatus();
        Task<int> ChangeCustomersubScriptionStatus(long userId,int? subscriptionStatusId);
        Task<int> ChangePaymentStatus(long userId, int? subscriptionStatusId);
        Task<IEnumerable<CustomerMembershipPaymentStatus>> GetCustomerMembershipPaymentStatus();

        #region "Jobs"
        Task<long> AddJobPosting(Jobs jobs);
        Task<long> EditJobPosting(Jobs jobs);
        Task<IEnumerable<Jobs>?> GetJobPosting(long BusinessId, long id, long customerId, int jobCategoryId,
            string searchText, long communityId, int pageNumber, int pageSize);
        Task<long> ChangeIsJobApproved(long Id, bool IsApproved, long LoggedInUserId);
        public Task<List<Jobs>> SendEmailJobUser(long Id);
        //public Task<List<Jobs>> GetJobCompanyName(long Id);
        //Task<long> DashboardIsJobApprove(long Id);
        Task<long> DeleteJobPosting(Jobs jobs);
        Task<long> UpdateViewCount(Jobs jobs);
        Task<long> ApplyJob(JobApplication jobs);
        Task<IEnumerable<Jobs>> GetCustomer(long customerId);

        #endregion

        #region "Business"

        Task<int> AddBusinessIndex(CustomerBusinessIndex customerBusinessIndex);
        Task<IEnumerable<CustomerBusinessIndex>?> GetBusinessIndex(long CommunityId, long? id, long? UserId, int BusinessCategoryId,
            string? searchText, int pageNumber, int pageSize, bool IsPendingBusinessOnly);
        Task<long> ChangeIsBusinessApproved(long Id, bool IsBusinessApproved, long LoggedInUserId);
        Task<long> ChangeIsAdmin(long Id, bool IsAdmin, long communityId);
        Task<List<CustomerBusinessIndex>> SendEmailBusinessUser(long Id);
        Task<List<CustomerDetails>> GetCustomerDetails(long Id);
        Task<long> DeleteBusiness(CustomerBusinessIndex business);
        Task<long> EditBusiness(CustomerBusinessIndex business);
        
        #endregion


        Task<int> NewCustomerGroup(Groups groups);
        Task<IEnumerable<Groups>?> GetCustomerGroup(long communityId);
        Task<long> DeleteCustomGroup(long Id);

        public long ActionOnNetwork(long ToId, long FromId, int FriendRequestStatusId,Customers customer);
        List<dynamic> GetCommunityNetwork(long CommunityId, long? LoggedInUserId, string? SearchText,int IsFriend, int pageNumber,int pageSize);
		//linked member
		//Task<int> GetLinkedMember(string Name, long Loggedinuser);
        public Task<List<Fundraiser>> GetFundraisers(long communityid);
       
        public Task<List<Fundraiser>> ViewFundraisersAsync(long Id);
        Task<bool> UploadImageAndFile5Async(long FundraiserId ,string ImagePath);
        Task<bool> UpdateFundraisersAsync(string Title,long Amount ,DateTime ExpiryDate,string PDFLink,string Description,string FormLink,long Id , long Community,long OrganizerId,string ImagePath);
        public Task<List<Fundraiser>> ViewArchivedFundraisersAsync(long Id);
        Task<int> UpdateCollected(long id, bool Iscollected);
        Task<int> DeleteArchivedFundraisersAsync(long Id);
        public FundListResponse GetFundraiserAsync(long CommunityId, long? FundraiserId);
        public long PayFundraiser(long CollectionReqId, long PayForUserId, decimal Amount, long LoggedInUserId, string Currency);
        Task<List<Fundraiser>> GetFundraiserAsync(long CommunityId);
        public Task<List<FundraiserType>> Gettypeoffundraiser();
        Task<bool> SaveNewCompaignAsync(long Communityid,long FundraiserTypeId, string FundraiserTitle, long OrganizerId, decimal ProductAmount, DateTime ExpiryDate, string PDFLink, string Description, string FormHyperlink,string ImagePath);
        Task<int> ArchiveFundraisersAsync(long Id);
        public Task<List<Fundraiser>> GetArchiveFundraisers(long communityid);
        Task<int> AddUserInGroup(CustomerGroups customerGroups);
        Task<IEnumerable<dynamic>> ShowCustomGroupList(long id);
       // public Task<List<FundraiserCollection>> GetViewPayments(long Id);
        public Task<long> DeleteGroupUsers(long Id);
        Task<IEnumerable<dynamic>> GetFundhubActiveOrders(long loggedinuser, long communityId, bool isCollected);
        public Task<List<FundraiserCollection>> GetViewPayments(long loggedinuserid);
        Task<int> DeleteCircleInfoItem2Async(long Id);
        Task<bool> SaveAmountCompaignAsync(long Communityid, long FundraiserTypeId, string FundraiserTitle, long OrganizerId, decimal ProductAmount, string PDFLink, string Description, string ImagePath, DateTime ExpiryDate);
        Task<IEnumerable<CommunityDetails>> GetCommunities(long CommunityId, string search, long pageNumber, long pageSize);
        public int  CheckValidAccessCode(string? accessCode, long communityId);
        Task<long> SaveNewCustomerCommunity(CustomerCommunity customerCommunity);
        Task <int> SaveCommunityAccessRequest(CommunityAccessRequests communityAccessRequests);
        Task <int> UpdateuserDetails(CommunityAccessDTO communityAccessCodeDTO);
        Task<List<MembershipType>> GetMasterMembershipTypeAsync();
        Task<List<CommunityAccessType>> GetMasterAccessTypeAsync(long SubscriptionType);

        Task<dynamic> updatemembersubscription(long CommunityId, decimal Price, long MembershipType, string CommunityUrl, long AccessType);

        public Task<List<CommunityAccessRequests>> GetRequest(long communityId);
        Task<int> UpdateIsStatus(long Id, long StatusId ,long Communityid);
        public Task<List<CommunityAccessRequests>> GetRequestEmail(long communityId, long Id);
        public Task<bool> CancelCommunitySubscriptions();
        Task<List<Groups>> GetCommunityGroupList(long communityId);
        Task<IEnumerable<Customers>> GetTotalMembers();
    }

}
