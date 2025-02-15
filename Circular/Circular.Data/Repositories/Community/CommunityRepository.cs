using Azure;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using LinqKit;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using RepoDb;
using RepoDb.Enumerations;
using System.Data;
using System.Linq;

namespace Circular.Data.Repositories.Community
{
    public class CommunityRepository : DbRepository<SqlConnection>, ICommunityRepository
    {
        public CommunityRepository(string connectionString) : base(connectionString)
        {

        }

        public IEnumerable<TEntity> ExecuteQuery<TEntity>(string commandText, object param = null, CommandType? commandType = null, string cacheKey = null, int? cacheItemExpiration = 180, IDbTransaction transaction = null) where TEntity : class
        { 
            IDbConnection dbConnection = transaction?.Connection ?? CreateConnection();
            try
            {
                return dbConnection.ExecuteQuery<TEntity>(commandText, param, commandType, cacheKey, cacheItemExpiration, "ExecuteQuery", CommandTimeout, transaction, Cache);
            }
            finally
            {
                if (dbConnection != null)
                {
                    dbConnection.Close();
                    dbConnection.Dispose();
                    dbConnection = null;
                }
                SqlConnection.ClearAllPools();
            }
        }

        public async Task<long> AddIsBlocked(long communityId, long customerId, bool Isblocked)
        {
            CustomerCommunity cc = new CustomerCommunity();
            cc.CommunityId = communityId;
            cc.CustomerId = customerId;

            List<CustomerCommunity> cclist = QueryAsync<CustomerCommunity?>(C => C.CustomerId == customerId && C.IsActive == true && C.IsPrimary == true).Result.ToList();
            if (cclist == null || cclist.Count == 0)
                cc.IsPrimary = true;
            cc.FillDefaultValues();
            var key = await InsertAsync<CustomerCommunity, int>(cc);
            return key;

        }

