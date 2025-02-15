using Circular.Core.Entity;
using RepoDb;
using Microsoft.Data.SqlClient;
using FirebaseAdmin.Messaging;
using NLog.Layouts;
using System.Diagnostics.Metrics;
using System.Net.NetworkInformation;


namespace Circular.Data.Repositories.CommunityFeatures
{
    public class CommunityFeaturesRepositories : DbRepository<SqlConnection>, ICommunityFeaturesRepositories
    {
        public CommunityFeaturesRepositories(string connectionString) : base(connectionString)
        {

        }

        public async Task<string> CreateCommunityAppUser(long customerId, long communityId, string? strName, string? strEmail)
        {
            string strCommand = "exec [dbo].[Usp_CreateCommunityAppUser]" + " " + customerId + "," + communityId + ",'" + strName + "','" + strEmail + "';";
            string result = ((string)ExecuteScalar(strCommand));
            return result;
        }
        public async  Task<IEnumerable<CommunityAccessType>> GetCommunityAccessTypes()
        {
            var results = QueryAll<CommunityAccessType>().Where(c => c.IsActive == true).ToList();
            return results;
        }
        public async Task<IEnumerable<SubscriptionFeatures>> GetCommunityFeatures()
        {
            var _featureName = QueryAll<SubscriptionFeatures>().Where(e => e.IsActive == true ).ToList();
            return _featureName;
        }

        public async  Task<IEnumerable<SubscriptionTier>> GetCommunitySubsTier(string featurePrice)
        {
            var result = QueryAll<SubscriptionTier>().Where(e => e.IsActive == true && e.Code == featurePrice).ToList();
            return result;
        }
        public async Task<SubscriptionTier> GetCommunitySubsTier(string Plan, string Period)
        {
            var result = QueryAll<SubscriptionTier>().Where(e => e.IsActive == true && e.PlanType == Period && e.Code == Plan).ToList().FirstOrDefault();
            return result;
        }
        
        public async Task<IEnumerable<SubscriptionTier>> GetCommunityTierFeatures(long Id)
        {
            var startsUpsFeatures = QueryAll<SubscriptionTier>().Where(s =>s.IsActive == true).ToList();
            return startsUpsFeatures;
        }

        public async Task<IEnumerable<SubscriptionType>> GetSubscriptionType()
        {
            var results =  QueryAll<SubscriptionType>().Where(st => st.IsActive == true).ToList();
            return results;
        }

        public string PostCommunityDetails(string? communityLogo, string? dashboardBanner, string communityName, 
            long membershipType, decimal? membershipAmount, long accessType, string about, string website, 
            string physicalAddress, string? planType, long SubscriptionTierId, long CustomerId, long TransactionId, string URL, string country, long countryid, string currency, string currencyToken, string strName)
        {
            try
            {
                if(membershipAmount is null)
                    membershipAmount = 0;
                string strCommand = "exec [dbo].[Usp_SaveCommunitySetupInfo]" + " '" + communityLogo + "','" + dashboardBanner + "','" + communityName.Replace("'", "''") + "'," + membershipType + "," + membershipAmount + "," + accessType + ",'"  + about.Replace("'", "''") + "','" + website + "','" + physicalAddress + "','" + planType + "'," + SubscriptionTierId.ToString() + "," + CustomerId.ToString() + "," + TransactionId.ToString() + ",'" + URL + "','" + country + "'," + countryid.ToString() + ",'" + currency + "','"  + currencyToken + "','" + strName +  "';"; 
                string result = (string)(ExecuteScalar(strCommand));
                return result;
             
            }
            catch(Exception ex)
            {
                return "";
            }

             
        }

        public async Task<int> SaveFeatureDetails(SelectedCommunityFeatures selectedCommunityFeatures)
        {
            var key = 0;
            foreach (FeatureSubscriptionsFee fsf in selectedCommunityFeatures.featureSubscriptionsFees)
            {
                fsf.FillDefaultValues();
                key = await InsertAsync<FeatureSubscriptionsFee, int>(fsf);
            }

            CommunitySignUp communitySignUp = QueryAll<CommunitySignUp>().Where(e => e.IsActive == true && e.Id==selectedCommunityFeatures.CommunityId).FirstOrDefault();
            communitySignUp.MemberCount = selectedCommunityFeatures.members;
            communitySignUp.MonthlySubsFee = selectedCommunityFeatures.monthlysubscription;
            communitySignUp.MonthlySupportsFee = selectedCommunityFeatures.addons;
            communitySignUp.OnceOffFee = selectedCommunityFeatures.onceOff;
            communitySignUp.TotalMonthlySubsFee = selectedCommunityFeatures.Totalmonthlysubscription;
            var fields = Field.Parse<CommunitySignUp>(e => new
            {
                e.MemberCount,
                e.MonthlySubsFee,
                e.MonthlySupportsFee,e.OnceOffFee,e.TotalMonthlySubsFee
            });
            var result = Update<CommunitySignUp>(entity: communitySignUp, fields: fields);
            return key;
        }

        public async Task<int> SaveSelectedFeatureDetails(SubscriptionFeaturesSelectedPlan selectedCommunityFeatures)
        {           
            try
            {
                var key = await InsertAsync<SubscriptionFeaturesSelectedPlan, int>(selectedCommunityFeatures);
                return key;
            }
            catch (Exception ex)
            {
                return 0;
            }
            
        }

        public async Task<int> SaveVisitor(Visitor visitor)
        {
                var key = await InsertAsync<Visitor, int>(visitor);
                return key;
        }
    }
}
