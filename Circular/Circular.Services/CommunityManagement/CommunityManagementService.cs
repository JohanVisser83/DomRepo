using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Data.Repositories.CommunityManagement;

namespace Circular.Services.CommunityManagement
{
    public class CommunityManagementService : ICommunityManagementService
    {
        private readonly ICommunityManagementRepository _communityManagementRepository;

        public CommunityManagementService(ICommunityManagementRepository CommunityManagementRepository)
        {
            _communityManagementRepository = CommunityManagementRepository;
        }


        public string HQAddCommunity(string AccountMobileNo, string OrgName, string AccessCode, string PrimaryEmail, string OrgLogo, string DashboardBanner, string Country, 
            long CountryId, string Currency,string currencyCode, string About, string PrimaryMobileNo, string Website, string OrgAddress1, string AffiliateCode)
        {
            
            string result =  _communityManagementRepository.HQAddCommunity(AccountMobileNo, OrgName, AccessCode, PrimaryEmail, OrgLogo, DashboardBanner, 
                Country, CountryId, Currency,currencyCode, About, PrimaryMobileNo, Website, OrgAddress1, AffiliateCode);
            return result;
        }


        

        public async Task<int> CreatelandingPage(Advertisement advertisement)
        {
            advertisement.FillDefaultValues();
            int result = await _communityManagementRepository.CreatelandingPage(advertisement);
            return result;
        }

        public async Task<IEnumerable<CommunityCategory>> GetCommunityCategory()
        {
            return await _communityManagementRepository.GetCommunityCategory();
        }

        public async Task<List<Communities>> GetEditCommunityDetails(long Id)
        {
            return await _communityManagementRepository.GetEditCommunityDetails(Id);
        }

        public async Task<IEnumerable<CommunitySignUp>>  GetHalfBakedCommunityList()
        {
            return await _communityManagementRepository.GetHalfBakedCommunityList();
        }
        
        public async Task<List<Communities>> GetEditHalfBakedCommunityDetails(long Id)
        {
            return await _communityManagementRepository.GetEditHalfBakedCommunityDetails(Id);
        }

        public async Task<long> UpdateCommunityInfo(Communities communities)
        {
            communities.FillDefaultValues();
            return await _communityManagementRepository.UpdateCommunityInfo(communities);
        }

        public async Task<IEnumerable<CommunitySignUp>> HalfBakedCommunityMember()
        {
            return await _communityManagementRepository.HalfBakedCommunityMember();
        }

        public async Task<IEnumerable<AffiliateCode>> GetAllAffiliatedCode()
        {
            return await _communityManagementRepository.GetAllAffiliatedCode();
        }

        public async Task<IEnumerable<AffiliatedCodeDetails>> GetAffiliateCodelist(long Id)
        {
            return await _communityManagementRepository.GetAffiliateCodelist(Id);
        }

       
        public async Task<int> AffiliateCode(AffiliatedCodeDetails affiliateCode)
        {
            affiliateCode.FillDefaultValues();
            int result = await _communityManagementRepository.AffiliateCode(affiliateCode);
            return result;
        }

        public async Task<long> DeleteCommunity(long id)
        {
            long result = await _communityManagementRepository.DeleteCommunity(id);
            return result;  
        }

        public async Task<IEnumerable<HQAllMemberDetails>> GetAllMemberDetails(long communityId, long customerId)
        {
            return await _communityManagementRepository.GetAllMemberDetails(communityId, customerId);
        }

        public async Task<long> UpdateAffiliateCodeDetails(AffiliatedCodeDetails affiliateCode)
        {
            return await _communityManagementRepository.UpdateAffiliateCodeDetails(affiliateCode);
        }

        public async Task<IEnumerable<HQCommunitiesList>> GetCommunityMaxMemberlist()
        {
            return await _communityManagementRepository.GetCommunityMaxMemberlist();
        }

        public async Task<QR> GetMemberQRCode(long CustomerId)
        {
            return await _communityManagementRepository.GetMemberQRCode(CustomerId);
        }

        public async Task<long> IsBlockUser(long communityId, long customerId, bool isblocked)
        {
            return await _communityManagementRepository.IsBlockUser(communityId, customerId, isblocked);
        }

       
    }
}
