using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Data.Repositories.Community;
using Circular.Framework.Middleware.Emailer;
using Circular.Framework.Notifications;
using Circular.Services.Email;
using Circular.Services.Notifications;
using MailKit.Search;
using System.Drawing.Printing;


namespace Circular.Services.Community
{
    public class CommunityService : ICommunityService
    {
        private readonly ICommunityRepository _communityRepository;
        private readonly INotificationService _notificationService;
        IMailService _mailService;

        public CommunityService(ICommunityRepository communityRepository, INotificationService notificationService, IMailService mailService)
        {
            this._communityRepository = communityRepository;
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        }


        public async Task<long> AddIsBlocked(long communityId, long customerId, bool Isblocked)
        {
            return await _communityRepository.AddIsBlocked(communityId, customerId, Isblocked);
        }
        public async Task<long> UpdateIsBlocked(long communityId, long customerId, bool Isblocked)
        {
            return await _communityRepository.UpdateIsBlocked(communityId, customerId, Isblocked);
        }

        public async Task<int> CommunityAsync(Communities communities)
        {
            communities.FillDefaultValues();
            return await _communityRepository.CommunityAsync(communities);
        }
        public async Task<Communities?> GetCommunityInfo(long id)
        {
            return await _communityRepository.GetCommunityInfo(id);
        }
        public async Task<long> UpdateCommunityInfo(Communities communities)
        {
            communities.FillDefaultValues();
            return await _communityRepository.UpdateCommunityInfo(communities);
        }
        public async Task<int> AddSocialMedia(Communities communities)
        {
            communities.FillDefaultValues();
            return await _communityRepository.AddSocialMedia(communities);
        }
        public async Task<Communities?> GetSocialMedia(long id)
        {
            return await _communityRepository.GetSocialMedia(id);
        }
        public async Task<long> UpdateSocialMedia(Communities communities)
        {
            communities.FillDefaultValues();
            return await _communityRepository.UpdateSocialMedia(communities);
        }
        public async Task<int> AddStaff(CommunityTeamProfile communityTeamProfile)
        {
            communityTeamProfile.FillDefaultValues();
            return await _communityRepository.AddStaff(communityTeamProfile);
        }
        public async Task<List<CommunityTeamProfile>> GetCommunityStaffAsync(long primaryId)
        {
            return await _communityRepository.GetCommunityStaffAsync(primaryId);
        }
        public async Task<List<CommunityTeamProfile>> GetCommunityTeamProfile(CommunityTeamProfile communityStaffs)
        {
            return await _communityRepository.GetCommunityTeamProfile(communityStaffs);
        }
        public async Task<int> DeleteCommunityStaffAsync(long id)
        {
            return await _communityRepository.DeleteCommunityStaffAsync(id);
        }
        public async Task<CommunityTeamProfile> GetCommunityStaffById(long Id)
        {
            return await _communityRepository.GetCommunityStaffById(Id);
        }
        public async Task<long> UpdateCommunityStaff(CommunityTeamProfile communityStaff)
        {
            return await _communityRepository.UpdateCommunityStaff(communityStaff);
        }
        public List<CustomerCommunity>? SearchCommunity(string communityName, long? CommunityId, long pagesize, long pagenumber, string search, long customerId)
        {
            return _communityRepository.SearchCommunity(communityName, CommunityId, pagesize, pagenumber, search, customerId);
        }




        public async Task<int> SaveCustomerCommunity(CustomerCommunity customerCommunity)
        {
            customerCommunity.FillDefaultValues();

            //var Toemail = await SendEmailSigupUser((long)customerCommunity.CustomerId);
            var username = await GetUserName((long)customerCommunity.CustomerId);
            var id = await _communityRepository.SaveCustomerCommunity(customerCommunity);
            return id;
            //MailRequest mailRequest = new MailRequest();
            //mailRequest.FromUserId = customerCommunity.CustomerId;
            //mailRequest.ReferenceId = id;
            //mailRequest.To = Toemail[0].PrimaryEmail;

            //MailSettings mailSettings = _mailService.EmailParameter(MailType.Welcome_Message, ref mailRequest);
            //string body = mailRequest.Body;
            //string[] PlaceHolders = { "$FirstName" };
            //string[] Values = { username[0].FirstName };
            //if (!string.IsNullOrEmpty(body) && (PlaceHolders.Length == Values.Length))
            //{
            //    for (int index = 0; index < PlaceHolders.Length; index++)
            //        body = body.Replace(PlaceHolders[index], Values[index]);
            //}
            //mailRequest.Body = body;
            //int a = Convert.ToInt16(await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings));
            //return a;

        }
        public async Task<List<Customers>> SendEmailSigupUser(long Id)
        {
            return await _communityRepository.SendEmailSigupUser(Id);
        }
        public async Task<List<CustomerDetails>> GetUserName(long Id)
        {
            return await _communityRepository.GetUserName(Id);
        }

