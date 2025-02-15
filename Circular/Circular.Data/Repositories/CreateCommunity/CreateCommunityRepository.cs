using Circular.Data.Repositories.Message;
using RepoDb;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Circular.Core.Entity;
using Circular.Core.DTOs;
using FirebaseAdmin.Messaging;
using RepoDb.Extensions;

namespace Circular.Data.Repositories.CreateCommunity
{
    public class CreateCommunityRepository : DbRepository<SqlConnection>, ICreateCommunityRepository
    {
        public CreateCommunityRepository(string connectionString) : base(connectionString)
        {

        }


        public async Task<IEnumerable<Country>> GetCountryName()
        {
            var countryName = QueryAll<Country>().Where(e => e.IsActive == true).ToList();
            return countryName;
        }



        public async Task<int> SaveCommunitySignUpDetails(CommunitySignUp communitySignUp)
        {
            try
            {
                var result = await InsertAsync<CommunitySignUp, int>(communitySignUp);
                return result;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }

        public async Task<string> GetCommunityMobileNumber(long communityId)
        {
            try
            {
                var MobileNumber = QueryAll<CommunitySignUp>().Where(e => e.IsActive == true && e.Id == communityId).FirstOrDefault().Mobile;
                return MobileNumber;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public async Task<IEnumerable<CommunitySignUp>> GetCommunityLogo(string CustomerId)
        {
            var communityLogo = QueryAll<CommunitySignUp>().Where(e => e.CustomerId == Convert.ToInt64(CustomerId)).ToList();
            return communityLogo;
        }

        public async Task<int> SaveSubscriptionCommunityInfo(SubscriptionCommunityInfo subscriptionBilling)
        {
            try
            {
                var result = await InsertAsync<SubscriptionCommunityInfo, int>(subscriptionBilling);
                return result;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }

        public async Task<IEnumerable<SubscriptionFeaturesSelectedPlan>> GetFeaturesOrderDetails(long TierId)
        {
           var orderdetails = QueryAll<SubscriptionFeaturesSelectedPlan>().Where(or => or.Id == TierId && or.IsActive ==  true).ToList();
            return orderdetails;
        }
    }
}
