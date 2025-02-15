using Circular.Core.Entity;
using Circular.Data.Repositories.CommunityFeatures;


namespace Circular.Services.CommunityFeatures
{
    public  class CommunityFeaturesServices : ICommunityFeaturesServices
    {
        private readonly ICommunityFeaturesRepositories _communityFeaturesRepositories;

        public CommunityFeaturesServices(ICommunityFeaturesRepositories communityFeaturesRepositories)
        {
            _communityFeaturesRepositories = communityFeaturesRepositories;
        }

        public async Task<string> CreateCommunityAppUser(long customerId, long communityId, string? strName, string? strEmail)
        {
            return await  _communityFeaturesRepositories.CreateCommunityAppUser(customerId, communityId, strName, strEmail);
        }

        public async Task<IEnumerable<CommunityAccessType>> GetCommunityAccessTypes()
        {
            return await _communityFeaturesRepositories.GetCommunityAccessTypes();
        }

        public async Task<IEnumerable<SubscriptionFeatures>> GetCommunityFeatures()
        {
            return await _communityFeaturesRepositories.GetCommunityFeatures();
        }

        public async Task<IEnumerable<SubscriptionTier>> GetCommunitySubsTier(string featurePrice)
        {
            return await _communityFeaturesRepositories.GetCommunitySubsTier(featurePrice);
        }
        public async Task<SubscriptionTier> GetCommunitySubsTier(string Plan, string Period)
        {
            return await _communityFeaturesRepositories.GetCommunitySubsTier( Plan,  Period);
        }
        
        public async Task<IEnumerable<SubscriptionTier>> GetCommunityTierFeatures(long Id)
        {
            return await _communityFeaturesRepositories.GetCommunityTierFeatures(Id);
        }

        public async  Task<IEnumerable<SubscriptionType>> GetSubscriptionType()
        {
            return await _communityFeaturesRepositories.GetSubscriptionType();
        }

        public string PostCommunityDetails(string? communityLogo, string? dashboardBanner, string communityName, 
            long membershipType, decimal? membershipAmount, long accessType, string about, string website, 
            string physicalAddress, string? planType, long SubscriptionTierId, long CustomerId, long TransactionId, string URL, string country, long countryid, string currency, string currencyToken, string strName)
        {
            return _communityFeaturesRepositories.PostCommunityDetails(communityLogo, dashboardBanner, communityName,
                membershipType, membershipAmount, accessType, about, website, physicalAddress,planType,  SubscriptionTierId,  CustomerId,  TransactionId, URL,  country, countryid, currency, currencyToken, strName);
        }

        public async Task<int> SaveFeatureDetails(SelectedCommunityFeatures selectedCommunityFeatures)
        {
            return await _communityFeaturesRepositories.SaveFeatureDetails(selectedCommunityFeatures);
        }

        public async Task<int> SaveSelectedFeatureDetails(SubscriptionFeaturesSelectedPlan selectedCommunityFeatures)
        {
            selectedCommunityFeatures.FillDefaultValues();
            return await _communityFeaturesRepositories.SaveSelectedFeatureDetails(selectedCommunityFeatures);
        }

        public async  Task<int> SaveVisitor(Visitor visitor)
        {
           visitor.FillDefaultValues();

            return await _communityFeaturesRepositories.SaveVisitor(visitor);
        }
    }
}