        //public async Task<int> SaveCustomerCommunity(CustomerCommunity customerCommunity)
        //{
        //    customerCommunity.FillDefaultValues();
        //    return await _communityRepository.SaveCustomerCommunity(customerCommunity);
        //}





        public async Task<IEnumerable<CustomerCommunity>?> CommunityListByCustomerId(long customerId)
        {
            return await _communityRepository.CommunityListByCustomerId(customerId);
        }
        public async Task<CustomerCommunity> GetFirstCommunityOfCustomerId(long CustomerId)
        {
            return CommunityListByCustomerId(CustomerId).Result.FirstOrDefault() ?? null;
        }
        public async Task<List<Groups>> GetCommunityGroups(long communityId)
        {
            return await _communityRepository.GetCommunityGroups(communityId);
        }

        public async Task<List<CustomerDetails>> GetCommunityUser(long communityId)
        {
            return await _communityRepository.GetCommunityUser(communityId);
        }

        public async Task<int> DeleteCustomerCommunity(long communityId, string UserId)
        {
            return await _communityRepository.DeleteCustomerCommunity(communityId, UserId);
        }
        public async Task<IEnumerable<Features>?> Features(long communityId, long loggedInUserId)
        {
            return await _communityRepository.Features(communityId,loggedInUserId);
        }
        public async Task<IEnumerable<Features>?> Features_App(long communityId, long loggedInUserId)
        {
            return await _communityRepository.Features_App(communityId, loggedInUserId);
        }
        public async Task<IEnumerable<AdminFeature>?> AdminFeature(long LoggedInUser, long communityId)
        {
            return await _communityRepository.AdminFeature(LoggedInUser, communityId);
        }

        public async Task<IEnumerable<CommunityStaffs>?> Staff(long communityId)
        {
            return await _communityRepository.Staff(communityId);
        }
        public async Task<IEnumerable<CommunityTeamProfile>?> CommunityTeamProfile(long communityId)
        {
            return await _communityRepository.CommunityTeamProfile(communityId);
        }

        public async Task<IEnumerable<CustomerDetails>> GetCommunityOrganizers(long communityId)
        {
            return await _communityRepository.GetCommunityOrganizers(communityId);

        }

        public async Task<IEnumerable<CustomerCommunity>?> GetCommunityMembers(long communityId)
        {
            return await _communityRepository.GetCommunityMembers(communityId);
        }
        public async Task<IEnumerable<CustomerDetails>?> GetCommunityMemberDetails(long communityId, int UserTypeId, long CustomerId, int IncludeCurrentCustomer, int showInactive = 0)
        {
            return await _communityRepository.GetCommunityMemberDetails(communityId, UserTypeId, CustomerId, IncludeCurrentCustomer, showInactive);
        }
        public async Task<IEnumerable<CustomerGroups>?> GetCommunityGroupMembers(long groupId)
        {
            return await _communityRepository.GetCommunityGroupMembers(groupId);
        }


        public async Task<IEnumerable<CustomerSubscriptionStatus>> GetcustomerSubscriptionStatus()
        {
            return await _communityRepository.GetcustomerSubscriptionStatus();
        }

        public async Task<int> ChangeCustomersubScriptionStatus(long userId, int? subscriptionStatusId)
        {
            return await _communityRepository.ChangeCustomersubScriptionStatus(userId, subscriptionStatusId);
        }

        public async Task<int> ChangePaymentStatus(long userId, int? paymentStatusId)
        {
            return await _communityRepository.ChangePaymentStatus(userId, paymentStatusId);
        }

        public async Task<IEnumerable<CustomerMembershipPaymentStatus>> GetCustomerMembershipPaymentStatus()
        {
            return await _communityRepository.GetCustomerMembershipPaymentStatus();
        }

