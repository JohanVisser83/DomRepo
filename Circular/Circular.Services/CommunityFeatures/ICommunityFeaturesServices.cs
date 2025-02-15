using Circular.Core.Entity;


namespace Circular.Services.CommunityFeatures
{
    public interface ICommunityFeaturesServices
    {
        Task <string> CreateCommunityAppUser(long customerId, long communityId, string? strName, string? strEmail);
        Task<IEnumerable<CommunityAccessType>> GetCommunityAccessTypes();


        Task<IEnumerable<SubscriptionFeatures>> GetCommunityFeatures();
        Task<IEnumerable<SubscriptionTier>> GetCommunitySubsTier(string featurePrice);
        Task<SubscriptionTier> GetCommunitySubsTier(string Plan, string Period);

        Task<IEnumerable<SubscriptionTier>> GetCommunityTierFeatures(long Id);
        Task<IEnumerable<SubscriptionType>> GetSubscriptionType();
        string PostCommunityDetails(string? communityLogo, string? dashboardBanner, string communityName, 
            long membershipType, decimal? membershipAmount, long accessType, string about, string website, 
            string physicalAddress, string? planType, long SubscriptionTierId, long CustomerId, 
            long TransactionId, string URL, string country,long countryid,string currency,
            string currencyToken, string strName);
        Task<int> SaveFeatureDetails(SelectedCommunityFeatures selectedCommunityFeatures);
        Task <int> SaveSelectedFeatureDetails(SubscriptionFeaturesSelectedPlan selectedCommunityFeatures);
        Task<int> SaveVisitor(Visitor visitor);
    }
}
