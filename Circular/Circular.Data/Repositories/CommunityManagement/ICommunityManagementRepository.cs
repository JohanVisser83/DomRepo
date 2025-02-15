using Circular.Core.Entity;

namespace Circular.Data.Repositories.CommunityManagement
{
    public interface ICommunityManagementRepository
    {
        Task<int> AffiliateCode(AffiliatedCodeDetails affiliateCode);
        Task<int> CreatelandingPage(Advertisement advertisement);
        Task<long> DeleteCommunity(long id);
        Task<IEnumerable<AffiliatedCodeDetails>> GetAffiliateCodelist(long Id);
        Task<List<AffiliateCode>> GetAllAffiliatedCode();
        Task<IEnumerable<HQAllMemberDetails>> GetAllMemberDetails(long communityId, long customerId);
        Task<IEnumerable<CommunityCategory>> GetCommunityCategory();
        Task<IEnumerable<HQCommunitiesList>> GetCommunityMaxMemberlist();
        Task<List<Communities>> GetEditCommunityDetails(long id);
        Task<List<Communities>> GetEditHalfBakedCommunityDetails(long id);
        Task<IEnumerable<CommunitySignUp>> GetHalfBakedCommunityList();
        public Task<QR> GetMemberQRCode(long customerID);
        Task<IEnumerable<CommunitySignUp>> HalfBakedCommunityMember();
        public  string HQAddCommunity(string AccountMobileNo, string OrgName, string AccessCode, string PrimaryEmail, string OrgLogo, 
            string DashboardBanner, string Country, long CountryId,  string Currency ,string currencyCode, string About, string PrimaryMobileNo, string Website, string OrgAddress1, string AffiliateCode);
        Task<long> IsBlockUser(long communityId, long customerId, bool isblocked);
        Task<long> UpdateAffiliateCodeDetails(AffiliatedCodeDetails affiliateCode);
        Task<long> UpdateCommunityInfo(Communities communities);
    }

}
