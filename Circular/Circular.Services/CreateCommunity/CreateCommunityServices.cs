using Circular.Core.Entity;
using Circular.Data.Repositories.CreateCommunity;
using Circular.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Services.CreateCommunity
{
    public  class CreateCommunityServices :  ICreateCommunityServices
    {
        private readonly ICreateCommunityRepository _CreateCommunityRepository;

        public CreateCommunityServices(ICreateCommunityRepository createCommunityRepository)
        {
            _CreateCommunityRepository = createCommunityRepository;

        }

        public async Task<IEnumerable<Country>> GetCountryName()
        {
            return await _CreateCommunityRepository.GetCountryName();
        }

        public async Task<int> SaveCommunitySignUpDetails(CommunitySignUp communitySignUp)
        {
            communitySignUp.FillDefaultValues();
            return  await _CreateCommunityRepository.SaveCommunitySignUpDetails(communitySignUp);
        }
        public Task<string> GetCommunityMobileNumber(long communityId)
        {
            return  _CreateCommunityRepository.GetCommunityMobileNumber(communityId);
        }

        public Task<IEnumerable<CommunitySignUp>> GetCommunityLogo(string customerId)
        {
            return _CreateCommunityRepository.GetCommunityLogo(customerId);
        }

        public async  Task<int> SaveSubscriptionCommunityInfo(SubscriptionCommunityInfo subscriptionBilling)
        {
            subscriptionBilling.FillDefaultValues();
            return await _CreateCommunityRepository.SaveSubscriptionCommunityInfo(subscriptionBilling);
        }

        public async Task<IEnumerable<SubscriptionFeaturesSelectedPlan>> GetFeaturesOrderDetails(long TierId)
        {
            return await _CreateCommunityRepository.GetFeaturesOrderDetails(TierId);
        }
    }
}