        #region
        public async Task<int> AddBusinessIndex(CustomerBusinessIndex customerBusinessIndex)
        {
            customerBusinessIndex.FillDefaultValues();
            return await _communityRepository.AddBusinessIndex(customerBusinessIndex);
        }
        public async Task<IEnumerable<CustomerBusinessIndex>?> GetBusinessIndex(long CommunityId, long? id, long? UserId, int BusinessCategoryId,
            string? searchText, int pageNumber, int pageSize, bool IsPendingBusinessOnly)
        {
            return await _communityRepository.GetBusinessIndex(CommunityId, id, UserId, BusinessCategoryId,
             searchText, pageNumber, pageSize, IsPendingBusinessOnly);
        }

        public async Task<long> ChangeIsBusinessApproved(long Id, bool IsBusinessApproved, long LoggedInUserId)
        {
            var business = await SendEmailBusinessUser((long)Id);
            var username = await GetCustomerDetails((long)Id);
            var id = await _communityRepository.ChangeIsBusinessApproved(Id, IsBusinessApproved, LoggedInUserId);
            MailRequest mailRequest = new MailRequest();
            mailRequest.FromUserId = Id;
            mailRequest.ReferenceId = id;
            mailRequest.To = business[0].CompanyEmail ?? "";
            MailSettings mailSettings;
            if (IsBusinessApproved == true)
            {
                mailSettings = _mailService.EmailParameter(MailType.BusinessApprove, ref mailRequest);
                _notificationService.Notify(NotificationTypes.Business_Approved,
                        NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", business[0].OwnerId.ToString()),
                        "Circular", "You just got your business approved.", Id, 0, false, "", business[0].OwnerId ?? 0, business[0].CommunityId, "", business[0].OwnerId ?? 0);
            }
            else
            {
                mailSettings = _mailService.EmailParameter(MailType.BusinessDeclined, ref mailRequest);
                _notificationService.Notify(NotificationTypes.Business_Declined,
                        NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", business[0].OwnerId.ToString()),
                        "Circular", "Your business listing is declined. Please feel free to resubmit it again.", Id, 0, false, "", business[0].OwnerId ?? 0, business[0].CommunityId, "", business[0].OwnerId ?? 0);

            }
            mailRequest.Body = mailRequest.Body.Replace("$user first name", username[0].FirstName ?? "");
            mailRequest.Body = mailRequest.Body.Replace("$business name", business[0].CompanyName ?? "");
            var businessnotapprove = Convert.ToInt16(await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings));
            return businessnotapprove;
        }

        public async Task<long> ChangeIsAdmin(long Id, bool IsAdmin, long communityId)
        {
            var result = await _communityRepository.ChangeIsAdmin(Id, IsAdmin, communityId);
            return result;
        }

        public async Task<List<CustomerBusinessIndex>> SendEmailBusinessUser(long Id)
        {
            return await _communityRepository.SendEmailBusinessUser(Id);
        }
        public async Task<List<CustomerDetails>> GetCustomerDetails(long Id)
        {
            return await _communityRepository.GetCustomerDetails(Id);
        }
        public async Task<long> DeleteBusiness(CustomerBusinessIndex business)
        {
            return await _communityRepository.DeleteBusiness(business);
        }
        public async Task<long> EditBusiness(CustomerBusinessIndex business)
        {
            return await _communityRepository.EditBusiness(business);
        }

        #endregion

