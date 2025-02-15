using RepoDb;
using Microsoft.Data.SqlClient;
using Circular.Core.Entity;
using System.Data;
using Circular.Framework.Utility;
using Microsoft.AspNetCore.Http;
using Circular.Core.DTOs;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using static System.Net.WebRequestMethods;
using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Management;
using System;
using System.Reflection;

namespace Circular.Data.Repositories.User
{
    public class CustomerRepository : DbRepository<SqlConnection>, ICustomerRepository
    {
        private readonly IHelper _helper;
        IHttpContextAccessor _httpContextAccessor;
        string Password = "";
        public CustomerRepository(string connectionString, IHelper helper, IHttpContextAccessor httpContextAccessor) : base(connectionString)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        #region "Customers"
        public Customers? getcustomerByUserName(string userName, bool isRelatedEntityFill)
        {
            Customers customer = QueryAsync<Customers?>(e => (e.Mobile == userName || e.PrimaryEmail == userName) && e.IsActive == true).Result.FirstOrDefault() ?? null;
            if (isRelatedEntityFill && customer != null)
                customer = getcustomerbyId(customer.Id, true);

            return customer;
        }
        public Customers? getcustomerByUserId(Guid usercode, bool isRelatedEntityFill)
        {
            Customers customer = QueryAsync<Customers?>(e => e.UserId == usercode && e.IsActive == true).Result.FirstOrDefault() ?? null;
            if (isRelatedEntityFill && customer != null)
                customer = getcustomerbyId(customer.Id, true);

            return customer;
        }

        public Customers getCommunityMemberDetailsById(long id)
        {
            Customers customer = new Customers();
            customer = QueryAsync<Customers?>(e => e.Id == id && e.IsActive == true).Result.FirstOrDefault() ?? null;

            if (customer != null)
            {
                var details = ExecuteQueryMultiple("exec [dbo].[usp_Community_GetDetailsbyId]" + " " + id.ToString() + ";");

                customer.CustomerDetails = details.Extract<CustomerDetails>().FirstOrDefault();
                customer.CustomerCommunities = details.Extract<CustomerCommunity>().ToList();
                List<Gateways> pgs = details.Extract<Gateways>().ToList();
                List<Settings> settings = details.Extract<Settings>().ToList();
                List<SubscriptionDetails> subscriptionDetails = details.Extract<SubscriptionDetails>().ToList();
                customer.SubscriptionStatus = subscriptionDetails?.FirstOrDefault()?.SubscriptionStatus;

                foreach (CustomerCommunity customerCommunity in customer.CustomerCommunities)
                {
                    customerCommunity.SponsorInformation = GetSponsorInformation(customerCommunity.CommunityId ?? 0);
                    customerCommunity.Paymentgateways = pgs.Where<Gateways>(com => com.CommunityId == customerCommunity.CommunityId).ToList<Gateways>() ?? new List<Gateways>();
                    customerCommunity.CommunitySettings = settings.Where<Settings>(s => s.CurrencyCode == customerCommunity.CurrencyToken).ToList();
                   

                }
            }
            return customer;
        }



