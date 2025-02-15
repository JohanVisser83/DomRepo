using Circular.Core.Entity;
using Circular.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Data.Repositories.CreateCommunity
{
    public interface ICreateCommunityRepository
    {
        Task<IEnumerable<Country>> GetCountryName();
        public Task<int> SaveCommunitySignUpDetails(CommunitySignUp communitySignUp);
        public Task<string> GetCommunityMobileNumber(long communityId);
        Task<IEnumerable<CommunitySignUp>> GetCommunityLogo(string customerId);
        Task<int> SaveSubscriptionCommunityInfo(SubscriptionCommunityInfo subscriptionBilling);
        Task<IEnumerable<SubscriptionFeaturesSelectedPlan>> GetFeaturesOrderDetails(long TierId);
    }
}
