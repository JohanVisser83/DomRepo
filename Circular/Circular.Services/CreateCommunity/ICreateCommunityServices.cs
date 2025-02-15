using Circular.Core.Entity;
using Circular.Core.DTOs;

namespace Circular.Services.CreateCommunity
{
    public interface ICreateCommunityServices
    {
        Task<IEnumerable<Country>> GetCountryName();
       
        public Task<int> SaveCommunitySignUpDetails(CommunitySignUp communitySignUp);
        public Task<string> GetCommunityMobileNumber(long communityId);
        Task<IEnumerable<CommunitySignUp>> GetCommunityLogo(string CustomerId);
        Task<int> SaveSubscriptionCommunityInfo(SubscriptionCommunityInfo subscriptionBilling);
        Task<IEnumerable<SubscriptionFeaturesSelectedPlan>> GetFeaturesOrderDetails(long TierId);
    }
}
