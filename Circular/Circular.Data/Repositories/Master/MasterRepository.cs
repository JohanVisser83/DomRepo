using RepoDb;
using Circular.Core.Entity;
using Microsoft.Data.SqlClient;
using Circular.Core.DTOs;
using Circular.Mapper;

namespace Circular.Data.Repositories.Home
{
    public class MasterRepository : DbRepository<SqlConnection>, IMasterRepository
    {
        public MasterRepository(string connectionString) : base(connectionString)
        {

        }

        public async Task<int> CreateAsync(MasterEntity entity)
        {
            return 0;
        }

        public async Task<int> DeleteAsync(MasterEntity entity)
        {

            return 0;
        }

        public async Task<IEnumerable<MasterEntity>?> GetAllAsync()
        {
            return null;
        }

        public async Task<MasterEntity?> GetAsync(int id)
        {

            return null;

        }

        public async Task<IEnumerable<MasterEntity>?> GetAllAsync(string masterType,
            bool allRecords, long customerId)
        {
            masterType = masterType.ToLower();
            IEnumerable<MasterEntity> masterEntities = null;
            CustomerSettings customerSettings = null;
            DateTime lastfetchedDate = DateTime.Now.AddYears(-10);

            if (masterType == "AnswerType".ToLower())
                masterEntities = await QueryAllAsync<AnswerType>();
            else if (masterType == "VoucherCompany".ToLower())
                masterEntities = await QueryAllAsync<VoucherCompany>();
            else if (masterType == "UserType".ToLower())
                masterEntities = await QueryAllAsync<UserType>();
            else if (masterType == "PrivacyPolicies".ToLower())
                masterEntities = await QueryAllAsync<PrivacyPolicies>();
            else if (masterType == "Country".ToLower())
                masterEntities = await QueryAllAsync<Country>();
            else if (masterType == "IncidentType".ToLower())
                masterEntities = await QueryAllAsync<IncidentType>();
            else if (masterType == "Devices".ToLower())
                masterEntities = await QueryAllAsync<Devices>();
            else if (masterType == "SystemAlerts".ToLower())
                masterEntities = await QueryAllAsync<SystemAlerts>();
            else if (masterType == "Classes".ToLower())
                masterEntities = await QueryAllAsync<CommunityClasses>();
            else if (masterType == "Houses".ToLower())
                masterEntities = await QueryAllAsync<House>();
            else if (masterType == "Banks".ToLower())
                masterEntities = await QueryAllAsync<Banks>();
            else if (masterType == "BankAccountType".ToLower())
                masterEntities = await QueryAllAsync<Banks>();
            else if (masterType == "BusinessCategory".ToLower())
            {
                masterEntities = await QueryAllAsync<BusinessCategory>();
				masterEntities = masterEntities.OrderBy(entity => entity.Name).ToList();
			}
            else if (masterType == "JobWorkType".ToLower())
                masterEntities = await QueryAllAsync<JobWorkType>();
            else if (masterType == "JobCategory".ToLower())
                masterEntities = await QueryAllAsync<JobCategory>();
            else
                masterEntities = new List<MasterEntity>();

            if (customerId > 0)
            {
                CustomerCommunity customerPrimaryCommunity = QueryAsync<CustomerCommunity>(CC => CC.CustomerId == customerId && CC.IsPrimary == true && CC.IsActive == true).Result.FirstOrDefault();
                if (customerPrimaryCommunity != null)
                {
                    if (masterType == "Classes".ToLower() || masterType == "Houses".ToLower())
                        masterEntities = masterEntities.Where<MasterEntity>(me => me.Code == (customerPrimaryCommunity.CommunityId.ToString() ?? ""));
                }
                customerSettings = QueryAsync<CustomerSettings>(CS => CS.CustomerId == customerId && CS.key == masterType && CS.IsActive == true).Result.FirstOrDefault();
                if (customerSettings == null)
                {
                    customerSettings = new CustomerSettings();
                    customerSettings.CustomerId = customerId;
                    customerSettings.key = masterType;
                    customerSettings.value = DateTime.Now.AddMinutes(-1).ToString();
                    customerSettings.Description = "Last fetched date";
                    customerSettings.FillDefaultValues();
                    await InsertAsync<CustomerSettings>(customerSettings);
                }
                else
                {
                    if (!string.IsNullOrEmpty(customerSettings?.value))
                        DateTime.TryParse(customerSettings.value, out lastfetchedDate);
                    customerSettings.value = DateTime.Now.AddMinutes(-1).ToString();
                    await UpdateAsync<CustomerSettings>(customerSettings);
                }

                if (!allRecords)
                    masterEntities = masterEntities.Where<MasterEntity>(me => me.ModifiedDate >= lastfetchedDate);
            }
            return masterEntities;

        }



        



