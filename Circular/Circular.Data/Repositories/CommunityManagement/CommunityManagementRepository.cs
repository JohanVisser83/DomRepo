using Circular.Core.Entity;
using RepoDb;
using Microsoft.Data.SqlClient;
using Circular.Framework.Utility;
using Microsoft.AspNetCore.Http;

namespace Circular.Data.Repositories.CommunityManagement
{
     public class CommunityManagementRepository : DbRepository<SqlConnection>, ICommunityManagementRepository
    {
        private readonly IHelper _helper;
        IHttpContextAccessor _httpContextAccessor;

        public CommunityManagementRepository(string connectionString, IHelper helper, IHttpContextAccessor httpContextAccessor) : base(connectionString)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<IEnumerable<CommunityCategory>> GetCommunityCategory()
        {
           return  QueryAll<CommunityCategory>().Where(e => e.IsActive == true).ToList();
        }


        public async Task<IEnumerable<CommunitySignUp>> GetHalfBakedCommunityList()
        {
            return QueryAll<CommunitySignUp>().Where(c => c.FinalCommunityId != 0 &&  c.IsActive == true ).ToList(); 
        }

        public string HQAddCommunity(string AccountNumber, string OrgName, string AccessCode, string Email, string CommunityLogo, string DashboardBannerImage, string Country, long CountryId, 
            string Currency, string CurrencyToken, string About, string PrimaryMobileNo, string Website, string OrgAddress1, string AffiliateCode)
        {
            try
            {
                string strCommand = "exec [dbo].[Usp_HQAddCommunityInfo]" + " '" + AccountNumber + "','" + OrgName.Replace("'","''") + "','" + AccessCode + "','" + Email + "','" + CommunityLogo + "','" + DashboardBannerImage + "','" + Country + "','" + CountryId + "','" + Currency + "','" + CurrencyToken + "','" + About + "','" + PrimaryMobileNo + "','" +  Website + "','" + OrgAddress1 + "','" + AffiliateCode + "';";
                string result = (string)(ExecuteScalar(strCommand));
                return result;


            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<int> CreatelandingPage(Advertisement advertisement)
        {
            try
            {
                var key = await InsertAsync<Advertisement, int>(advertisement);
                return key;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<List<Communities>> GetEditCommunityDetails(long Id)
        {
            var communityDetails = QueryAll<Communities>().Where(a => a.Id == Id && a.IsActive == true).ToList();
            return (List<Communities>)communityDetails;
        }

        public async Task<List<Communities>> GetEditHalfBakedCommunityDetails(long Id)
        {

            var communities = QueryAll<Communities>().Where(a => a.Id == Id && a.IsActive == true).ToList();
            return (List<Communities>)communities;

        }


        public async Task<long> UpdateCommunityInfo(Communities communities)
        {

            Communities oCI = QueryAsync<Communities>(x => x.Id == communities.Id).Result.FirstOrDefault();
            if (string.IsNullOrEmpty(communities.OrgLogo) || communities.OrgLogo == "undefined")
                communities.OrgLogo = oCI.OrgLogo;

            if (string.IsNullOrEmpty(communities.coverimage) || communities.coverimage == "undefined")
                communities.coverimage = oCI.coverimage;
            

            communities.UpdateModifiedByAndDateTime();
            var fields = Field.Parse<Communities>(x => new
            {
                x.Id,
                x.OrgName,
                x.OrgLogo,
                x.coverimage,
                x.About,
                x.PrimaryMobileNo,
                x.PrimaryEmail,
                x.TitleLabel,
                x.TitleHolder,
                x.AccessCode,
                x.MainAccountNumber,
                x.ModifiedDate
            });
            var updaterow = await UpdateAsync<Communities>(entity: communities, fields: fields);
            return updaterow;

        }

       

        public async Task<IEnumerable<CommunitySignUp>> HalfBakedCommunityMember()
        {
            return QueryAll<CommunitySignUp>().Where(c => c.IsActive == true).ToList();
        }

        public async Task<List<AffiliateCode>> GetAllAffiliatedCode()
        {
            var result = ExecuteQuery<AffiliateCode>("exec dbo.USP_HQGetAvailableAffiliates").ToList();
            return result;
        }

        public async Task<IEnumerable<AffiliatedCodeDetails>> GetAffiliateCodelist(long Id )
        {
            var result = ExecuteQuery<AffiliatedCodeDetails>("exec dbo.usp_Affiliate_GetAffiliateList " + Id);
            return result;
        }

        public async Task<int> AffiliateCode(AffiliatedCodeDetails affiliateCode)
        {


            try
            {
                int key = 0;

                var result = QueryAsync<AffiliatedCodeDetails>(af => af.phone == affiliateCode.phone && af.Email == affiliateCode.Email && af.IsActive == true).Result.FirstOrDefault();
                if(result == null)
                {
                    AffiliateCodeMapping affiliateCodeMapping = new AffiliateCodeMapping();

                    key = await InsertAsync<AffiliatedCodeDetails, int>(affiliateCode);
                    if (key > 0)
                    {
                        affiliateCodeMapping.AffiliateId = key;
                        affiliateCodeMapping.AffiliateCodeId = affiliateCode.AffiliateCodeId;
                        affiliateCodeMapping.FillDefaultValues();
                        key = await InsertAsync<AffiliateCodeMapping, int>(affiliateCodeMapping);
                    }
                }

                return key;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }





        public async Task<long> DeleteCommunity(long id)
        {
            Communities communities = QueryAsync<Communities>(c => c.Id == id && c.IsActive == true).Result.FirstOrDefault();
            communities.UpdateModifiedByAndDateTime();

            if(communities.IsDeletedByAdmin == false)
            {
                communities.IsDeletedByAdmin = true;
                var fields = Field.Parse<Communities>(x => new
                {
                    x.IsDeletedByAdmin,
                    x.ModifiedDate
                });

                var updaterow = await UpdateAsync<Communities>(entity: communities, fields: fields);
                return updaterow;
            }
            else
            {
                communities.IsDeletedByAdmin = false;
                var fields = Field.Parse<Communities>(x => new
                {
                    x.IsDeletedByAdmin,
                    x.ModifiedDate
                });

                var updaterow1 = await UpdateAsync<Communities>(entity: communities, fields: fields);
                return updaterow1;
            }
            
            
           
        }

        public async  Task<IEnumerable<HQAllMemberDetails>> GetAllMemberDetails(long communityId, long customerId)
        {
            
          var result = ExecuteQuery<HQAllMemberDetails>("exec dbo.usp_HQMember_Getdetails " + " '" + communityId + "'," + customerId).ToList();
          return result;
            
        }

        public async Task<long> UpdateAffiliateCodeDetails(AffiliatedCodeDetails affiliateCode)
        {
            AffiliatedCodeDetails oCI = QueryAsync<AffiliatedCodeDetails>(af => af.Id == affiliateCode.Id).Result.FirstOrDefault();

            affiliateCode.UpdateModifiedByAndDateTime();
            var fields = Field.Parse<AffiliatedCodeDetails>(x => new
            {
                x.Id,
                x.FirstName,
                x.LastName,
                x.Email,
                x.phone,
                x.ModifiedDate
            });
            var updaterow = await UpdateAsync<AffiliatedCodeDetails>(entity: affiliateCode, fields: fields);
            return updaterow;
        }

        public async Task<IEnumerable<HQCommunitiesList>> GetCommunityMaxMemberlist()
        {
            var result = ExecuteQuery<HQCommunitiesList>("exec dbo.Usp_HQGetCommunityMemberMax ").ToList();
            return result;
        }

        public async Task<QR> GetMemberQRCode(long customerID)
        {
            QR code = new QR();
            string QRCodePath = Directory.GetCurrentDirectory() + "/Uploads/QRs/Customer/";
            var browsePath = _httpContextAccessor?.HttpContext.Request.Scheme + "://" + _httpContextAccessor?.HttpContext.Request.Host + "/Uploads/QRs/Customer/";
            string filename = _helper.EncryptUsingSHA1Hashing(customerID.ToString()) + ".png";
            code.QRCode = _helper.GetQRCode(customerID.ToString(), filename, ref QRCodePath);
            code.QRPath = browsePath + filename;
            return code;
        }

        public async Task<long> IsBlockUser(long communityId, long customerId, bool isblocked)
        {
            CustomerCommunity community = QueryAsync<CustomerCommunity?>(C => C.CustomerId == customerId && C.IsActive == true && C.CommunityId == communityId).Result.FirstOrDefault();
            community.UpdateModifiedByAndDateTime();
            community.IsActive = isblocked;
            var fields = Field.Parse<CustomerCommunity>(e => new
            {
                e.IsActive,
                e.ModifiedBy,
                e.ModifiedDate
            });

            var updatedRows = Update<CustomerCommunity>(entity: community, fields: fields);

            if (community.IsPrimary == true)
            {
                List<CustomerCommunity> cc = QueryAsync<CustomerCommunity?>(C => C.CustomerId == customerId && C.IsActive == true).Result.ToList();
                if (cc.Count > 0)
                {
                    CustomerCommunity customerCommunity = cc.FirstOrDefault();
                    customerCommunity.UpdateModifiedByAndDateTime();
                    customerCommunity.IsPrimary = true;

                    var fields2 = Field.Parse<CustomerCommunity>(e => new
                    {
                        e.IsPrimary,
                        e.ModifiedBy,
                        e.ModifiedDate
                    });

                    updatedRows = Update<CustomerCommunity>(entity: customerCommunity, fields: fields2);
                }
            }

            //Logout the customer
            return updatedRows;
        }
    }
}