        public async Task<long> UpdateIsBlocked(long communityId, long customerId, bool Isblocked)
        {
            CustomerCommunity community = QueryAsync<CustomerCommunity?>(C => C.CustomerId == customerId && C.IsActive == true && C.CommunityId == communityId).Result.FirstOrDefault();
            community.UpdateModifiedByAndDateTime();
            community.IsActive = Isblocked;
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

        public async Task<int> CommunityAsync(Communities communities)
        {
            try
            {

                var key = await InsertAsync<Communities, int>(communities);
                return key;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Communities?> GetCommunityInfo(long id)
        {
            try
            {
                var user = QueryAsync<Communities>(x => x.Id == id && x.IsActive == true).Result.FirstOrDefault();
                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<long> UpdateCommunityInfo(Communities communities)
        {

            Communities oCI = QueryAsync<Communities>(x => x.Id == communities.Id).Result.FirstOrDefault();
            if (string.IsNullOrEmpty(communities.OrgLogo) || communities.OrgLogo == "undefined")
                communities.OrgLogo = oCI.OrgLogo;

            if (string.IsNullOrEmpty(communities.coverimage) || communities.coverimage == "undefined")
                communities.coverimage = oCI.coverimage;
            if (string.IsNullOrEmpty(communities.DashboardBanner) || communities.coverimage == "undefined")
                communities.DashboardBanner = oCI.DashboardBanner;

            communities.UpdateModifiedByAndDateTime();
            var fields = Field.Parse<Communities>(x => new
            {
                x.Id,
                x.OrgName,
                x.OrgLogo,
                x.coverimage,
                x.OrgAddress1,
                x.About,
                x.PrimaryMobileNo,
                x.PrimaryEmail,
                x.WorkingHours,
                x.TitleLabel,
                x.TitleHolder,
           
                x.Website,
                x.DashboardBanner,
                x.ModifiedDate
            });
            var updaterow = await UpdateAsync<Communities>(entity: communities, fields: fields);
            return updaterow;

        }

        public async Task<Communities?> GetSocialMedia(long id)
        {
            var user = QueryAsync<Communities>(x => x.Id == id && x.IsActive == true).Result.FirstOrDefault();
            return user;
        }

        public async Task<int> AddSocialMedia(Communities communities)
        {
            var key = await InsertAsync<Communities, int>(communities);
            return key;
        }
        public async Task<long> UpdateSocialMedia(Communities communities)
        {
            try
            {
                var fields = Field.Parse<Communities>(x => new
                {
                    x.Facebook,
                    x.LinkedIn,
                    x.Instagram,
                    x.TikTok,
                    x.Twitter,
                    x.Youtube,
                    x.ModifiedDate
                });
                var updaterow = await UpdateAsync<Communities>(entity: communities, fields: fields);
                return updaterow;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> AddStaff(CommunityTeamProfile communityStaff)
        {
            var key = await InsertAsync<CommunityTeamProfile, int>(communityStaff);
            return key;
        }

        public async Task<List<CommunityTeamProfile>> GetCommunityStaffAsync(long primaryId)
        {
            return QueryAll<CommunityTeamProfile>().Where((e => e.CommunityId.Equals(primaryId) && e.IsActive == true)).OrderByDescending(x => x.Id).ToList();
        }

        public async Task<List<CommunityTeamProfile>> GetCommunityTeamProfile(CommunityTeamProfile communityStaffs)
        {
            return QueryAll<CommunityTeamProfile>().Where(s => s.IsActive == true).OrderBy(x => x.Name).ToList();
        }
        public async Task<int> DeleteCommunityStaffAsync(long id)
        {
            CommunityTeamProfile deleteStaff = new CommunityTeamProfile();
            deleteStaff.Id = id;
            deleteStaff.IsActive = false;
            var fields = Field.Parse<CommunityTeamProfile>(x => new
            {
                x.IsActive
            });
            var updatedrows = await UpdateAsync<CommunityTeamProfile>(entity: deleteStaff, fields: fields);
            return updatedrows;
        }
        public async Task<CommunityTeamProfile> GetCommunityStaffById(long Id)
        {
            return QueryAll<CommunityTeamProfile>().Where(e => e.Id.Equals(Id) && e.IsActive.Equals(true)).FirstOrDefault();
        }
        public async Task<long> UpdateCommunityStaff(CommunityTeamProfile communityStaff)
        {
            CommunityTeamProfile oCF = QueryAsync<CommunityTeamProfile>(x => x.Id == communityStaff.Id).Result.FirstOrDefault();
            if (string.IsNullOrEmpty(communityStaff.ProfileImage) || communityStaff.ProfileImage == "undefined")
                communityStaff.ProfileImage = oCF.ProfileImage;

            communityStaff.UpdateModifiedByAndDateTime();
            var fields = Field.Parse<CommunityTeamProfile>(e => new
            {
                e.Name,
                e.Position,
                e.Email,
                e.Contact,
                e.ProfileImage,
                e.About,
                e.ModifiedDate,
                e.ModifiedBy

            });
            var updatedRows = Update(entity: communityStaff, fields: fields);
            return updatedRows;
        }
        public List<CustomerCommunity?> SearchCommunity(string communityName, long? CommunityId, long pagesize, long pagenumber, string search, long customerId)
        {

            List<CustomerCommunity> community = ExecuteQuery<CustomerCommunity>("Exec [dbo].[Usp_GetCommunities] " + CommunityId + "," + pagesize + "," + pagenumber + ",'" + search + "'," + customerId.ToString()).ToList();
            return community;

        }

        public async Task<int> SaveCustomerCommunity(CustomerCommunity customerCommunity)
        {
            var communities = QueryAsync<CustomerCommunity?>(e => e.CustomerId == customerCommunity.CustomerId
            && e.CommunityId == customerCommunity.CommunityId
             && e.IsActive == true).Result.FirstOrDefault() ?? null;
            if (communities == null)
            {
                if (!(bool)customerCommunity.IsPrimary)
                {
                    var customercommunity = await InsertAsync<CustomerCommunity, int>(customerCommunity);
                    return customercommunity;
                }
                else
                {
                    var primarycommunity = QueryAsync<CustomerCommunity?>(e => e.CustomerId == customerCommunity.CustomerId && e.IsPrimary == true && e.IsActive == true).Result.FirstOrDefault() ?? null;
                    if (primarycommunity != null)
                    {
                        primarycommunity.IsPrimary = false;
                        primarycommunity.UpdateModifiedByAndDateTime();
                        var fields = Field.Parse<CustomerCommunity>(e => new
                        {
                            e.IsPrimary,
                            e.ModifiedBy,
                            e.ModifiedDate
                        });
                        var updatedRows = Update<CustomerCommunity>(entity: primarycommunity, fields: fields);
                    }
                    var updatedRowcount = await InsertAsync<CustomerCommunity, int>(customerCommunity);
                    return updatedRowcount;
                }
            }
            else
            {
                if (customerCommunity.IsPrimary == true)
                {
                    var fields = Field.Parse<CustomerCommunity>(e => new
                    {
                        e.IsPrimary,
                        e.ModifiedBy,
                        e.ModifiedDate
                    });
                    var primarycommunity = QueryAsync<CustomerCommunity?>(e => e.CustomerId == customerCommunity.CustomerId && e.IsPrimary == true && e.IsActive == true).Result.FirstOrDefault() ?? null;
                    if (primarycommunity != null)
                    {
                        primarycommunity.IsPrimary = false;
                        primarycommunity.UpdateModifiedByAndDateTime();
                        var updatedRows = Update<CustomerCommunity>(entity: primarycommunity, fields: fields);
                    }
                    communities.IsPrimary = customerCommunity.IsPrimary;
                    var updatedRowcount = Update<CustomerCommunity>(entity: communities, fields: fields);
                    return updatedRowcount;
                }

                return 0;
            }

        }
        public async Task<List<Customers>> SendEmailSigupUser(long Id)
        {
            return QueryAll<Customers>().Where(G => G.IsActive == true && G.Id == Id).ToList();
        }
        public async Task<List<CustomerDetails>> GetUserName(long Id)
        {
            return QueryAll<CustomerDetails>().Where(G => G.IsActive == true && G.CustomerId == Id).ToList();
        }

        public async Task<List<Groups>> GetCommunityGroups(long communityId)
        {
            return ExecuteQuery<Groups>("exec dbo.usp_GetCommunityMembers " + communityId.ToString()).ToList<Groups>();
        }

        public async Task<List<CustomerDetails>> GetCommunityUser(long communityId)
        {
            return ExecuteQuery<CustomerDetails>("exec dbo.USP_GetTotalUserInCommunity " + communityId.ToString()).ToList<CustomerDetails>();
        }


        public async Task<IEnumerable<Features>?> Features(long communityId , long loggedInUserId)
        {
            var icons = ExecuteQuery<Features>("exec dbo.USP_Community_Features " + communityId +","+ loggedInUserId);
            return icons;
        }
        public async Task<IEnumerable<Features>?> Features_App(long communityId, long loggedInUserId)
        {
            var icons = ExecuteQuery<Features>("exec dbo.USP_Community_Features_App " + communityId + "," + loggedInUserId);
            return icons;
        }
        public async Task<IEnumerable<AdminFeature>?> AdminFeature(long LoggedInUser,long communityId)
        {
            var icons = ExecuteQuery<AdminFeature>("exec dbo.Usp_RestrictUserAccess " + LoggedInUser +","+ communityId.ToString());
            return icons;
        }

        public async Task<IEnumerable<CustomerCommunity>?> CommunityListByCustomerId(long customerId)
        {
            return QueryAll<CustomerCommunity>().Where(x => x.CustomerId == customerId && x.IsActive == true).ToList();
        }
        public async Task<IEnumerable<CustomerCommunity>?> GetCommunityMembers(long communityId)
        {
            return QueryAll<CustomerCommunity>().Where(x => x.CommunityId == communityId && x.IsActive == true).ToList();
        }
        public async Task<IEnumerable<CustomerDetails>?> GetCommunityMemberDetails(long communityId, int UserTypeId, long CustomerId, int IncludeCurrentCustomer, int ShowInactive = 0)
        {
            return ExecuteQuery<CustomerDetails>("exec dbo.USP_Community_MemberDetails " + UserTypeId + "," + communityId + "," + CustomerId + "," + IncludeCurrentCustomer + "," + ShowInactive);
        }
        public async Task<IEnumerable<CustomerGroups>?> GetCommunityGroupMembers(long groupId)
        {
            return QueryAll<CustomerGroups>().Where(x => x.GroupId == groupId && x.IsActive == true).ToList();
        }
        public async Task<int> DeleteCustomerCommunity(long communityId, string UserId)
        {
            string commandText = "exec [dbo].[USP_Customer_DelinkCommunity]" + " '" + UserId.ToString() + "'," + communityId.ToString() + ";";
            return await ExecuteNonQueryAsync(commandText);
        }
        public async Task<IEnumerable<Features>?> DashboardIcons(long customerId, long communityId)
        {
            var icons = ExecuteQuery<Features>("exec dbo.USP_GetDashboardIcons2 " + customerId.ToString());
            return icons;
        }
        public async Task<IEnumerable<CommunityStaffs>?> Staff(long communityId)
        {
            return QueryAll<CommunityStaffs>().Where(x => x.CommunityId == communityId && x.IsActive == true).OrderBy(x => x.Name).ToList();
        }
        public async Task<IEnumerable<CommunityTeamProfile>?> CommunityTeamProfile(long communityId)
        {
            return QueryAll<CommunityTeamProfile>().Where(x => x.CommunityId == communityId && x.IsActive == true).OrderBy(x => x.Name).ToList();
        }
        public async Task<IEnumerable<CustomerDetails>> GetCommunityOrganizers(long CommunityId)
        {
            return await ExecuteQueryAsync<CustomerDetails>("exec dbo.Usp_Community_Organizers "
                + CommunityId.ToString());
        }

        #region "Business"
        public async Task<int> AddBusinessIndex(CustomerBusinessIndex customerBusinessIndex)
        {
            var key = await InsertAsync<CustomerBusinessIndex, int>(customerBusinessIndex);
            return key;
        }
        public async Task<IEnumerable<CustomerBusinessIndex>?> GetBusinessIndex(long CommunityId, long? id, long? UserId, int BusinessCategoryId,
            string? searchText, int pageNumber, int pageSize, bool IsPendingBusinessOnly)
        {
            var predicate = PredicateBuilder.New<CustomerBusinessIndex>();
            predicate = predicate.And(x => x.IsActive == true && x.CommunityId == CommunityId);
            if (id > 0)
                predicate = predicate.And(x => x.Id == id);

            if (UserId > 0)
                predicate = predicate.And(x => x.OwnerId == UserId);
            else
            {
                if (id <= 0)
                    predicate = predicate.And(x => x.IsBusinessapproved == true);
                else
                {
                    if (IsPendingBusinessOnly)
                        predicate = predicate.And(x => x.IsBusinessapproved == false);
                }
            }


            if (BusinessCategoryId > 0)
                predicate = predicate.And(x => x.CategoryId == BusinessCategoryId);
            if (!searchText.IsNullOrEmpty())
                predicate = predicate.And(x => x.CompanyName.Contains(searchText, StringComparison.OrdinalIgnoreCase) || x.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase));

            IEnumerable<CustomerBusinessIndex> CBList = QueryAll<CustomerBusinessIndex?>().Where(predicate).OrderBy(x => x.CompanyName).Skip(pageNumber - 1).Take(pageSize);
            return CBList;
        }
        public async Task<long> ChangeIsBusinessApproved(long Id, bool IsBusinessApproved, long LoggedInUserId)
        {
            CustomerBusinessIndex businessIndex = new CustomerBusinessIndex();
            businessIndex.Id = Id;
            businessIndex.IsActive = IsBusinessApproved;
            businessIndex.IsBusinessapproved = IsBusinessApproved;
            businessIndex.UpdateModifiedByAndDateTime();

            var fields = Field.Parse<CustomerBusinessIndex>(e => new
            {
                e.Id,
                e.IsActive,
                e.IsBusinessapproved,
                e.ModifiedBy,
                e.ModifiedDate
            });

            var updatedRows = Update<CustomerBusinessIndex>(entity: businessIndex, fields: fields);
            return updatedRows;


        }
        public async Task<long> ChangeIsAdmin(long Id, bool IsAdmin, long communityId)
        {
            CustomerCommunity customercomm = QueryAsync<CustomerCommunity?>(C => C.CustomerId == Id && C.IsActive == true && C.CommunityId == communityId).Result.FirstOrDefault() ?? null;
            customercomm.IsAdmin = IsAdmin;
            customercomm.UpdateModifiedByAndDateTime();
            var fields = Field.Parse<Customers>(e => new
            {
                e.IsAdmin,
                e.ModifiedBy,
                e.ModifiedDate
            });
            var updatedRows = Update<CustomerCommunity>(entity: customercomm, fields: fields);
            return updatedRows;
        }

        public async Task<List<CustomerBusinessIndex>> SendEmailBusinessUser(long Id)
        {
            return QueryAll<CustomerBusinessIndex>().Where(B => B.IsActive == true && B.Id == Id).ToList();
        }

        public async Task<List<CustomerDetails>> GetCustomerDetails(long Id)
        {
            try
            {
                // return QueryAll<CustomerDetails>().Where(E => E.IsActive == true && E.CustomerId == Id).ToList();
                var response = ExecuteQuery<CustomerDetails>("exec dbo.[USP_Community_GetCustomerDetails] '" + Id + "'").ToList();
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<long> DashboardIsBusinessApproved(long Id)
        {
            CustomerBusinessIndex customerBusinessIndex = new CustomerBusinessIndex();
            customerBusinessIndex.Id = Id;
            customerBusinessIndex.IsBusinessapproved = true;
            customerBusinessIndex.UpdateModifiedByAndDateTime();

            var fields = Field.Parse<CustomerBusinessIndex>(d => new
            {
                d.Id,
                d.IsBusinessapproved,
                d.ModifiedBy,
                d.ModifiedDate
            });

            var updatedRows = Update<CustomerBusinessIndex>(entity: customerBusinessIndex, fields: fields);
            return updatedRows;
        }
        public async Task<long> DeleteBusiness(CustomerBusinessIndex business)
        {
            business.UpdateModifiedByAndDateTime();
            business.IsActive = false;
            var fields = Field.Parse<Jobs>(e => new
            {
                e.IsActive,
                e.ModifiedBy,
                e.ModifiedDate
            });
            var updatedRows = Update<CustomerBusinessIndex>(entity: business, fields: fields);
            return updatedRows;
        }

        public async Task<long> EditBusiness(CustomerBusinessIndex business)
        {
            business.UpdateModifiedByAndDateTime();
            var fields = Field.Parse<CustomerBusinessIndex>(e => new
            {
                e.CategoryId,
                e.CompanyName,
                e.CompanyAddress,
                e.CompanyWebsite,
                e.CompanySize,
                e.CompanyEmail,
                e.ContactNumber,
                e.Location,
                e.Description,
                e.CoverImage,
                e.CompanyLogo,
                e.Hours,
                e.ModifiedBy,
                e.ModifiedDate
            });
            var updatedRows = Update<CustomerBusinessIndex>(entity: business, fields: fields);
            return updatedRows;
        }

        public async Task<long> UpdateJobCount(long businessId)
        {
            var updatedRows = 0;
            CustomerBusinessIndex business = QueryAsync<CustomerBusinessIndex?>(B => B.Id == businessId && B.IsActive == true).Result.FirstOrDefault() ?? null;
            if (business != null)
            {
                business.JobCount = business.JobCount + 1;
                var fields = Field.Parse<CustomerBusinessIndex>(e => new
                {
                    e.JobCount,
                    e.ModifiedBy,
                    e.ModifiedDate
                });

                updatedRows = Update<CustomerBusinessIndex>(entity: business, fields: fields);
            }
            return updatedRows;
        }

        #endregion



        public async Task<IEnumerable<CustomerSubscriptionStatus>> GetcustomerSubscriptionStatus()
        {
            var customerSubscriptionStatus = QueryAll<CustomerSubscriptionStatus>().Where(e => e.IsActive == true).ToList();
            return customerSubscriptionStatus;
        }
        public async Task<IEnumerable<CustomerMembershipPaymentStatus>> GetCustomerMembershipPaymentStatus()
        {
            var customerMembershipPaymentStatus = QueryAll<CustomerMembershipPaymentStatus>().Where(e => e.IsActive == true).ToList();
            return customerMembershipPaymentStatus;
        }
        public async Task<int> ChangeCustomersubScriptionStatus(long userId, int? subscriptionStatusId)
        {


            var query = "exec [USP_Community_ChangeSubscriptionStatusId]" + "" + userId + " ,'" + subscriptionStatusId + "';";
            var result = await ExecuteQueryAsync<dynamic>(query);
            if (result != null)
            {
                return 1;
            }
            return 0;

        }
        public async Task<int> ChangePaymentStatus(long userId, int? paymentStatusId)
        {


            var query = "exec [USP_Community_ChangePaymentStatusId]" + "" + userId + " ,'" + paymentStatusId + "';";
            var result = await ExecuteQueryAsync<dynamic>(query);
            if (result != null)
            {
                return 1;
            }
            return 0;

        }

        #region "Jobs"

        public async Task<long> AddJobPosting(Jobs jobs)
        {
            long result = await InsertAsync<Jobs, long>(jobs);
            UpdateJobCount(jobs.BusinessId);
            return result;
        }
        public async Task<long> EditJobPosting(Jobs job)
        {
            job.UpdateModifiedByAndDateTime();
            var fields = Field.Parse<Jobs>(e => new
            {
                e.CategoryId,
                e.CompanyName,
                e.YourName,
                e.Position,
                e.CompanySize,
                e.JobTitle,
                e.JobDesc,
                e.Location,
                e.WorkTypeId,
                e.CompanyWebsite,
                e.ContactNumber
                ,
                e.CompanyEmail,
                e.CoverPhoto,
                e.AttachJobSpec,
                e.ModifiedBy,
                e.ModifiedDate
            });
            var updatedRows = Update<Jobs>(entity: job, fields: fields);
            return updatedRows;
        }
        public async Task<long> DeleteJobPosting(Jobs job)
        {
            job.UpdateModifiedByAndDateTime();
            job.IsActive = false;
            var fields = Field.Parse<Jobs>(e => new
            {
                e.IsActive,
                e.ModifiedBy,
                e.ModifiedDate
            });

            var updatedRows = Update<Jobs>(entity: job, fields: fields);
            return updatedRows;
        }
        public async Task<long> UpdateViewCount(Jobs job)
        {
            var updatedRows = 0;
            job = QueryAsync<Jobs?>(J => J.Id == job.Id && J.IsActive == true).Result.FirstOrDefault() ?? null;
            if (job != null)
            {
                job.ViewCount = job.ViewCount + 1;
                var fields = Field.Parse<Jobs>(e => new
                {
                    e.ViewCount,
                    e.ModifiedBy,
                    e.ModifiedDate
                });

                updatedRows = Update<Jobs>(entity: job, fields: fields);
            }
            return updatedRows;
        }
        public async Task<long> UpdateApplicantCount(long jobId)
        {
            var updatedRows = 0;
            Jobs job = QueryAsync<Jobs?>(J => J.Id == jobId && J.IsActive == true).Result.FirstOrDefault() ?? null;
            if (job != null)
            {
                job.ApplicantCount = job.ApplicantCount + 1;
                var fields = Field.Parse<Jobs>(e => new
                {
                    e.ApplicantCount,
                    e.ModifiedBy,
                    e.ModifiedDate
                });

                updatedRows = Update<Jobs>(entity: job, fields: fields);
            }
            return updatedRows;
        }

        public async Task<long> ApplyJob(JobApplication jobApplication)
        {
            var JAId = await InsertAsync<JobApplication, long>(jobApplication);
            UpdateApplicantCount(jobApplication.JobId);
            return JAId;
        }

        public async Task<IEnumerable<Jobs>> GetCustomer(long customerId)
        {
            var customer = QueryAll<Jobs>().Where(e => e.Id == customerId && e.IsActive == true).ToList();
            return customer;
        }
        public async Task<IEnumerable<Jobs>?> GetJobPosting(long BusinessId, long id, long customerId, int jobCategoryId,
            string searchText, long communityId, int pageNumber, int pageSize)
        {
            var predicate = PredicateBuilder.New<Jobs>();
            predicate = predicate.And(x => x.IsActive == true && x.CommunityId == communityId);
            if (BusinessId > 0)
                predicate = predicate.And(x => x.BusinessId == BusinessId);
            if (id > 0)
                predicate = predicate.And(x => x.Id == id);
            if (customerId > 0)
                predicate = predicate.And(x => x.CustomerId == customerId);
            else
            {
                if (id == 0)
                    predicate = predicate.And(x => x.IsApproved == true);
                else if (id == -2)
                    predicate = predicate.And(x => x.IsApproved == false);
            }

            if (jobCategoryId > 0)
                predicate = predicate.And(x => x.CategoryId == jobCategoryId);
            if (!searchText.IsNullOrEmpty())
                predicate = predicate.And(x => x.JobTitle.Contains(searchText, StringComparison.OrdinalIgnoreCase) || x.JobDesc.Contains(searchText, StringComparison.OrdinalIgnoreCase));

            var jobs = QueryAll<Jobs?>().Where(predicate).OrderBy(x => x.CreatedDate).Skip(pageNumber - 1).Take(pageSize);
            List<Jobs> finalJobs = new List<Jobs>();
            foreach (Jobs j in jobs)
            {
                CustomerBusinessIndex? cb = Query<CustomerBusinessIndex>(CB => CB.Id == j.BusinessId)?.FirstOrDefault();
                if (cb != null && cb.IsActive == true && cb.IsBusinessapproved == true)
                {
                    j.BusinessLogo = cb?.CompanyLogo;
                    finalJobs = finalJobs.Append(j).ToList();
                }
                if (id == -2 && finalJobs.Count() >= 3)
                    break;
            }
            return finalJobs;
        }

        public async Task<long> ChangeIsJobApproved(long Id, bool IsApproved, long LoggedInUserId)
        {
            Jobs jobs = new Jobs();
            jobs.Id = Id;
            jobs.IsActive = IsApproved;
            jobs.IsApproved = IsApproved;
            jobs.CommunityId = LoggedInUserId;
            jobs.UpdateModifiedByAndDateTime();

            var fields = Field.Parse<Jobs>(e => new
            {
                e.Id,
                e.IsActive,
                e.IsApproved,
                e.ModifiedBy,
                e.ModifiedDate
            });

            var updatedRows = Update<Jobs>(entity: jobs, fields: fields);
            return updatedRows;
        }

        public async Task<List<Jobs>> SendEmailJobUser(long Id)
        {
            return QueryAll<Jobs>().Where(E => E.IsActive == true && E.Id == Id).ToList();
        }
        //public async Task<List<Jobs>> GetJobCompanyName(long Id)
        //{
        //    return QueryAll<Jobs>().Where(E => E.IsActive == true && E.CustomerId == Id).ToList();
        //}

        //public async Task<long> DashboardIsJobApprove(long Id)
        //{
        //    Jobs job = new Jobs();
        //    job.Id = Id;
        //    job.IsApproved = true;
        //    job.UpdateModifiedByAndDateTime();

        //    var fields = Field.Parse<Jobs>(j => new
        //    {
        //        j.Id,
        //        j.IsApproved,
        //        j.ModifiedBy,
        //        j.ModifiedDate
        //    });

        //    var updateRows = Update<Jobs>(entity: job, fields: fields);
        //    return updateRows;
        //}
        #endregion


        public async Task<int> NewCustomerGroup(Groups groups)
        {
            var key = await InsertAsync<Groups, int>(groups);
            return key;
        }
        public async Task<IEnumerable<Groups>?> GetCustomerGroup(long communityId)
        {
            return await ExecuteQueryAsync<Groups>("exec dbo.USP_GetAllCustomGroups "
               + communityId.ToString());
        }
        public async Task<long> DeleteCustomGroup(long Id)
        {
            Groups groups = new Groups();
            groups.Id = (long)Id;
            groups.IsActive = false;

            var fields = Field.Parse<Groups>(x => new
            {
                x.IsActive
            });
            var updaterow = Update<Groups>(entity: groups, fields: fields);
            return updaterow;
        }

        public List<dynamic> GetCommunityNetwork(long CommunityId, long? LoggedInUserId, string? SearchText, int IsFriend, int pageNumber, int pageSize)
        {
            return ExecuteQuery<dynamic>("exec dbo.[USP_Community_Network] " + CommunityId + "," + LoggedInUserId + ",'" + SearchText + "'," + IsFriend
                 + "," + pageNumber + "," + pageSize).ToList();

        }



        //linked member
        //public async List<LinkedMembers> GetLinkedMember(long userId)
        //{
        //List<LinkedMembers> cls = new List<LinkedMembers>();
        //var result = await ExecuteQuery<dynamic>("exec [dbo].[USP_GetLinkedMemberInfo] " + userId + "").ToList();
        //if (result != null && result.Count > 0)
        //{
        //    foreach(var item in result)
        //    {
        //        LinkedMembers linkedMembers = new LinkedMembers();
        //       linkedMembers.

        //        cls.Add(obj);
        //    }
        //}
        //    return 8;

        //}

        public async Task<IEnumerable<CustomerDetails>?> GetAllCustomers(long cusotmerId)
        {
            var customerDetails = QueryAll<CustomerDetails>().Where(e => e.IsActive == true).ToList();
            return customerDetails;
        }
        public async Task<List<UserContactList>> GetUserContactListAsync(long CommunityId, string Search)
        {
            string query = "exec [USP_Messages_SearchContact]" + "" + CommunityId + " ,'" + Search + "';";
            var result = await ExecuteQueryAsync<UserContactList>(query);
            return result.ToList<UserContactList>();
        }
        public async Task<long> AddUser(CustomerGroups customerGroups)
        {

            var result = await InsertAsync<CustomerGroups, long>(customerGroups);
            return result;
        }
        public async Task<int> GetLinkedMember(string Name, long Loggedinuser)
        {

            var query = "exec [dbo].[]" + " '" + Name + "','" + Loggedinuser + "'";
            var result = await ExecuteQueryAsync<dynamic>(query);
            if (result != null)
            {
                return 1;
            }
            return 0;

        }
        public long ActionOnNetwork(long ToId, long FromId, int FriendRequestStatusId)
        {
            var response = ExecuteQuery<dynamic>("exec dbo.[USP_Community_ActionOnNetwork] " + ToId + "," + FromId + "," + FriendRequestStatusId).FirstOrDefault();
            return Convert.ToInt64(response.Response ?? 0);

        }


        public async Task<List<Fundraiser>> GetFundraisers(long communityid)
        {
            var results = await ExecuteQueryAsync<Fundraiser>("exec [dbo].[USP_Community_ActiveFundraisers]" + +communityid + "," + 0);
            if (results != null)
                return (List<Fundraiser>)results;
            else
                return null;
        }



        public async Task<List<Fundraiser>> ViewFundraisersAsync(long Id)
        {
            try
            {
                var result = await ExecuteQueryAsync<Fundraiser>("Exec [dbo].[USP_Community_ViewActiveFundraisers]" + " '" + Id + "'");
                if (result != null)
                {
                    foreach (var f in result)
                    {
                        f.Images = QueryAll<FundraiserProductImages>().Where(x => x.FundraiserId == f.Id && x.IsActive == true).ToList() ?? new List<FundraiserProductImages>();
                    }
                    return (List<Fundraiser>)result;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<int> UpdateCollected(long id, bool Iscollected)
        {

            try
            {
                FundraiserCollection update = new FundraiserCollection();
                update.Id = id;
                update.IsCollected = Iscollected == true ? 1 : 0;
                update.UpdateModifiedByAndDateTime();
                var fields = Field.Parse<FundraiserCollection>(x => new
                {
                    x.IsCollected,
                    x.ModifiedBy,
                    x.ModifiedDate
                });
                var updaterow = await UpdateAsync<FundraiserCollection>(entity: update, fields: fields);
                return updaterow;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<bool> UpdateFundraisersAsync(string Title, long Amount, DateTime ExpiryDate, string PDFLink, string Description, string FormLink, long Id, long Community, long OrganizerId, string ImagePath)
        {
            try
            {

                String Query = "exec [dbo].[Usp_Community_UpdateFundraisers] '" + Title + "','" + Amount + "','" + ExpiryDate.ToString("yyyy-MM-dd") + "','" + PDFLink + "','" + Description + "','" + FormLink + "','" + Id + "','" + Community + "','" + OrganizerId + "','" + ImagePath + "'";
                int status = await ExecuteNonQueryAsync(Query);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }



        //public async Task<long> UpdateFundraisersAsync(string Title, long Amount, DateTime ExpiryDate, string PDFLink, string Description, string FormLink, long Id, long Community, long OrganizerId, string ImagePath)
        //{
        //    try
        //    {
        //        FundraiserDTO fund = new FundraiserDTO();
        //        FundraiserDTO oCF = (await QueryAsync<FundraiserDTO>(x => x.Id == Id)).FirstOrDefault();
        //        if (string.IsNullOrEmpty(PDFLink) || PDFLink == "undefined")
        //            PDFLink = oCF.PDFLink;

        //        var fields = Field.Parse<FundraiserDTO>(e => new
        //        {
        //            e.Title,
        //            e.Amount,
        //            e.ExpiryDate,
        //            e.PDFLink,
        //            e.Description,
        //            e.FormLink,
        //            e.Id,
        //            e.CommunityId,
        //            e.OrganizerId,
        //            e.ImagePath

        //        });
        //        var updatedRows = Update(entity: fund, fields: fields);
        //        return updatedRows;
        //    }
        //    catch (Exception ex)
        //    {
        //        return 1;
        //    }
        //}



        public async Task<bool> UploadImageAndFile5Async(long FundraiserId, string ImagePath)
        {

            String Query = "exec [dbo].[USP_Community_GetTitle] '" + FundraiserId + "','" + ImagePath + "'";
            int status = await ExecuteNonQueryAsync(Query);
            return true;
        }

        public async Task<List<Fundraiser>> ViewArchivedFundraisersAsync(long Id)
        {

            try
            {
                var result = await ExecuteQueryAsync<Fundraiser>("Exec [dbo].[USP_Community_ViewActiveFundraisers]" + " '" + Id + "'");
                if (result != null)
                {
                    foreach (var f in result)
                    {
                        f.Images = QueryAll<FundraiserProductImages>().Where(x => x.FundraiserId == f.Id && x.IsActive == true).ToList() ?? new List<FundraiserProductImages>();
                    }
                    return (List<Fundraiser>)result;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }


        }

        public async Task<int> DeleteArchivedFundraisersAsync(long Id)
        {
            Fundraiser delete = new Fundraiser();
            delete.Id = Id;
            delete.IsActive = false;
            var fields = Field.Parse<Fundraiser>(x => new
            {
                x.IsActive

            });
            var updaterow = Update<Fundraiser>(entity: delete, fields: fields);
            return updaterow;
        }
        public async Task<List<Fundraiser>> GetFundraiserAsync(long CommunityId)
        {

            var result = await ExecuteQueryAsync<Fundraiser>("Exec [dbo].[USP_Community_ActiveFundraisers]" + CommunityId + "'");
            if (result != null)
                return (List<Fundraiser>)result;
            else
                return null;
        }

        public async Task<List<FundraiserType>> Gettypeoffundraiser()
        {
            var results = QueryAll<FundraiserType>().Where(x => x.IsActive == true).ToList();
            if (results != null)
                return results;
            else
                return null;
        }

        public async Task<bool> SaveNewCompaignAsync(long Communityid, long FundraiserTypeId, string FundraiserTitle, long OrganizerId, decimal ProductAmount, DateTime ExpiryDate, string PDFLink, string Description, string FormHyperlink, string ImagePath)
        {
            try
            {
                String Query = "exec [dbo].[Usp_Community_PostCompaign] '" + Communityid + "','" + FundraiserTypeId + "','" + FundraiserTitle + "','" + OrganizerId + "','" + ProductAmount + "','" + ExpiryDate.ToString("MM-dd-yyyy HH:mm:ss") + "','" + PDFLink + "','" + Description + "','" + FormHyperlink + "','" + ImagePath + "'";
                int status = await ExecuteNonQueryAsync(Query);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<int> ArchiveFundraisersAsync(long Id)
        {
            Fundraiser archive = new Fundraiser();
            archive.Id = Id;
            archive.IsArchive = true;
            var fields = Field.Parse<Fundraiser>(x => new
            {
                x.IsArchive

            });
            var updaterow = Update<Fundraiser>(entity: archive, fields: fields);
            return updaterow;
        }

        public async Task<List<Fundraiser>> GetArchiveFundraisers(long CommunityId)
        {

            var result = await ExecuteQueryAsync<Fundraiser>("Exec [dbo].[USP_Community_ArchivedFundraisers]" + CommunityId + "," + 0);
            if (result != null)
                return (List<Fundraiser>)result;
            else
                return null;
        }

        public FundListResponse GetFundraiserAsync(long CommunityId, long? FundraiserId)
        {
            try
            {
                List<Fundraiser> lstFundraiser = ExecuteQuery<Fundraiser>("exec [dbo].[USP_Community_ActiveFundraisers] " + CommunityId + "," + FundraiserId).ToList();
                FundListResponse fundListResponse = null;
                if (lstFundraiser != null)
                {
                    fundListResponse = new FundListResponse();
                    foreach (var f in lstFundraiser)
                    {
                        f.Images = QueryAll<FundraiserProductImages>().Where(x => x.FundraiserId == f.Id && x.IsActive == true).ToList() ?? new List<FundraiserProductImages>();
                    }
                    lstFundraiser.ForEach(fundraiser =>
                    {
                        // group the data in dates
                        int index = fundListResponse.FundGroups.FindIndex(fg => fg.Date == (new DateTime(fundraiser.ExpiryDate.Year, fundraiser.ExpiryDate.Month, 1)));
                        if (index == -1)
                        {
                            FundGroupResponse fundraiserGroupResponse = new FundGroupResponse();
                            fundraiserGroupResponse.Date = (new DateTime(fundraiser.ExpiryDate.Year, fundraiser.ExpiryDate.Month, 1));
                            fundraiserGroupResponse.FundraiserList?.Add(fundraiser);
                            fundListResponse.FundGroups.Add(fundraiserGroupResponse);
                        }
                        else

                            fundListResponse.FundGroups[index].FundraiserList?.Add(fundraiser);
                    }
                    );
                }


                return fundListResponse;
            }
            catch (Exception ex)
            {
                return null;
            }



        }

        public long PayFundraiser(long fundraiserTypeId, long PayForUserId, decimal Amount, long LoggedInUserId, string Currency)
        {
            var response = ExecuteQuery<dynamic>("exec dbo.[Usp_Community_PayFundraiser] " + fundraiserTypeId + "," + PayForUserId + "," + Amount + "," + LoggedInUserId + "," + Currency).FirstOrDefault();
            return Convert.ToInt64(response.result ?? 0);
        }

        public async Task<int> AddUserInGroup(CustomerGroups customerGroups)
        {
            CustomerGroups customerGroup = QueryAsync<CustomerGroups?>(e => e.CustomerId == customerGroups.CustomerId && e.GroupId == customerGroups.GroupId && e.IsActive == true).Result.FirstOrDefault() ?? null;
            if (customerGroup == null)
            {
                var key = await InsertAsync<CustomerGroups, int>(customerGroups);
                return key;
            }
            else
                return 0;
        }

        public async Task<IEnumerable<dynamic>> ShowCustomGroupList(long Id)
        {
            return await ExecuteQueryAsync<dynamic>("Exec [dbo].[USP_Community_GetgroupsMemberList] '" + Id + "'");
        }

        public async Task<List<FundraiserCollection>> GetViewPayments(long Id)
        {

            var results = await ExecuteQueryAsync<FundraiserCollection>("exec [dbo].[USP_Community_ViewPayments]'" + Id + "'");
            if (results != null)
                return (List<FundraiserCollection>)results;
            else
                return null;

        }


        public async Task<long> DeleteGroupUsers(long Id)
        {
            CustomerGroups customerGroups = new CustomerGroups();
            customerGroups.Id = Id;
            customerGroups.IsActive = false;

            var fields = Field.Parse<CustomerGroups>(x => new
            {
                x.IsActive
            });

            var updaterow = Update<CustomerGroups>(entity: customerGroups, fields: fields);
            return updaterow;

        }

        public async Task<IEnumerable<dynamic>> GetFundhubActiveOrders(long loggedinuser, long communityId, bool isCollected)
        {
            try
            {
                var response = ExecuteQuery<dynamic>("exec dbo.[USP_Community_GetFundhubActiveOrders] " + loggedinuser + "," + communityId + "," + isCollected).ToList();

                return response;

            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<int> DeleteCircleInfoItem2Async(long Id)
        {
            FundraiserProductImages delete = new FundraiserProductImages();
            delete.Id = Id;
            delete.IsActive = false;
            var fields = Field.Parse<FundraiserProductImages>(x => new
            {
                x.IsActive

            });
            var updaterow = Update<FundraiserProductImages>(entity: delete, fields: fields);
            return updaterow;
        }

        public async Task<bool> SaveAmountCompaignAsync(long Communityid, long FundraiserTypeId, string FundraiserTitle, long OrganizerId, decimal ProductAmount, string PDFLink, string Description, string ImagePath, DateTime ExpiryDate)
        {
            try
            {
                // DateTime expirydate = DateTime.Now.Date.AddYears(1);
                String Query = "exec [dbo].[Usp_Community_PostCompaignAmount] '" + Communityid + "','" + FundraiserTypeId + "','"
                    + FundraiserTitle + "','" + OrganizerId + "','" + ProductAmount + "','" + PDFLink + "','" + Description + "','" + ImagePath + "','" + ExpiryDate.ToString("yyyy-MM-dd") + "'";
                int status = await ExecuteNonQueryAsync(Query);
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }

        }

        public async Task<IEnumerable<CommunityDetails>> GetCommunities(long CommunityId, string search, long pageNumber, long pageSize)
        {
            var response = ExecuteQuery<CommunityDetails>("exec dbo.[Usp_GetCommunities] " + CommunityId.ToString() + "," + pageSize.ToString() + "," + pageNumber.ToString() + ",'" + search + "',0").ToList();
            return response;
        }

        public async Task<long> GetCommunityId(string URL)
        {
            Communities result = QueryAsync<Communities>(c => c.CoummunityUrl == URL && c.IsActive == true).Result.FirstOrDefault();
            return result.Id;
        }
        public async Task<string> GetCommunityURL(long Id)
        {
            Communities result = QueryAsync<Communities>(c => c.Id == Id && c.IsActive == true).Result.FirstOrDefault();
            return result.CoummunityUrl;
        }
        public int CheckValidAccessCode(string? accessCode, long communityId)
        {
            var result = QueryAsync<Communities>(c => c.AccessCode == accessCode && c.Id == communityId && c.IsActive == true).Result;
            if (result != null && result.Count() > 0)
                return (int)result.FirstOrDefault().Id;
            else
                return 0;
        }

        public async Task<long> SaveNewCustomerCommunity(CustomerCommunity customerCommunity)
        {
            long key = 0;
            var communities = QueryAsync<CustomerCommunity?>(e => e.CustomerId == customerCommunity.CustomerId && e.CommunityId == customerCommunity.CommunityId
                        && e.IsActive == true).Result.FirstOrDefault() ?? null;

            if (communities == null)
                key = await InsertAsync<CustomerCommunity, long>(customerCommunity);
            else
                key = communities.Id;
            return key;
        }

        public async Task<int> SaveCommunityAccessRequest(CommunityAccessRequests communityAccessRequests)
        {
            var communities = QueryAsync<CustomerCommunity?>(e => e.CustomerId == communityAccessRequests.CustomerId
            && e.CommunityId == communityAccessRequests.CommunityId
            && e.IsActive == true).Result.FirstOrDefault() ?? null;
            if (communities == null)
            {
                var communityReq = QueryAsync<CommunityAccessRequests?>(e => e.CustomerId == communityAccessRequests.CustomerId
                && e.CommunityId == communityAccessRequests.CommunityId
                && e.IsActive == true && e.StatusId == 102).Result.FirstOrDefault() ?? null;
                if (communityReq == null)
                {
                    var key = await InsertAsync<CommunityAccessRequests, int>(communityAccessRequests);
                    return key;
                }
                return -1;
            }
            else
                return -1;
        }

        public async Task<int> UpdateuserDetails(CommunityAccessDTO communityAccessCodeDTO)
        {
            CustomerDetails cd = QueryAsync<CustomerDetails>(cd => cd.CustomerId == communityAccessCodeDTO.CustomerId).Result.FirstOrDefault();
            cd.CustomerId = communityAccessCodeDTO.CustomerId;
            cd.FirstName = communityAccessCodeDTO.Fullname;
            cd.Email = communityAccessCodeDTO.Email;
            cd.UsertypeId = 104;
            cd.CustomerTypeId = 103;
            var fields = Field.Parse<CustomerDetails>(x => new
            {
                x.FirstName,x.Email,x.UsertypeId,x.CustomerTypeId

            });
            var updaterow = Update<CustomerDetails>(entity: cd, fields: fields);
            return updaterow;
        }

        public async Task<List<MembershipType>> GetMasterMembershipTypeAsync()
        {
            return QueryAll<MembershipType>().Where(e => e.IsActive == true).ToList();
        }
        public async Task<List<CommunityAccessType>> GetMasterAccessTypeAsync(long SubscriptionType)
        {
            if (SubscriptionType == 101)
                return QueryAll<CommunityAccessType>().Where(e => e.IsActive == true && e.Id != 104).ToList();
            else
                return QueryAll<CommunityAccessType>().Where(e => e.IsActive == true && e.Id == 104).ToList();
        }

        public async Task<dynamic> updatemembersubscription(long CommunityId, decimal Price, long MembershipType, string CommunityUrl, long AccessType)
        {
            try
            {
                return await ExecuteNonQueryAsync("Exec [dbo].[Usp_updatemembersubscription] " + CommunityId + "," + Price + "," + MembershipType + ",'" + CommunityUrl + "'," + AccessType);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        public async Task<List<CommunityAccessRequests>> GetRequest(long communityid)
        {
            var results = await ExecuteQueryAsync<CommunityAccessRequests>("exec [dbo].[Usp_Community_RequestDetails] " + communityid);
            if (results != null)
                return (List<CommunityAccessRequests>)results;
            else
                return null;
        }
        public async Task<List<CommunityAccessRequests>> GetRequestEmail(long communityid, long Id)
        {
            var results = await ExecuteQueryAsync<CommunityAccessRequests>("exec [dbo].[Usp_Community_RequestEmailDetails] " + communityid + ", " + Id);
            if (results != null)
                return (List<CommunityAccessRequests>)results;
            else
                return null;
        }

        public async Task<int> UpdateIsStatus(long Id, long StatusId, long Communityid)
        {

            try
            {
                CommunityAccessRequests update = new CommunityAccessRequests();
                update.Id = Id;
                update.StatusId = StatusId;
                update.UpdateModifiedByAndDateTime();
                var fields = Field.Parse<CommunityAccessRequests>(x => new
                {
                    x.StatusId,
                    x.ModifiedBy,
                    x.ModifiedDate
                });
                var updaterow = await UpdateAsync<CommunityAccessRequests>(entity: update, fields: fields);
                return updaterow;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<bool> CancelCommunitySubscriptions()
        {
            var result  = true;
            return result;
        }

        public async Task<List<Groups>> GetCommunityGroupList(long communityId)
        {
            return ExecuteQuery<Groups>("exec dbo.usp_GetCommunityMembers " + communityId.ToString()).ToList<Groups>();

        }

        public async  Task<IEnumerable<Customers>> GetTotalMembers()
        {
            return QueryAll<Customers>().Where(e => e.IsActive == true).ToList();
        }
    }
}