        public async Task<int> RequestSupport(CustomerIssues customerIssues)
        {
            var result = await InsertAsync<CustomerIssues, int>(customerIssues);
            return result;
        }
        public async Task<bool> QRScan(QRScanRequest scanRequest)
        {
            try
            {
                return  ExecuteScalarAsync<bool>("exec [dbo].[USP_Master_QRScan] '" + (scanRequest.Type ?? "").ToString() + "'," +  scanRequest.LoggedInCustomerId + "," 
                    + scanRequest.ReferenceId + "," + scanRequest.OptionalReferenceId + "," + scanRequest.AdditionalReferenceId +   ",'" 
                    + (scanRequest.OptionalFirstParameter ?? "").ToString() + "','" + (scanRequest.OptionalSecondParameter ?? "").ToString() + "','" 
                    + (scanRequest.OptionalThirdParameter ?? "").ToString() + "'," + scanRequest.OptionalFourthParameter + "," + scanRequest.OptionalFifthParameter 
                    + "," + scanRequest.OptionalSixthParameter +         ",'" + scanRequest.OptionalSeventhParameter + "'," + scanRequest.OptionalEighthParameter 
                    + "," + scanRequest.OptionalNinethParameter).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<int> SaveTimeZone(long id, string CurrentTimeZone)
        {
            Customers update = new Customers();
            update.Id = id;
            update.CurrentTimeZone = CurrentTimeZone;
            var fields = Field.Parse<Customers>(x => new
            {
               
                x.CurrentTimeZone


            });
            var updaterow = await UpdateAsync<Customers>(entity: update, fields: fields);
            return updaterow;
        }

        public async Task<IEnumerable<dynamic>> DeleteOTP()
        {
            try
            {
                return await ExecuteQueryAsync<dynamic>("Exec [dbo].[Usp_DeleteOTP] " );
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<dynamic>> UniqueDevices()
        {
            try
            {
                return await ExecuteQueryAsync<dynamic>("Exec [dbo].[USP_Customer_UpdateDistinctDevicesEveryNight] ");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<MasterEntity>?> GetCommunityHouseAllAsync(string masterType, long CommunityId)
        {
            masterType = masterType.ToLower();
            IEnumerable<MasterEntity> masterEntities = null;
            if (masterType == "Houses".ToLower())
                masterEntities = await QueryAllAsync<House>();
            else
                masterEntities = new List<MasterEntity>();
            if(CommunityId > 0)
            {
                masterEntities = masterEntities.Where<MasterEntity>(me => me.Code == (CommunityId.ToString() ?? "") && me.IsActive == true);

            }
            return masterEntities; 

        }


        public async Task<IEnumerable<MasterEntity>?> GetCommunityClassesAllAsync(string masterType, long CommunityId)
        {
            masterType = masterType.ToLower();
            IEnumerable<MasterEntity> masterEntities = null;
            if (masterType == "Classes".ToLower())
                masterEntities = await QueryAllAsync<CommunityClasses>();
            else
                masterEntities = new List<MasterEntity>();
            if (CommunityId > 0)
            {
                masterEntities = masterEntities.Where<MasterEntity>(me => me.Code == (CommunityId.ToString() ?? "") && me.IsActive == true);

            }
            return masterEntities;

        }

        
    }
}
