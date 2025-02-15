using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Services.CommunityManagement
{
    public interface ICommunityManagementService
    {
        Task <int>AffiliateCode(AffiliatedCodeDetails affiliateCode);
        Task<int> CreatelandingPage(Advertisement advertisement);
        Task<long> DeleteCommunity(long id);
        Task<IEnumerable<AffiliatedCodeDetails>> GetAffiliateCodelist(long id);
        Task<IEnumerable<AffiliateCode>> GetAllAffiliatedCode();
        Task<IEnumerable<HQAllMemberDetails>> GetAllMemberDetails(long communityId, long customerId);
        Task<IEnumerable<CommunityCategory>> GetCommunityCategory();
        Task<IEnumerable<HQCommunitiesList>> GetCommunityMaxMemberlist();
        Task<List<Communities>> GetEditCommunityDetails(long ids);

        Task<List<Communities>> GetEditHalfBakedCommunityDetails(long ids); 
        Task<IEnumerable<CommunitySignUp>> GetHalfBakedCommunityList();
        public Task<QR> GetMemberQRCode(long Customerid);
        Task<IEnumerable<CommunitySignUp>> HalfBakedCommunityMember();
        string HQAddCommunity(string AccountMobileNo, string OrgName, string AccessCode, string PrimaryEmail, 
            string OrgLogo, string DashboardBanner, string Country, long CountryId, string Currency, string currencyCode, 
            string About, string PrimaryMobileNo, string Website, string OrgAddress1, string AffiliateCode);
        Task<long> IsBlockUser(long communityId, long customerId, bool isblocked);
        Task<long> UpdateAffiliateCodeDetails(AffiliatedCodeDetails affiliateCode);
        Task<long> UpdateCommunityInfo(Communities communities);

        
    }
}