        #region "Jobs"
        public async Task<long> AddJobPosting(Jobs jobs)
        {
            jobs.FillDefaultValues();
            return await _communityRepository.AddJobPosting(jobs);
        }
        public async Task<long> EditJobPosting(Jobs jobs)
        {
            jobs.FillDefaultValues();
            return await _communityRepository.EditJobPosting(jobs);
        }
        public async Task<long> DeleteJobPosting(Jobs jobs)
        {
            return await _communityRepository.DeleteJobPosting(jobs);
        }
        public async Task<long> UpdateViewCount(Jobs jobs)
        {
            return await _communityRepository.UpdateViewCount(jobs);
        }
        public async Task<long> ApplyJob(JobApplication jobApplication)
        {
            jobApplication.FillDefaultValues();

            try
            {
                MailRequest mailRequest = new MailRequest();
                var jobApplicatant = await _communityRepository.ApplyJob(jobApplication);
                var account_sentEmail = await _communityRepository.GetCustomer(jobApplication.JobId);


                mailRequest.FromUserId = jobApplication.CustomerID;
                mailRequest.ReferenceId = jobApplication.JobId;
                MailSettings mailSettings = _mailService.EmailParameter(MailType.JobPostApplicant, ref mailRequest);
                foreach (var items in account_sentEmail)
                {
                    mailRequest.To = items.CompanyEmail;
                }
                mailRequest.Body = mailRequest.Body.Replace("$jobtitle", account_sentEmail.FirstOrDefault().JobTitle);
                mailRequest.Body = mailRequest.Body.Replace("$recipientname", account_sentEmail.FirstOrDefault().CompanyName);
                mailRequest.Body = mailRequest.Body.Replace("$Applicant'sName", jobApplication.Name);
                mailRequest.Body = mailRequest.Body.Replace("$JobRole", account_sentEmail.FirstOrDefault().Position);
                mailRequest.Body = mailRequest.Body.Replace("$Applicant'sEmail", jobApplication.EmailId);
                mailRequest.Body = mailRequest.Body.Replace("$Applicant'sPhoneNumber", jobApplication.PhoneNumber);
                mailRequest.Body = mailRequest.Body.Replace("$salaryexpectation", jobApplication.SalaryExpectation);
                mailRequest.Body = mailRequest.Body.Replace("$number", jobApplication.IDNumber);
                mailRequest.Body = mailRequest.Body.Replace("$coverlettercontent", jobApplication.CV);

                _mailService.SaveAndSendMailAsync(mailRequest, mailSettings).GetAwaiter().GetResult();

                return await _communityRepository.ApplyJob(jobApplication);

            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<IEnumerable<Jobs>> GetCustomer(long customerId)
        {
            return await _communityRepository.GetCustomer(customerId);
        }


        public async Task<IEnumerable<Jobs>?> GetJobPosting(long BusinessId, long id, long customerId, int jobCategoryId,
            string searchText, long communityId, int pageNumber, int pageSize)
        {
            return await _communityRepository.GetJobPosting(BusinessId, id, customerId, jobCategoryId, searchText, communityId, pageNumber, pageSize);
        }
        public async Task<long> ChangeIsJobApproved(long Id, bool IsApproved, long LoggedInUserId)
        {

            var lstuserlist = await SendEmailJobUser((long)Id);
            //var username = await GetJobCompanyName((long)Id);

            var id = await _communityRepository.ChangeIsJobApproved(Id, IsApproved, LoggedInUserId);

            MailRequest mailRequest = new MailRequest();
            mailRequest.FromUserId = Id;
            mailRequest.ReferenceId = id;
            mailRequest.To = lstuserlist[0].CompanyEmail ?? "";
            MailSettings mailSettings;
            if (IsApproved == true)
            {

                mailSettings = _mailService.EmailParameter(MailType.JobApproved, ref mailRequest);
                _notificationService.Notify(NotificationTypes.Job_Approved,
                        NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", lstuserlist[0].CustomerId.ToString()),
                        "Circular", "You just got your job approved.", Id, 0, false, "", lstuserlist[0].CustomerId, lstuserlist[0].CommunityId, "", lstuserlist[0].CustomerId);

            }
            else
            {

                mailSettings = _mailService.EmailParameter(MailType.JobDecline, ref mailRequest);

                _notificationService.Notify(NotificationTypes.Job_Approved,
                        NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", lstuserlist[0].CustomerId.ToString()),
                        "Circular", "Your business listing is declined. Please feel free to resubmit it again.", Id, 0, false, "", lstuserlist[0].CustomerId, lstuserlist[0].CommunityId, "", lstuserlist[0].CustomerId);

            }
            mailRequest.Body = mailRequest.Body.Replace("$user first name", Convert.ToString(lstuserlist[0].YourName));
            mailRequest.Body = mailRequest.Body.Replace("$job post name", Convert.ToString(lstuserlist[0].JobTitle));
            var jobnotdone = Convert.ToInt16(await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings));
            return jobnotdone;
        }

        public async Task<List<Jobs>> SendEmailJobUser(long Id)   //SendEmailSigupUser
        {
            return await _communityRepository.SendEmailJobUser(Id);
        }
        //public async Task<List<Jobs>> GetJobCompanyName(long Id)
        //{
        //    return await _communityRepository.GetJobCompanyName(Id);
        //}
        //public async Task<long> DashboardIsJobApprove(long Id)
        //{
        //    // return await _communityRepository.DashboardIsJobApprove(Id);

        //    var lstuserlist = await SendEmailJobUser((long)Id);
        //    //var username = Convert.ToString(await GetJobCompanyName((long)Id));

        //    var id = await _communityRepository.DashboardIsJobApprove(Id);
        //    MailRequest mailRequest = new MailRequest();
        //    mailRequest.FromUserId = Id;
        //    mailRequest.ReferenceId = id;
        //    mailRequest.To = lstuserlist[0].CompanyEmail ?? "";

        //    MailSettings mailSettings = _mailService.EmailParameter(MailType.JobApproved, ref mailRequest);
        //    mailRequest.Body = mailRequest.Body.Replace("$user first name", Convert.ToString(lstuserlist[0].YourName));
        //    mailRequest.Body = mailRequest.Body.Replace("$job post name", Convert.ToString(lstuserlist[0].CompanyName));

        //    var jobapprove = Convert.ToInt16(await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings));
        //    return jobapprove;
        //}


        #endregion


        public async Task<int> NewCustomerGroup(Groups groups)
        {
            groups.FillDefaultValues();
            return await _communityRepository.NewCustomerGroup(groups);
        }
        public async Task<IEnumerable<Groups>?> GetCustomerGroup(long communityId)
        {
            return await _communityRepository.GetCustomerGroup(communityId);
        }
        public async Task<long> DeleteCustomGroup(long Id)
        {
            return await _communityRepository.DeleteCustomGroup(Id);
        }


        public List<dynamic> GetCommunityNetwork(long CommunityId, long? LoggedInUserId, string? SearchText, int IsFriend, int pageNumber, int pageSize)
        {
            return _communityRepository.GetCommunityNetwork(CommunityId, LoggedInUserId, SearchText, IsFriend, pageNumber, pageSize);
        }

        public long ActionOnNetwork(long ToId, long FromId, int FriendRequestStatusId, Customers currentCustomer)
        {
            var result = _communityRepository.ActionOnNetwork(ToId, FromId, FriendRequestStatusId);

            string senderName = currentCustomer.CustomerDetails?.FirstName + ' ' + currentCustomer.CustomerDetails?.LastName;

            if (FriendRequestStatusId == 102)
            {
                _notificationService.Notify(Framework.Notifications.NotificationTypes.Friend_Request_Received,
                    NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", ToId.ToString()),
                senderName, "You received a new friend request from " + senderName, result, 0, false, "",
                FromId, currentCustomer.PrimaryCommunity.CommunityId ?? 0, "", ToId);

                _notificationService.Notify(Framework.Notifications.NotificationTypes.Friend_Request_Sent,
                NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", FromId.ToString()),
                senderName, "Your friend request has been sent successfully.", result, 0, false, "",
                FromId, currentCustomer.PrimaryCommunity.CommunityId ?? 0, "", FromId);
            }
            else if (FriendRequestStatusId == 101)
            {
                _notificationService.Notify(Framework.Notifications.NotificationTypes.Friend_Request_Accepted,
                NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", ToId.ToString()),
                senderName, "Your friend request has been accepted by " + senderName, result, 0, false, "",
                FromId, currentCustomer.PrimaryCommunity.CommunityId ?? 0, "", ToId);
            }

            return result;
        }


        //linked member
        //public async Task<int> GetLinkedMember(string Name, long Loggedinuser)
        //{
        //    return await _communityRepository.GetLinkedMember(Name, Loggedinuser);
        //}
        public async Task<List<Fundraiser>> GetFundraisers(long communityid)
        {
            return await _communityRepository.GetFundraisers(communityid);
        }

        public async Task<List<Fundraiser>> ViewFundraisersAsync(long Id)
        {
            return await _communityRepository.ViewFundraisersAsync(Id);
        }
        public async Task<int> UpdateCollected(long id, bool Iscollected)
        {
            return await _communityRepository.UpdateCollected(id, Iscollected);
        }

        public async Task<bool> UpdateFundraisersAsync(string Title, long Amount, DateTime ExpiryDate, string PDFLink, string Description, string FormLink, long Id, long Community, long OrganizerId, string ImagePath)
        {
            return await _communityRepository.UpdateFundraisersAsync(Title, Amount, ExpiryDate, PDFLink, Description, FormLink, Id, Community, OrganizerId, ImagePath);
        }

        public async Task<bool> UploadImageAndFile5Async(long FundraiserId, string ImagePath)
        {
            return await _communityRepository.UploadImageAndFile5Async(FundraiserId, ImagePath);
        }
        public async Task<List<Fundraiser>> ViewArchivedFundraisersAsync(long Id)
        {
            return await _communityRepository.ViewArchivedFundraisersAsync(Id);
        }
        public async Task<int> DeleteArchivedFundraisersAsync(long Id)
        {
            return await _communityRepository.DeleteArchivedFundraisersAsync(Id);
        }
        public FundListResponse GetFundraiserAsync(long CommunityId, long? FundraiserId)
        {
            return _communityRepository.GetFundraiserAsync(CommunityId, FundraiserId);
        }

        public async Task<List<Fundraiser>> GetFundraiserAsync(long CommunityId)
        {
            return await _communityRepository.GetFundraiserAsync(CommunityId);
        }
        public async Task<List<FundraiserType>> Gettypeoffundraiser()
        {
            return await _communityRepository.Gettypeoffundraiser();
        }
        public async Task<bool> SaveNewCompaignAsync(long Communityid, long FundraiserTypeId, string FundraiserTitle, long OrganizerId, decimal ProductAmount, DateTime ExpiryDate, string PDFLink, string Description, string FormHyperlink, string ImagePath)
        {
            return await _communityRepository.SaveNewCompaignAsync(Communityid, FundraiserTypeId, FundraiserTitle, OrganizerId, ProductAmount, ExpiryDate, PDFLink, Description, FormHyperlink, ImagePath);
        }

        public async Task<int> ArchiveFundraisersAsync(long Id)
        {
            return await _communityRepository.ArchiveFundraisersAsync(Id);
        }
        public async Task<List<Fundraiser>> GetArchiveFundraisers(long communityid)
        {
            return await _communityRepository.GetArchiveFundraisers(communityid);
        }
        public long PayFundraiser(long FundraiserTypeId, long PayForUserId, decimal Amount, long LoggedInUserId, string Currency)
        {
            return _communityRepository.PayFundraiser(FundraiserTypeId, PayForUserId, Amount, LoggedInUserId, Currency);

        }

        public async Task<int> AddUserInGroup(CustomerGroups customerGroups)
        {
            customerGroups.FillDefaultValues();
            return await _communityRepository.AddUserInGroup(customerGroups);
        }

        public async Task<IEnumerable<dynamic>> ShowCustomGroupList(long id)
        {
            return await _communityRepository.ShowCustomGroupList(id);
        }

        public async Task<IEnumerable<dynamic>> GetFundhubActiveOrders(long loggedinuser, long communityId, bool isCollected)
        {
            try
            {
                return await _communityRepository.GetFundhubActiveOrders(loggedinuser, communityId, isCollected);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<FundraiserCollection>> GetViewPayments(long Id)
        {
            return await _communityRepository.GetViewPayments(Id);
        }

        public async Task<long> DeleteGroupUsers(long Id)
        {
            return await _communityRepository.DeleteGroupUsers(Id);
        }

        public async Task<int> DeleteCircleInfoItem2Async(long Id)
        {
            return await _communityRepository.DeleteCircleInfoItem2Async(Id);
        }

        public async Task<bool> SaveAmountCompaignAsync(long Communityid, long FundraiserTypeId, string FundraiserTitle, long OrganizerId, decimal ProductAmount, string PDFLink, string Description, string ImagePath, DateTime ExpiryDate)
        {
            return await _communityRepository.SaveAmountCompaignAsync(Communityid, FundraiserTypeId, FundraiserTitle, OrganizerId, ProductAmount, PDFLink, Description, ImagePath, ExpiryDate);
        }

        public async Task<IEnumerable<CommunityDetails>> GetCommunities(long CommunityId, string search, long pageNumber, long pageSize)
        {
            return await _communityRepository.GetCommunities(CommunityId, search, pageNumber, pageSize);
        }

        public async Task<long> GetCommunityId(string URL)
        {
            return await _communityRepository.GetCommunityId(URL);
        }
        public async Task<string> GetCommunityURL(long Id)
        {
            return await _communityRepository.GetCommunityURL(Id);
        }
        public int CheckValidAccessCode(string? accessCode, long communityId)
        {
            return _communityRepository.CheckValidAccessCode(accessCode, communityId);
        }

        public async Task<long> SaveNewCustomerCommunity(CustomerCommunity customerCommunity)
        {
            customerCommunity.FillDefaultValues();
            customerCommunity.IsPrimary = true;
            var id = await _communityRepository.SaveCustomerCommunity(customerCommunity);
            return id;
        }

        public async Task<int> SaveCommunityAccessRequest(CommunityAccessRequests communityAccessRequests)
        {
            communityAccessRequests.FillDefaultValues();
            var id = await _communityRepository.SaveCommunityAccessRequest(communityAccessRequests);
            return id;
        }

        public async Task<int> UpdateuserDetails(CommunityAccessDTO communityAccessCodeDTO)
        {
            return await _communityRepository.UpdateuserDetails(communityAccessCodeDTO);
        }
        public async Task<List<MembershipType>> GetMasterMembershipTypeAsync()
        {
            return await _communityRepository.GetMasterMembershipTypeAsync();
        }
        public async Task<List<CommunityAccessType>> GetMasterAccessTypeAsync(long SubscriptionType)
        {
            return await _communityRepository.GetMasterAccessTypeAsync(SubscriptionType);
        }

        public async Task<dynamic> updatemembersubscription(long CommunityId, decimal Price, long MembershipType, string CommunityUrl, long AccessType)
        {
            return await _communityRepository.updatemembersubscription(CommunityId, Price, MembershipType, CommunityUrl, AccessType);
        }
        public async Task<List<CommunityAccessRequests>> GetRequest(long communityid)
        {
            return await _communityRepository.GetRequest(communityid);
        }
        public async Task<List<CommunityAccessRequests>> GetRequestEmail(long communityid, long Id)
        {
            return await _communityRepository.GetRequestEmail(communityid, Id);
        }



        public async Task<int> UpdateIsStatus(long Id, long StatusId, long Communityid)
        {
            try
            {
                var getemail = await _communityRepository.GetRequestEmail(Communityid, Id);
                var id = await _communityRepository.UpdateIsStatus(Id, StatusId, Communityid);

                MailRequest mailRequest = new MailRequest();
                mailRequest.FromUserId = getemail[0].CustomerId;
                mailRequest.ReferenceId = id;
                mailRequest.To = getemail[0].Email;

                MailSettings mailSettings = null;
                if (StatusId == 101)
                {
                     mailSettings = _mailService.EmailParameter(MailType.RequestApproval, ref mailRequest);
                    _notificationService.Notify(NotificationTypes.Request_Approved,
                   NotificationTopics.Circular_user_ReferenceId.ToString().Replace("ReferenceId", getemail[0].CustomerId.ToString()),
                   "Circular", "You just got your request approved.", Id, 0, false, "", getemail[0].CustomerId, getemail[0].CommunityId, "", getemail[0].CustomerId);
                }
                else if(StatusId==103)
                {
                     mailSettings = _mailService.EmailParameter(MailType.RequestDeclined, ref mailRequest);
                    
                }
           

                string body = mailRequest.Body;
                string[] PlaceHolders = { "$user" , "$communityname" };
                string[] Values = { getemail[0].FirstName, getemail[0].CommunityName };
                if (!string.IsNullOrEmpty(body) && (PlaceHolders.Length == Values.Length))
                {
                    for (int index = 0; index < PlaceHolders.Length; index++)
                        body = body.Replace(PlaceHolders[index], Values[index]);
                }
                mailRequest.Body = body;
                if (mailSettings != null)
                {
                    await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);
                }
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async  Task<bool> CancelCommunitySubscriptions()
        {
            return await _communityRepository.CancelCommunitySubscriptions();
        }

        public async  Task<List<Groups>> GetCommunityGroupList(long communityId)
        {
            return await _communityRepository.GetCommunityGroupList(communityId);
        }

        public async Task<IEnumerable<Customers>> GetTotalMembers()
        {
            return await _communityRepository.GetTotalMembers();
        }
    }

}