        public Customers getCommunityMemberDetailsByIdHQ(long id)
        {
            Customers customer = new Customers();
            customer = QueryAsync<Customers?>(e => e.Id == id && e.IsActive == true).Result.FirstOrDefault() ?? null;

            if (customer != null)
            {
                var details = ExecuteQueryMultiple("exec [dbo].[usp_HQCommunity_GetDetailsbyId]" + " " + id.ToString() + ";");

                customer.CustomerDetails = details.Extract<CustomerDetails>().FirstOrDefault();
                customer.CustomerCommunities = details.Extract<CustomerCommunity>().ToList();
                List<Gateways> pgs = details.Extract<Gateways>().ToList();
                List<Settings> settings = details.Extract<Settings>().ToList();

                foreach (CustomerCommunity customerCommunity in customer.CustomerCommunities)
                {
                    customerCommunity.SponsorInformation = GetSponsorInformation(customerCommunity.CommunityId ?? 0);
                    customerCommunity.Paymentgateways = pgs.Where<Gateways>(com => com.CommunityId == customerCommunity.CommunityId).ToList<Gateways>() ?? new List<Gateways>();
                    customerCommunity.CommunitySettings = settings.Where<Settings>(s => s.CurrencyCode == customerCommunity.CurrencyToken).ToList();


                }
            }
            return customer;
        }
        public Customers getcustomerbyId(long id, bool isRelatedEntityFill)
        {
            try
            {


            Customers customer = new Customers();
            customer = QueryAsync<Customers?>(e => e.Id == id && e.IsActive == true).Result.FirstOrDefault() ?? null;
            if (customer != null && string.IsNullOrEmpty(customer.Passcode))
            {
                customer.IsPasscodeSet = false;
                customer.Passcode = "";
            }
            if (!isRelatedEntityFill)
                return customer;
            else
            {
                if (customer != null)
                {
                    var details =  ExecuteQueryMultiple("exec [dbo].[usp_Customer_GetDetails]" + " " + id.ToString() + ";");

                    CustomerInfo customerInfo = details.Extract<CustomerInfo>().FirstOrDefault();
                    customer.CustomerDetails = details.Extract<CustomerDetails>().FirstOrDefault();
                    customer.AppVersions = details.Extract<AppVersions>().ToList();
                    customer.CustomerGroups = details.Extract<CustomerGroups>().ToList();
                    customer.BankAccounts = details.Extract<CustomerBankAccounts>().ToList();
                    customer.CustomerCommunities = details.Extract<CustomerCommunity>().ToList();
                    customer.privateSpaces = details.Extract<PrivateSpace>().ToList();  
                    List<CustomerPermission> cp = details.Extract<CustomerPermission>().ToList();
                    List<Gateways> pgs = details.Extract<Gateways>().ToList();
                    List<Settings> settings = details.Extract<Settings>().ToList();
                    List<CommunityExternalLinks> externalLinks = details.Extract<CommunityExternalLinks>().ToList();
                    
               


                    customer.IsPasswordSet = customerInfo.IsPasswordSet == 1 ? true:false;
                    customer.CountryFlag = customerInfo.CountryFlag;
                    customer.WalletBalance = customerInfo.WalletBalance;
                    customer.UnreadNotifications = customerInfo.UnreadNotifications;
                    customer.UnreadMessages = customerInfo.UnreadMessages;

                    string QRCodePath = Directory.GetCurrentDirectory() + "/Uploads/QRs/Customer/";
                    var browsePath = _httpContextAccessor?.HttpContext.Request.Scheme + "://" + _httpContextAccessor?.HttpContext.Request.Host + "/Uploads/QRs/Customer/";
                    string filename = _helper.EncryptUsingSHA1Hashing(customer.Mobile) + ".png";
                    _helper.GetQRCode(customer.Mobile, filename, ref QRCodePath);
                    customer.QRCode.QRPath = browsePath + filename;
                    bool IsPrimary = false;
                    foreach (CustomerCommunity customerCommunity in customer.CustomerCommunities)
                    {
                        customerCommunity.SponsorInformation = GetSponsorInformation(customerCommunity.CommunityId ?? 0);
                        if (cp != null && cp.Count > 0)
                        {
                            List<CustomerPermission> cp1 = cp.Where<CustomerPermission>(p => p.CommunityId == customerCommunity.CommunityId).ToList();
                            if (cp1 != null && cp1.Count > 0)
                            {
                                foreach (CustomerPermission permission in cp1)
                                {
                                    if (permission.PermissionId == (int)UserPermissions.EventScanner)
                                        customerCommunity.TicketScan = true;
                                    if (permission.PermissionId == (int)UserPermissions.VisitorScanner)
                                        customerCommunity.AttendanceScan = true;
                                    if (permission.PermissionId == (int)UserPermissions.TransportScanner)
                                        customerCommunity.DriverScan = true;
                                }
                            }
                        }

                        customerCommunity.Paymentgateways = pgs.Where<Gateways>(com => com.CommunityId == customerCommunity.CommunityId).ToList<Gateways>() ?? new List<Gateways>();
                        customerCommunity.CommunitySettings = settings.Where<Settings>(s => s.CurrencyCode == customerCommunity.CurrencyToken).ToList();
                        customerCommunity.ExternalLink = externalLinks.Where<CommunityExternalLinks>(c => c.CommunityId == customerCommunity.CommunityId).ToList();
                        

                            if (customerCommunity.IsPrimary == true) { IsPrimary = true; }

                    }
                        if (!IsPrimary && customer.CustomerCommunities != null && customer.CustomerCommunities.Count() > 0)
                            customer.CustomerCommunities.FirstOrDefault().IsPrimary = true;

                        if(customer.CustomerCommunities != null && customer.CustomerCommunities.Count() > 0)
                            customer.CustomerDetails.IsAdmin =  (customer.CustomerCommunities.Where(c => c.IsPrimary == true).FirstOrDefault())?.IsAdmin ?? false;

                    }
                    return customer;
            }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public SponsorInformation GetSponsorInformation(long communityId)
        {
            SponsorInformation sponsorInformation =  new SponsorInformation();
            //SponsorInformation sponsorInformation = QueryAsync<SponsorInformation>(s => s.CommunityId == communityId
            //&& s.IsActive == true)?.Result?.LastOrDefault()??new SponsorInformation();
            //if (sponsorInformation != null)
            //{
            //    sponsorInformation.SectionDetails = QueryAsync<SponsorSectionDetails>(s => s.SponsorId == sponsorInformation.SponsorId
            //        && s.IsActive == true).Result.ToList();
            //    if (sponsorInformation.SectionDetails != null)
            //        sponsorInformation.SectionDetails.ForEach(
            //            SD => SD.SectionFields = GetSponsorSectionfields(SD.Id, SD.SponsorId ?? 0)
            //        );
            //}
            return sponsorInformation;
        }
        public List<SponsorSectionfields> GetSponsorSectionfields(long sectionId, long sponsorId)
        {
            List<SponsorSectionfields> sponsorSectionfields = QueryAsync<SponsorSectionfields>(sf => sf.SectionId == sectionId
            && sf.SponsorId == sponsorId && sf.IsActive == true).Result.ToList();
            if (sponsorSectionfields != null)
            {
                sponsorSectionfields.ForEach(sSF =>

                {
                    sSF.SectionCircles = QueryAsync<SponsorSectionCircle>(sc => sc.SectionId == sSF.SectionId
                    && sc.IsActive == true).Result.ToList();

                    sSF.SectionImages = QueryAsync<SponsorSectionImages>(si => si.SectionId == sSF.SectionId
                    && si.IsActive == true && si.SponsorId == sSF.SponsorId).Result.ToList();
                }
                );
            }
            return sponsorSectionfields;

        }
        private string setPasswordStatus()
        {
            return null;
        }

        public async Task<long> Save(Customers customers)
        {
            var customer = await InsertAsync<Customers, int>(customers);
            return customer;
        }
        public Customers UpdatePasscode(long id, string passcode, bool isRelatedEntityFill)
        {
            Customers customers = new Customers();
            customers.Id = id;
            customers.Passcode = passcode; ;
            customers.UpdateModifiedByAndDateTime();
            var fields = Field.Parse<Customers>(e => new
            {
                e.Passcode,
                e.ModifiedBy,
                e.ModifiedDate
            });
            var updatedRows = Update<Customers>(entity: customers, fields: fields);
            return getcustomerbyId(id, isRelatedEntityFill);
        }
        public bool VerifyPasscode(long id, string passcode)
        {
            Customers customer = QueryAsync<Customers?>(c => c.Id == id && c.IsActive == true && c.Passcode == passcode).Result.FirstOrDefault() ?? null;
            if (customer != null)
                return true;
            else
                return false;
        }
        public async Task<int> DeActivateMyAccount(string UserId)
        {
            try
            {


                return await ExecuteQueryAsync("exec [dbo].[USP_Customer_DeactivateAccount]" + " '" + UserId.ToString() + "';").Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public List<NewsFeeds?> GetNewFeeds(long customerId, int IsArchiveUnArchiveOrAll, long Feedid)
        {
            try
            {
                var newfeedslist = ExecuteQuery<NewsFeeds>("exec dbo.USP_GetNewFeeds " + customerId + "," + IsArchiveUnArchiveOrAll + "," + Feedid);
                foreach (var feed in newfeedslist)
                {
                    feed.ArticleMedia = QueryAsync<ArticleMedia?>(c => c.NewFeedsId == feed.Id && c.IsActive == true).Result.ToList() ?? null;

                    if (!String.IsNullOrEmpty(feed.DocumentPath))
                    {
                        feed.postURL = feed.DocumentPath;

                    }
                }
                return (List<NewsFeeds?>)newfeedslist;
            }
            catch(Exception ex)
            {
                return null;
            }
            
        }
        public async Task<int> LikeArticle(LikedFeeds likedFeeds)
        {
            var result = 0;
            LikedFeeds FeedLike = QueryAsync<LikedFeeds?>(e => e.Feedid == likedFeeds.Feedid &&
            e.Userid == likedFeeds.Userid && e.IsActive == true).Result.FirstOrDefault() ?? null;
            if (FeedLike != null)
            {
                FeedLike.IsActive = false;
                FeedLike.UpdateModifiedByAndDateTime();
                FeedLike.ModifiedBy = likedFeeds.Userid;
                var fields = Field.Parse<LikedFeeds>(e => new
                {
                    e.IsActive,
                    e.ModifiedBy,
                    e.ModifiedDate
                });
                result = Update<LikedFeeds>(entity: FeedLike, fields: fields);
            }
            else
                result = await InsertAsync<LikedFeeds, int>(likedFeeds);
            return result;
        }
        #endregion

        #region "CustomerDetails"
        public async Task<int> SaveDetails(CustomerDetails customerDetails)
        {
            var i = await InsertAsync<CustomerDetails, int>(customerDetails);
            return i;
        }
        public CustomerDetails GetCustomerDetailsIdUsingId(long Id)
        {
            return QueryAsync<CustomerDetails?>(e => e.Id == Id && e.IsActive == true).Result.FirstOrDefault() ?? null;
        }
        public int GetCustomerDetailsIdUsingCustomerId(long customerId)
        {
            var customerdetails = QueryAsync<CustomerDetails?>(e => e.CustomerId == customerId && e.IsActive == true).Result;
            return (int)customerdetails.FirstOrDefault().Id;
        }
        public Customers UpdateUserType(long CustomerId, long userTypeId, long modifiedBy)
        {
            CustomerDetails customerDetails = new CustomerDetails();
            customerDetails.Id = GetCustomerDetailsIdUsingCustomerId(CustomerId);
            customerDetails.CustomerId = CustomerId;
            customerDetails.UsertypeId = userTypeId;
            customerDetails.ModifiedBy = modifiedBy;
            customerDetails.UpdateModifiedByAndDateTime();
            var fields = Field.Parse<CustomerDetails>(e => new
            {
                e.UsertypeId,
                e.ModifiedBy,
                e.ModifiedDate
            });
            var updatedRows = Update<CustomerDetails>(entity: customerDetails, fields: fields);
            return getcustomerbyId(customerDetails.CustomerId, true);
        }
        public Customers UpdateCustomerBasicDetails(CustomerDetails customerDetail)
        {
            if (customerDetail.Id <= 0)
                customerDetail.Id = GetCustomerDetailsIdUsingCustomerId(customerDetail.CustomerId);
            customerDetail.UpdateModifiedByAndDateTime();
            IEnumerable<Field> fields;
            if (customerDetail.IsSignUpFlow)
            {
                fields = Field.Parse<CustomerDetails>(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Email,
                    e.DOB,
                    e.ModifiedBy,
                    e.ModifiedDate,
                    e.Latitude,
                    e.Longitude
                });
            }
            else
            {
                fields = Field.Parse<CustomerDetails>(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Email,
                    e.DOB,
                    e.ModifiedBy,
                    e.ModifiedDate,
                    e.Latitude,
                    e.Longitude,
                    e.ProfilePic,
                    e.ClassId,
                    e.staffId,
                    e.HouseId,
                    e.IDNumber
                });
            }


            var updatedRows = Update<CustomerDetails>(entity: customerDetail, fields: fields);
            Customers customer = getcustomerbyId(customerDetail.CustomerId, true);
            if (customer != null)
            {
                customer.UpdateModifiedByAndDateTime();
                customer.PrimaryEmail = customerDetail.Email;
                var customerfields = Field.Parse<Customers>(e => new
                {
                    e.PrimaryEmail
                });
                var updatedCRows = Update<Customers>(entity: customer, fields: customerfields);
            }
            return customer;
        }
        public async Task<int> SaveDeviceDetails(CustomerDevices userDevices)
        {
            var result = await InsertAsync<CustomerDevices, int>(userDevices);
            return result;

        }


        #endregion

        #region "Linked Members"
        public async Task<IEnumerable<dynamic>?> GetLinkedMembers(string UserId)
        {
            var linkedMembers = await ExecuteQueryAsync<dynamic>(
                "exec [dbo].[USP_Customers_Link_Members]" + " '" + UserId.ToString() + "';");

            return linkedMembers;

        }
        public async Task<long> AddLinkedMembers(LinkedMembers linkedMember)
        {
            Customers customer = getcustomerbyId(linkedMember.FromCustId, true);
            if (customer != null)
            {
                if (customer.CustomerDetails != null && customer.CustomerDetails.UsertypeId == 101)
                    return 4;
            }
            else
                return 0;

            LinkedMembers _linkedMembers = QueryAsync<LinkedMembers?>(e => e.FromCustId == linkedMember.FromCustId
            && e.ToCustId == linkedMember.ToCustId && e.IsActive == true).Result.FirstOrDefault() ?? null;
            if (_linkedMembers == null)
            {
                linkedMember.FillDefaultValues();
                linkedMember.LinkingStatusId = 100;
                long id = await InsertAsync<LinkedMembers, long>(linkedMember);
                if (id != 0)
                {
                    return id;
                }
                else
                    return 0;
            }
            else
            {
                if (_linkedMembers.LinkingStatusId == 100)
                    return 2; // Request already in pending status
                else if (_linkedMembers.LinkingStatusId == 103)
                    return 5; // Request already in pending status
                else
                    return 3; // Already linked
            }

            /*
                Return
                 0 - Issue
                 >5 - Successful
                 2 - Request already in pending status
                 3 - Already linked
                 4 - Usertype not correct
                 5 - Reported
            */
        }
        public async Task<int> RemoveLinkedMembers(LinkedMembers linkedMember)
        {
            int result = 0;
            LinkedMembers _linkedMembers = QueryAsync<LinkedMembers?>(e => e.FromCustId == linkedMember.FromCustId
            && e.ToCustId == linkedMember.ToCustId && e.IsActive == true).Result.FirstOrDefault() ?? null;

            if (_linkedMembers != null)
            {
                _linkedMembers.UpdateModifiedByAndDateTime();
                _linkedMembers.IsActive = false;
                var fields = Field.Parse<LinkedMembers>(e => new
                {
                    e.IsActive,
                    e.ModifiedBy,
                    e.ModifiedDate
                });
                var updatedRows = Update<LinkedMembers>(entity: _linkedMembers);
                if (updatedRows != 0)
                    result = 1;
            }
            _linkedMembers = null;
            _linkedMembers = QueryAsync<LinkedMembers?>(e => e.FromCustId == linkedMember.ToCustId
                && e.ToCustId == linkedMember.FromCustId && e.IsActive == true).Result.FirstOrDefault() ?? null;

            if (_linkedMembers != null)
            {
                _linkedMembers.UpdateModifiedByAndDateTime();
                _linkedMembers.IsActive = false;
                var fields = Field.Parse<LinkedMembers>(e => new
                {
                    e.IsActive,
                    e.ModifiedBy,
                    e.ModifiedDate
                });
                var updatedRows = Update<LinkedMembers>(entity: _linkedMembers);
                if (updatedRows != 0)
                    result = 1;
            }


            return result;
        }
        public async Task<int> ConfirmLinkedMembers(LinkedMembers linkedMember)
        {
            LinkedMembers _linkedMembers = QueryAsync<LinkedMembers?>(e => e.FromCustId == linkedMember.FromCustId
            && e.ToCustId == linkedMember.ToCustId && e.IsActive == true && e.LinkingStatusId == 100).Result.FirstOrDefault() ?? null;
            if (_linkedMembers != null)
            {
                _linkedMembers.UpdateModifiedByAndDateTime();
                _linkedMembers.LinkingStatusId = linkedMember.LinkingStatusId;
                var fields = Field.Parse<LinkedMembers>(e => new
                {
                    e.LinkingStatusId,
                    e.ModifiedBy,
                    e.ModifiedDate
                });
                var updatedRows = Update<LinkedMembers>(entity: _linkedMembers);
                if (updatedRows != 0)
                    return 1;
                else
                    return 0;
            }
            else
                return 0;
        }


        public int CheckValidEmail(string Email)
        {
            var result = QueryAsync<Customers>(e => (e.PrimaryEmail == Email) && e.IsActive == true).Result;
            if (result.Count() > 0 && result != null)
                return (int)result.FirstOrDefault().Id;
            else
                return 0;
        }




        public async Task<CustomerDetails> CheckValidHQAdminEmail(string userName)
        {
            try
            {
                var result = await QueryAsync<Customers>(e => (e.PrimaryEmail == userName) && e.IsActive == true);
                var firstCustomer = result.FirstOrDefault();
                var result1 = QueryAsync<CustomerDetails>(c => c.CustomerId == firstCustomer.Id && c.IsActive == true).Result;
                if (result1.Count() > 0 && result1 != null)
                    return result1.FirstOrDefault();
                else
                    return null;
            }
            catch(Exception ex)
            {
                return null;
            }
        }


        public bool CheckIfExistingOwnerId(long UserId)
        {
            var result = QueryAsync<Communities>(c => c.OwnerCustomerId == UserId && c.IsActive == true).Result;
            if (result == null || result.Count() <= 0)
                return false;
            else
                return true;
        }

        public CustomerDetails GetCustomerDetails(long Id, string Email)
        {
            // return QueryAsync<CustomerDetails>(e => e.Email == Email && e.IsActive == true).Result.FirstOrDefault() ?? null;
            //var result = QueryAll<Customers>().Where(e => e.Id == Id && e.IsActive == true).ToList();
            CustomerDetails customers = new CustomerDetails();
            var tuple = QueryMultiple<CustomerDetails, Customers>(cd => cd.IsActive == true && (cd.CustomerId == Id || cd.Email == Email), cg => cg.IsActive == true && cg.Id == Id);
            if (tuple != null)
            {
                customers.Mobile = tuple.Item2.FirstOrDefault()?.Mobile ?? "";
                customers.Email = tuple.Item1.FirstOrDefault()?.Email ?? "";
                customers.FirstName = tuple.Item1.FirstOrDefault()?.FirstName ?? "";
            }
            return customers;

        }


        public async Task<int> sendOTP(CustomerPasswordChangeRequest data)
        {
            try
            {
                var key = await InsertAsync<CustomerPasswordChangeRequest, int>(data);
                return key;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        #endregion



        public Task<int> GetVerifyOTP(string PasswordActivationCode, long CustomerId)
        {

            return ExecuteQueryAsync("Exec [dbo].[USP_VerifyOTP] '" + PasswordActivationCode + "','" + CustomerId + "'").Result.FirstOrDefault();

        }


        public async Task<List<ActiveOTPDetails>> GetOTPUser()
        {
            var result =  ExecuteQueryAsync<ActiveOTPDetails>("Exec [dbo].[USp_GetActiveOTP]").Result.ToList();
            return result;
        }


        public List<Advertisement?> GetAdvertisement(long CommunityId, long advertisementId)
        {
            try
            {
                var advertisementlist = ExecuteQuery<Advertisement>("exec dbo.USP_GetAdvertisementList " + CommunityId + "," + advertisementId);


                foreach (var advertisement in advertisementlist)
                {
                    advertisement.AdvertisementMediaList = QueryAsync<AdvertisementMedia?>(c => c.AdvertisementId == advertisement.Id && c.IsActive == true).Result.ToList() ?? null;
                   
                }

                return (List<Advertisement?>)advertisementlist;
            }
            catch (Exception ex)
            {
                return null;
            }

        }



        public async Task<long> UpdateArticleViewCount(ArticleViews articleViews)
        {
            try
            {

                var result = 0;
                ArticleViews countViews = QueryAsync<ArticleViews?>(Av => Av.Feedid == articleViews.Feedid && Av.Userid == articleViews.Userid && Av.IsActive == true).Result.FirstOrDefault() ?? null;
                if (countViews != null)
                {
                    countViews.ViewCount = countViews.ViewCount + 1;
                    var fields = Field.Parse<ArticleViews>(e => new
                    {
                        e.ViewCount,
                        e.ModifiedBy,
                        e.ModifiedDate
                    });

                    result = Update<ArticleViews>(entity: countViews, fields: fields);
                }
                else
                    result = await InsertAsync<ArticleViews, int>(articleViews);

                return result;


            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public int CheckValidUser(string mobile)
        {
            var result = QueryAsync<Customers>(e => (e.Mobile == mobile) && e.IsActive == true).Result;
            if (result.Count() > 0 && result != null)
                return (int)result.FirstOrDefault().Id;
            else
                return 0;
        }


    }
}
