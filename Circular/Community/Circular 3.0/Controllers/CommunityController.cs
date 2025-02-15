using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Data.Repositories.User;
using Circular.Framework.Utility;
using Circular.Services.Community;
using Circular.Services.Message;
using Circular.Services.User;
using CircularWeb.Business;
using CircularWeb.filters;
using CircularWeb.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nancy.Diagnostics.Modules;
using OpenIddict.Client;
using Quartz.Impl.AdoJobStore.Common;
using System.Net.Http.Headers;
using System.Security.Claims;
using Tweetinvi.Core.Events;
using Tweetinvi.Parameters;

namespace CircularWeb.Controllers
{
    [Authorize]
    public class CommunityController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IHelper _helper;
        private readonly IGlobal _global;

        private readonly ICustomerService _customerService;
        private readonly ICommunityService _communityService;
        private readonly IMessageService _MessageService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CommunityModel communityModel = new CommunityModel();
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CurrentUser currentUser;
        public AccountModel accountModel = new AccountModel();
        List<string> uploadedpaths = new List<string>();
        List<string> uploadpdffiless = new List<string>();
        private readonly IConfiguration _configuration;
        private IServiceProvider _provider;
        private string OIDCUrl;


        public CommunityController(ICommunityService communityService, ICustomerService customerService, IMapper mapper, IHelper helper,
            IWebHostEnvironment webHostEnvironment, IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor, IMessageService messageService, ICustomerRepository customerRepository, IServiceProvider provider)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _communityService = communityService ?? throw new ArgumentNullException(nameof(communityService));
            _MessageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            _mapper = mapper;
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _global = new Global(_httpContextAccessor, _config, customerRepository);
            _configuration = configuration;
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            if (string.IsNullOrEmpty(configuration["OIDCUrl"]))
                throw new ArgumentNullException("configuration : OIDCUrl is not defined in App Setting");
            OIDCUrl = Convert.ToString(configuration["OIDCUrl"]);


        }
        [HttpGet]
        [Route("Community/Community")]
        [ActionLog("Community", "{0} opened community.")]
        public async Task<IActionResult> Community()
        {
            try
            {
                CurrentUser cUser = _global.GetCurrentUser();
                long primaryId = cUser.PrimaryCommunityId;
                long loggedinuserid = cUser.Id;
                long Community = cUser.PrimaryCommunityId;
                communityModel.CommunityLogo = cUser.CustomerInfo.PrimaryCommunity.CommunityLogo;
                communityModel.CommunityFeatures = _communityService.Features(primaryId,loggedinuserid).Result.ToList();
                //
                communityModel.IsOwner = cUser.CustomerInfo.PrimaryCommunity.OwnerCustomerId == loggedinuserid? true:false;


                List<CommunityAccessRequests> requests = _communityService.GetRequestEmail(cUser.PrimaryCommunityId, 0).Result.ToList();
                communityModel.RequestStatus = requests.Where(cm => cm.StatusId == 102);

                List<CustomerDetails> CommunityMembers = _communityService.GetCommunityMemberDetails(cUser.PrimaryCommunityId
                    , 0, cUser.Id, 0, 0).Result.ToList();
                communityModel.Members = CommunityMembers;
                communityModel.CommunityStaff = await _communityService.GetCommunityStaffAsync(primaryId);
                communityModel.customerSubscriptionStatuses = await _communityService.GetcustomerSubscriptionStatus();
                communityModel.CustomerMembershipPaymentStatus = await _communityService.GetCustomerMembershipPaymentStatus();
                communityModel.GetBusinessIndex = await _communityService.GetBusinessIndex(primaryId, null, null, 0, "", 0, 100, false);
                communityModel.JobPostingList = await _communityService.GetJobPosting(0, -1, 0, 0, "", primaryId, 0, 100);
                communityModel.GetCustomGroups = await _communityService.GetCustomerGroup(Community);

                communityModel.Organizers = await _communityService.GetCommunityOrganizers(cUser.PrimaryCommunityId);
                communityModel.MembershipType = await _communityService.GetMasterMembershipTypeAsync();
                //communityModel.AccessType = await _communityService.GetMasterAccessTypeAsync();

                communityModel.lstFundraisers = await _communityService.GetFundraisers(Community);
                communityModel.lsttypeoffundraiser = await _communityService.Gettypeoffundraiser();
                communityModel.currencyModel.CurrencyCode = _global.currentUser.Currency;

                communityModel.requestlist = await _communityService.GetRequest(primaryId);
                communityModel.exploreURL = Convert.ToString(_config["ExploreURL"]);
                communityModel.SubscriptionStatus = cUser?.SubscriptionStatus;
                if (!communityModel.IsFeatureAvailable("C-004"))
                    throw new ArgumentNullException("Unauthroized : You dont have permission to access this functionality.");
                else
                    return View(communityModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet]
        [ActionLog("Community", "{0} Get member information.")]
        public async Task<IActionResult> GetMasterMembershipType()
        {
            var result = await _communityService.GetMasterMembershipTypeAsync();
            return Json(new { data = result, success = true });
        }
        [HttpPost]
        [ActionLog("Community", "{0} Get AccessType information.")]
        public async Task<IActionResult> GetMasterAccessType(long SubscriptionType)
        {
            var result = await _communityService.GetMasterAccessTypeAsync(SubscriptionType);
            return Json(new { data = result, success = true });
        }

        [HttpGet]
        [ActionLog("Community", "{0} fetched staff profile.")]
        public async Task<ActionResult> GetTeamProfile(CommunityTeamProfileDTO communityTeamProfileDTO)
        {
            CommunityTeamProfile communityTeamProfile = _mapper.Map<CommunityTeamProfile>(communityTeamProfileDTO);
            var result = await _communityService.GetCommunityTeamProfile(communityTeamProfile);
            return Json(new { data = result, success = true });
        }

        [HttpPost]
        [ActionLog("Community", "{0} saved community information.")]
        public async Task<ActionResult<int>> AddCommmunityInfo(CommunityDTO communityDTO)
        {
            try
            {

                if (communityDTO.OrgLogoImg != null)
                    communityDTO.OrgLogo = _helper.SaveFile(communityDTO.OrgLogoImg, _global.UploadFolderPath, this.Request);

                if (communityDTO.OrgCoverImage != null)
                    communityDTO.coverimage = _helper.SaveFile(communityDTO.OrgCoverImage, _global.UploadFolderPath, this.Request);

                if (communityDTO.DasboardBannerImg != null)
                    communityDTO.DashboardBanner = _helper.SaveFile(communityDTO.DasboardBannerImg, _global.UploadFolderPath, this.Request);


                Communities communityinfo = _mapper.Map<Communities>(communityDTO);

                var result = await _communityService.CommunityAsync(communityinfo);
                if (result > 0)
                    return Json(new { success = true, message = "Your item is now LIVE in your community" });
                else
                    return Json(new { success = false, message = "Data Not Saved Successfully!" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Oops! something went wrong" });
            }
        }

        [HttpGet]
        [ActionLog("Community", "{0} fetched community information.")]
        public async Task<IActionResult> GetCommunityInfoById(CommunityDTO communityDTO)
        {
            var communityId = _global.currentUser.PrimaryCommunityId;
            communityDTO.Id = communityId;
            var result = (await _communityService.GetCommunityInfo(communityDTO.Id));
            TempData["OrgLogo"] = result.OrgLogo;
            TempData["CoverImage"] = result.coverimage;
            TempData["DashboardBanner"] = result.DashboardBanner;
            return Json(new { data = result, success = true });
        }

        [HttpPost]
        [ActionLog("Community", "{0} updated community information.")]
        public async Task<ActionResult> UpdateCommunityInfo(CommunityDTO communityDTO)
        {
            try
            {
                communityDTO.Id = _global.GetCurrentUser().PrimaryCommunityId;

                if (communityDTO.OrgLogoImg != null)
                    communityDTO.OrgLogo = _helper.SaveFile(communityDTO.OrgLogoImg, _global.UploadFolderPath, this.Request);

                if (communityDTO.OrgCoverImage != null)
                    communityDTO.coverimage = _helper.SaveFile(communityDTO.OrgCoverImage, _global.UploadFolderPath, this.Request);


                if (communityDTO.DasboardBannerImg != null)
                    communityDTO.DashboardBanner = _helper.SaveFile(communityDTO.DasboardBannerImg, _global.UploadFolderPath, this.Request);

                communityDTO.CurrencyToken = "";
                Communities updatecommunities = _mapper.Map<Communities>(communityDTO);
                var result = await _communityService.UpdateCommunityInfo(updatecommunities);
                if (result > 0)
                    return Json(new { success = true, message = "Your item is now LIVE in your community" });
                else
                    return Json(new { success = false, message = "Data Not Saved Successfully!" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Oops! something went wrong" });
            }
        }

        [HttpPost]
        [ActionLog("Community", "{0} saved social media links.")]
        public async Task<ActionResult<int>> AddSocialMedia(CommunityDTO communityDTO)
        {
            try
            {
                Communities communities = _mapper.Map<Communities>(communityDTO);
                var result = await _communityService.AddSocialMedia(communities);
                if (result > 0)
                    return Json(new { success = true, message = "Data save Successfully !" });
                else
                    return Json(new { success = false, message = "Data not save Successfully !" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Oops! Something went wrong !" });
            }
        }

        [HttpGet]
        [ActionLog("Community", "{0} fetched social media links.")]
        public async Task<IActionResult> GetSocialMediaInfo(CommunityDTO communityDTO)
        {
            var communityId = _global.currentUser.PrimaryCommunityId;
            communityDTO.Id = communityId;
            var result = (await _communityService.GetSocialMedia(communityDTO.Id));
            return Json(new { data = result, success = true });
        }

        [HttpPost]
        [ActionLog("Community", "{0} updated social media links.")]
        public async Task<ActionResult> UpdateSocialMediaInfo(CommunityDTO communityDTO)
        {
            try
            {
                Communities communities = _mapper.Map<Communities>(communityDTO);
                var result = await _communityService.UpdateSocialMedia(communities);

                if (result > 0)
                    return Json(new { success = true, message = "Data save successfully !" });
                else
                    return Json(new { success = false, message = "Data not save successfully !" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Oops! Something went wrong !" });
            }
        }

        [HttpPost]
        [ActionLog("Community", "{0} saved community staff.")]
        public async Task<ActionResult<int>> AddCommunityStaff(CommunityTeamProfileDTO communityTeamProfileDTO)
        {
            try
            {
                communityTeamProfileDTO.CommunityId = _global.currentUser.PrimaryCommunityId;

                if (communityTeamProfileDTO.Mediafile != null)
                {
                    string UploadFolder = "/" + _config["FileUpload:FileUploadPath"].ToString();
                    communityTeamProfileDTO.ProfileImage = _helper.SaveFile(communityTeamProfileDTO.Mediafile, UploadFolder, this.Request);
                }

                CommunityTeamProfile staff = _mapper.Map<CommunityTeamProfile>(communityTeamProfileDTO);
                var result = await _communityService.AddStaff(staff);
                if (result > 0)
                    return Json(new { success = true, message = "Your item is save in your community" });
                else
                    return Json(new { success = false, message = "Data Not Saved Successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Oops! something went wrong" });
            }
        }

        [HttpPost]
        [ActionLog("Community", "{0} deleted community staff.")]
        public async Task<IActionResult> DeleteCommunityStaff(long Id)
        {
            var result = await _communityService.DeleteCommunityStaffAsync(Id);
            if (result > 0)
                return Json(new { success = true, message = "Item deleted Successfully" });
            else
                return Json(new { success = true, message = " Item not deleted Successfully" });
        }
        [ActionLog("Community", "{0} fetched community staff.")]
        public async Task<IActionResult> GetCommunityStaffById(long Id)
        {
            var result = await _communityService.GetCommunityStaffById(Id);
            if (result != null)
                return Json(new { success = true, message = "Data fetch Successfully", data = result });
            else
                return Json(new { success = false, message = "No Data Found", data = "" });
        }
        [HttpPost]
        [ActionLog("Community", "{0} updated community staff.")]
        public async Task<ActionResult> UpdateCommmunityStaff(CommunityTeamProfileDTO communityTeamProfileDTO)
        {
            try
            {
                long result = 0;
                if (communityTeamProfileDTO != null)
                {
                    communityTeamProfileDTO.CommunityId = _global.GetCurrentUser().PrimaryCommunityId;
                    if (communityTeamProfileDTO.ProfileImg != null)
                    {
                        communityTeamProfileDTO.ProfileImage = _helper.SaveFile(communityTeamProfileDTO.ProfileImg, _global.UploadFolderPath, this.Request);
                    }

                    CommunityTeamProfile communityStaffs = _mapper.Map<CommunityTeamProfile>(communityTeamProfileDTO);
                    if (communityTeamProfileDTO.Id > 0)
                        result = await _communityService.UpdateCommunityStaff(communityStaffs);
                    else
                    {
                        if (string.IsNullOrEmpty(communityStaffs.ProfileImage))
                            return Json(new { success = false, message = "Oops. Something went wrong. Please check the data and try again!" });

                        result = await _communityService.UpdateCommunityStaff(communityStaffs);
                    }
                }

                if (result > 0)
                    return Json(new { success = true, message = "Your item is now Live in your community" });
                else
                    return Json(new { success = false, message = "Data not save Successfully !" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Oops! Something went wrong." });
            }
        }

        [ActionLog("Community", "{0} updated subscriptions status in students, parents and members.")]
        public async Task<IActionResult> ChangeCustomersubScriptionStatus(long userId, int? subscriptionStatusId)
        {
            try
            {
                //var customerId = _global.currentUser.Id;
                var result = await _communityService.ChangeCustomersubScriptionStatus(userId, subscriptionStatusId);

                if (result != null)
                {
                    return Json(new { success = true, message = "SubscriptionStatus changed Successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "OOPS! Something Went Wrong!" });
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [ActionLog("Community", "{0} updated subscription status in alumni.")]
        public async Task<IActionResult> ChangePaymentStatus(long userId, int? paymentStatusId)
        {
            try
            {
                //var customerId = _global.currentUser.Id;
                var result = await _communityService.ChangePaymentStatus(userId, paymentStatusId);

                if (result != null)
                {
                    return Json(new { success = true, message = "PaymentStatus changed Successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "OOPS! Something Went Wrong!" });
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [ActionLog("Community", "{0} saved new customer group.")]
        public async Task<ActionResult> AddNewCustomerGroup(GroupsDTO groupsDTO)
        {
            try
            {
                var CommunityId = _global.GetCurrentUser().PrimaryCommunityId;
                groupsDTO.CommunityID = CommunityId;
                Groups customerGroupDetails = _mapper.Map<Groups>(groupsDTO);
                var result = await _communityService.NewCustomerGroup(customerGroupDetails);

                if (result > 0)
                    return Json(new { success = true, message = "Data added Successfully." });
                else
                    return Json(new { success = false, message = "Data not added Successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Opps! Something went wrong." });
            }
        }

        [ActionLog("Community", "{0} fetched customer group.")]
        public async Task<ActionResult> ShowCustonGroup(long Id)
        {
            var result = _communityService.GetCommunityMemberDetails(_global.currentUser.PrimaryCommunityId
                    , 0, _global.currentUser.Id, 0, 0).Result.ToList().Where(Cg => Cg.UsertypeId == Id);
            if (result.Count() > 0)
                return Json(new { success = true, message = "Data added Successfully.", data = result });
            else
                return Json(new { success = false, message = "Data not added Successfully." });
        }

        [ActionLog("Community", "{0} deleted custome group.")]
        public async Task<IActionResult> DeleteCustomGroup(long Id)
        {
            var result = await _communityService.DeleteCustomGroup(Id);
            if (result > 0)
                return Json(new { success = true, message = "Data Deleted Successfully." });
            else
                return Json(new { success = false, message = "Item not deleted Successfully" });
        }

        [ActionLog("Community", "{0} business approval.")]
        public async Task<IActionResult> UpdateBusinessApproved(long Id, bool IsBusinessApproved)
        {
            long communityId = _global.currentUser.PrimaryCommunityId;
            var result = await _communityService.ChangeIsBusinessApproved(Id, IsBusinessApproved, communityId);
            if (result > 0)
                return Json(new { success = true, message = "" });
            else
                return Json(new { success = false, message = "" });
        }

        [ActionLog("Community", "{0} job approval.")]
        public async Task<IActionResult> UpdateJobApproved(long Id, bool IsApproved)
        {
            long LoggedInUserId = _global.currentUser.PrimaryCommunityId;
            var result = await _communityService.ChangeIsJobApproved(Id, IsApproved, LoggedInUserId);
            if (result > 0)

                return Json(new { success = true, message = "Job Approved Successfully" });
            else
                return Json(new { success = false, message = "Job not Approved Successfully" });

        }

        //Active tab
        [ActionLog("Community", "{0} fetched fundraisers list.")]
        public async Task<IActionResult> Fundraisers(Fundraiser obj)
        {
            long Community = _global.currentUser.PrimaryCommunityId;
            obj.CommunityId = Community;

            var lstFundraisers = await _communityService.GetFundraisers(obj.CommunityId);
            return Json(new { success = true, Data = lstFundraisers });
        }


        [ActionLog("Community", "{0} fetched fundraisers details.")]
        public async Task<IActionResult> ViewFundraisers(Fundraiser obj)
        {
            var lstViewFundraisers = await _communityService.ViewFundraisersAsync(obj.Id);

            // Return the data without modifying it
            return Json(new { success = true, Data = lstViewFundraisers });
        }



        [HttpPost]
        [ActionLog("Community", "{0} updated fundraisers.")]
        public async Task<ActionResult> UpdateFundraisers(FundraiserDTO obj)
        {
            try
            {
                DateTime expiryDateTime;
                string format = "dd/MM/yyyy HH:mm:ss";

                if (!DateTime.TryParseExact(obj.ExpiryDate.ToString(), format, null, System.Globalization.DateTimeStyles.None, out expiryDateTime))
                {

                    return Json(new { success = false, message = "Invalid date format" });
                }

                long communityId = _global.currentUser.PrimaryCommunityId;
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    string uploadFolder = "/" + _config["FileUpload:FileUploadPath"].ToString();
                    SavemultipleFilesupdate(file, uploadFolder, this.Request);
                    var data = uploadedpaths;

                    if (data.Count() > 0)
                    {
                        obj.PDFLink = data[0];
                    }
                }
                else
                {
                    if (obj.pdf == null)
                    {
                        var txtdata = await _communityService.ViewFundraisersAsync(obj.Id);
                        obj.PDFLink = txtdata[0].PDFLink;


                    }

                }
                obj.PDFLink = Convert.ToString(obj.PDFLink);

                var result = await _communityService.UpdateFundraisersAsync(
                    obj.Title,
                    (long)obj.Amount,
                  expiryDateTime,
                    obj.PDFLink,
                    obj.Description,
                    obj.FormLink,
                    obj.Id,
                    communityId,
                    obj.OrganizerId,
                    obj.ImagePath
                );

                if (result != null)
                {
                    return Json(new { success = true, message = "Your campaign has been updated successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Data Not Updated Successfully!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating the campaign" });
            }
        }


        public void SavemultipleFilesupdate(IFormFile file, string uploadFolder, HttpRequest request)
        {
            var path = "";
            var returnfilepath = "";
            for (int i = 0; i < request.Form.Files.Count; i++)
            {
                file = request.Form.Files[i];
                string url = $"{request.Scheme}://{request.Host}{request.PathBase}" + uploadFolder;
                string filesPath = Directory.GetCurrentDirectory() + uploadFolder;
                if (!Directory.Exists(filesPath))
                    Directory.CreateDirectory(filesPath);

                var uniqueFileName = GetUniqueFileNameupdate(file.FileName);


                path = Path.Combine(filesPath, uniqueFileName);
                file.CopyToAsync(new FileStream(path, FileMode.Create));
                returnfilepath = Path.Combine(url, uniqueFileName);
                uploadedpaths.Add(returnfilepath);
            }

        }
        private string GetUniqueFileNameupdate(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }

        [HttpPost]
        public async Task<ActionResult> UploadImageAndFile5(FundraiserProductImagesDTO obj)
        {
            long Community = _global.currentUser.PrimaryCommunityId;
            string CoverImage = string.Empty;
            var file = Request.Form.Files[0];
            string UploadFolder = "/" + _config["FileUpload:FileUploadPath"].ToString();
            SavemultipleFilesupdate(file, UploadFolder, this.Request);
            var data = uploadedpaths;
            if (data.Count() > 0)
            {
                CoverImage = data[0];
            }
            obj.ImagePath = Convert.ToString(CoverImage);
            var result = await _communityService.UploadImageAndFile5Async(obj.FundraiserId, obj.ImagePath);
            if (result != null)
                return Json(new { success = true, message = "Your campaign has been updated successfully" });
            else
                return Json(new { success = false, message = "Data Not Updated Successfully!" });

        }

        [ActionLog("Community", "{0} deleted image from active and archive in fundhub.")]
        public async Task<ActionResult> DeleteCircleInfoItem2(long Id)
        {
            var result = await _communityService.DeleteCircleInfoItem2Async(Id);
            if (result > 0)
                return Json(new { success = true, message = "Deleted Successfully!" });
            else
                return Json(new { success = false, message = "Not Successfully!" });
        }


        [ActionLog("Community", "{0} archived fundraisers.")]
        public async Task<ActionResult> ArchiveFundraisers(long Id)
        {
            var result = await _communityService.ArchiveFundraisersAsync(Id);
            if (result > 0)
                return Json(new { success = true, message = "Archived Successfully!" });
            else
                return Json(new { success = false, message = "Not Archived Successfully!" });
        }

        [ActionLog("Community", "{0} fetched payments.")]
        public async Task<IActionResult> ViewPayments(FundraiserCollection obj)
        {
            var lstViewPayments = await _communityService.GetViewPayments(obj.Id);
            return Json(new { success = true, Data = lstViewPayments });
        }

        //Archived tab
        [ActionLog("Community", "{0} fetched active archive fundraisers.")]
        public async Task<IActionResult> ArchiveFundraisersactive(Fundraiser obj)
        {
            long Community = _global.currentUser.PrimaryCommunityId;


            var lstArchiveFund = await _communityService.GetArchiveFundraisers(Community);
            return Json(new { success = true, Data = lstArchiveFund });
        }

        [ActionLog("Community", "{0} updated iscollected.")]
        public async Task<IActionResult> UpdateIsCollected(long Id, long Iscollected)
        {
            var result = await _communityService.UpdateCollected(Id, Convert.ToBoolean(Iscollected));
            if (result > 0)
                return Json(new { success = true, message = "Item Update Successfully" });
            else
                return Json(new { success = false, message = "Item Not Deleted Successfully" });
        }
        [ActionLog("Community", "{0} fetched archive fundraisers.")]
        public async Task<IActionResult> ViewArchivedFundraisers(Fundraiser obj)
        {
            var lstViewArchived = await _communityService.ViewArchivedFundraisersAsync(obj.Id);
            return Json(new { success = true, Data = lstViewArchived });
        }
        [ActionLog("Community", "{0} deleted archive fundraisers.")]
        public async Task<ActionResult> DeleteArchiveFundraisers(long Id)
        {
            var result = await _communityService.DeleteArchivedFundraisersAsync(Id);
            if (result > 0)
                return Json(new { success = true, message = "Deleted Successfully!" });
            else
                return Json(new { success = false, message = "Not Successfully!" });
        }

        [ActionLog("Community", "{0} saved new campaign.")]
        public async Task<IActionResult> PostNewCompaign(FundraiserDTO obj)
        {
            DateTime expiryDateTime;
            string format = "dd/MM/yyyy HH:mm:ss"; 

            if (!DateTime.TryParseExact(obj.ExpiryDate.ToString(), format, null, System.Globalization.DateTimeStyles.None, out expiryDateTime))
            {
                
                return Json(new { success = false, message = "Invalid date format" });
            }

            long Communityid = _global.currentUser.PrimaryCommunityId;
            var communityName = _global.currentUser.PrimaryCommunityName;
            string uploadpdffile = string.Empty;
            string CoverImage = string.Empty;
            string updatedcommunityName = communityName.Replace("-", " ").Replace("@", "").Replace("(", "").Replace(")", "").Replace(",", "").Replace("~", "").Replace("#", "").Replace("$", "").Replace("*", "").Replace("^", "").Replace("!", "").Replace("'", "");
            string UploadFolder = "/" + _config["FileUpload:FileUploadPath"].ToString() + "/Articles/" + updatedcommunityName.Trim();
            string _imgPath = "";

            for (int i = 0; i < obj.Images.Count; i++)
            {
                string imagepath = "";
                string imagefile = obj.Images[i].filename;
                string ImgBase64 = obj.Images[i].ImagePath;

                imagepath = _helper.ConvertBase64toImage(imagefile, ImgBase64, UploadFolder, this.Request);
                obj.Images[i].ImagePath = Convert.ToString(imagepath);

                communityModel.currencyModel.CurrencyCode = _global.currentUser.Currency;
                string _newimgPath = imagepath;
                _imgPath += _newimgPath + ",";
            }

            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                SavemultipleFiles(file, UploadFolder, this.Request);
                var data = uploadedpaths;
                if (data.Count() > 0)
                {
                    uploadpdffile = data[0];
                }
            }
            obj.PDFLink = string.IsNullOrEmpty(uploadpdffile) ? "" : Convert.ToString(uploadpdffile);

            bool result = await _communityService.SaveNewCompaignAsync(
                Communityid,
                obj.FundraiserTypeId,
                obj.FundraiserTitle,
                obj.OrganizerId,
                obj.ProductAmount,
                expiryDateTime,
                obj.PDFLink,
                obj.Description,
                obj.FormHyperlink,
                _imgPath.TrimEnd(',')
            );

            if (result)
            {
                return Json(new { success = true, message = "Your campaign has been added successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Oops! something went wrong" });
            }
        }





        public void SavemultipleFiles(IFormFile file, string uploadFolder, HttpRequest request)
        {
            var path = "";
            var returnfilepath = "";
            for (int i = 0; i < request.Form.Files.Count; i++)
            {
                file = request.Form.Files[i];
                string url = $"{request.Scheme}://{request.Host}{request.PathBase}" + uploadFolder;
                string filesPath = Directory.GetCurrentDirectory() + uploadFolder;
                if (!Directory.Exists(filesPath))
                    Directory.CreateDirectory(filesPath);

                var uniqueFileName = GetUniqueFileName(file.FileName);
                //string fileName = Path.GetFileName(uniqueFileName);

                path = Path.Combine(filesPath, uniqueFileName);
                file.CopyToAsync(new FileStream(path, FileMode.Create));
                returnfilepath = Path.Combine(url, uniqueFileName);
                uploadedpaths.Add(returnfilepath);
            }

        }
        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }



        [ActionLog("Community", "{0} fetched user list in groups.")]
        public async Task<ActionResult> GetUserContactList(string Search)
        {
            long communityId = _global.GetCurrentUser().PrimaryCommunityId;
            var userContactList = await _MessageService.GetUserContactListAsync(communityId, Search);
            var result = (userContactList?.Select(u => _mapper.Map<UserContactList>(u)).ToList());
            //return Json({ userContact, data = userContact});
            return Json(result);
        }

        [ActionLog("Community", "{0} saved members in group.")]
        public async Task<ActionResult> AddUserInGroup(CustomerGroups customerGroups)
        {


            var user = await _communityService.AddUserInGroup(customerGroups);

            return Json(user);
        }
        [ActionLog("Community", "{0} fetched all groups in groups.")]
        public async Task<ActionResult> ShowCustomGroupList(long Id)
        {
            var result = await _communityService.ShowCustomGroupList(Id);
            if (result is not null)
            {
                return Json(new { success = true, message = "", data = result });
            }
            else
            {
                return Json(new { success = false, });
            }

        }
        [ActionLog("Community", "{0} deleted members from members list in group.")]
        public async Task<IActionResult> DeleteGroupUserItem(long Id)
        {
            var result = await _communityService.DeleteGroupUsers(Id);
            if (result > 0)
                return Json(new { success = true, message = "Data Deleted Successfully." });
            else
                return Json(new { success = false, message = "Item not Deleted Successfully" });
        }

        [ActionLog("Community", "{0} fetched members list in groups.")]
        public async Task<IActionResult> GetLinkedMembers(string Id)
        {

            var result = await _customerService.GetLinkedMembers(Id);
            // var loggedInUserId = _global.GetCurrentUser().Id.ToString();
            //var result = await _customerService.GetLinkedMembers(UserId);
            return Json(new { success = true, message = "fetched succesfully", data = result });
        }


        public void SaveSingleFilesupdate(IFormFile file, string uploadFolder, HttpRequest request)
        {
            var path = "";
            var returnfilepath = "";

            file = request.Form.Files[0];
            string url = $"{request.Scheme}://{request.Host}{request.PathBase}" + uploadFolder;
            string filesPath = Directory.GetCurrentDirectory() + uploadFolder;
            if (!Directory.Exists(filesPath))
                Directory.CreateDirectory(filesPath);

            var uniqueFileName = GetUniqueFileNameupdate(file.FileName);


            path = Path.Combine(filesPath, uniqueFileName);
            file.CopyToAsync(new FileStream(path, FileMode.Create));
            returnfilepath = Path.Combine(url, uniqueFileName);
            uploadedpaths.Add(returnfilepath);


        }

        [ActionLog("Community", "{0} saved new campaign.")]
        public async Task<IActionResult> PostNewAmountFund(FundraiserDTO obj)
        {
            try
            {
                //Inside post new funds
                long Communityid = _global.currentUser.PrimaryCommunityId;
                var communityName = _global.currentUser.PrimaryCommunityName;
                string uploadpdffile = string.Empty;
                string CoverImage = string.Empty;
                string updatedcommunityName = communityName.Replace("-", "").Replace("@", "").Replace("(", "").Replace(")", "").Replace(",", "").Replace("~", "").Replace("#", "").Replace("$", "").Replace("*", "").Replace("^", "").Replace("!", "").Replace("'", "");
                string UploadFolder = "/" + _config["FileUpload:FileUploadPath"].ToString() + "/Articles/" + updatedcommunityName.Trim();
                string _imgPath = "";

                for (int i = 0; i < obj.Images.Count; i++)
                {
                    string imagepath = "";
                    string imagefile = obj.Images[i].filename;
                    string ImgBase64 = obj.Images[i].ImagePath;

                    imagepath = _helper.ConvertBase64toImage(imagefile, ImgBase64, UploadFolder, this.Request);
                    obj.Images[i].ImagePath = Convert.ToString(imagepath);



                    communityModel.currencyModel.CurrencyCode = _global.currentUser.Currency;
                    string _newimgPath = imagepath;
                    _imgPath += _newimgPath + ",";
                }

                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    SavemultipleFiles(file, UploadFolder, this.Request);
                    var data = uploadedpaths;
                    if (data.Count() > 0)
                    {
                        uploadpdffile = data[0];
                    }
                }
                obj.PDFLink = string.IsNullOrEmpty(uploadpdffile) ? "" : Convert.ToString(uploadpdffile);

                bool result = await _communityService.SaveAmountCompaignAsync(Communityid, obj.FundraiserTypeId, obj.FundraiserTitle, obj.OrganizerId, obj.ProductAmount, obj.PDFLink, obj.Description, _imgPath.Remove(_imgPath.Length - 1), obj.ExpiryDate);
                if (result)
                {
                    return Json(new { success = true, message = "Your campaign has been added successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Oops! something went wrong" });
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [ActionLog("Community", "{0} updated isBlocked.")]
        public async Task<IActionResult> UpdateIsBlock(long customerId, bool Isblocked)
        {
            long communityId = _global.GetCurrentUser().PrimaryCommunityId;
            var result = await _communityService.UpdateIsBlocked(communityId, customerId, Isblocked);
            if (result > 0)
            {
                //LogOutUser("08439fc3-ff66-4eb8-e62e-08dbfd414414");

                return Json(new { success = true, message = "Item Update Successfully" });
            }
            else
                return Json(new { success = false, message = "Item Not Deleted Successfully" });

        }

        public async Task<IActionResult> AddIsBlock(long customerId, bool Isblocked)
        {
            long communityId = _global.GetCurrentUser().PrimaryCommunityId;
            var result = await _communityService.AddIsBlocked(communityId, customerId, Isblocked);
            if (result > 0)
                return Json(new { success = true, message = "Item Update Successfully" });
            else
                return Json(new { success = false, message = "Item Not Deleted Successfully" });
        }

        [NonAction]
        public async Task<ActionResult> LogOutUser(string userId)
        {
            try
            {
                var OIDCClient = _provider.GetRequiredService<OpenIddictClientService>();
                var clientDetails = await OIDCClient.GetClientRegistrationAsync(new Uri(OIDCUrl));
                if (clientDetails != null)
                {
                    var client = _provider.GetRequiredService<HttpClient>();
                    var request = new HttpRequestMessage(HttpMethod.Post, OIDCUrl + "connect/force-logout");


                    request.Headers.Add("Authorization", $"Bearer {HttpContext.User.FindFirst(ClaimTypes.SerialNumber).Value}");

                    request.Content = new FormUrlEncodedContent(new Dictionary<string, string>{
                        //{"client_id", clientDetails.ClientId},
                        //{"client_secret", clientDetails.ClientSecret},
                        {"subject", userId}
                    });

                    var response = await client.SendAsync(request);
                    var result = "";
                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadAsStringAsync();
                        return Ok(result);
                    }
                    return BadRequest();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<IActionResult> GetMembersubscription()
        {
            long Communityid = _global.currentUser.PrimaryCommunityId;
            List<CustomerCommunity> result = _communityService.SearchCommunity("", Communityid, 1, 1, "", _global.currentUser.Id);
            if (result != null)
                return Json(new { success = true, message = "Data fetch Successfully", data = result });
            else
                return Json(new { success = false, message = "No Data Found", data = "" });
        }

        public async Task<ActionResult> updatemembersubscriptionn(decimal Price, long MembershipType, string CoummunityUrl, long AccessType)
        {
            long CommunityId = _global.currentUser.PrimaryCommunityId;
            var result = await _communityService.updatemembersubscription(CommunityId, Price, MembershipType, CoummunityUrl, AccessType);
            if (result != null)
                return Json(new { success = true, message = "", data = result });
            else
                return Json(new { success = false, message = "" });
        }

        [ActionLog("Community", "{0} updated is Approval.")]
        public async Task<IActionResult> UpdateIsStatus(long Id, long StatusId)
        {
            long Communityid = _global.currentUser.PrimaryCommunityId;
            var result = await _communityService.UpdateIsStatus(Id, StatusId, Communityid);
            if (result > 0)
                return Json(new { success = true, message = "Item Update Successfully" });
            else
                return Json(new { success = false, message = "Item Not Deleted Successfully" });
        }

        [ActionLog("Community", "{0} fetched Request details.")]
        public async Task<IActionResult> GetRequestEmail(long communityId, long Id)
        {
            long Community = _global.currentUser.PrimaryCommunityId;
            long Loggedinuser = _global.currentUser.Id;
            communityId = Community;
            var lstTicketsale = await _communityService.GetRequestEmail(communityId, Id);
            return Json(new { success = true, Data = lstTicketsale });
        }

        [ActionLog("Community", "{0} Save Approval.")]
        public async Task<IActionResult> SaveApproval(CustomerCommunityRequestDTO customerCommunityDTO)
        {
            long CommunityId = _global.currentUser.PrimaryCommunityId;
            customerCommunityDTO.CommunityId = CommunityId;
            CustomerCommunity customerCommunity = _mapper.Map<CustomerCommunity>(customerCommunityDTO);
            var result = await _communityService.SaveCustomerCommunity(customerCommunity);
            if (result > 0)
                return Json(new { success = true, message = "Item Update Successfully" });
            else
                return Json(new { success = false, message = "Item Not Deleted Successfully" });
        }

        [ActionLog("Community", "{0} IsAdmin.")]
        public async Task<IActionResult> UpdateIsAdmin(long Id, bool IsAdmin)
        {
            long communityId = _global.currentUser.PrimaryCommunityId;
            var result = await _communityService.ChangeIsAdmin(Id, IsAdmin, communityId);
            if (result > 0)
                return Json(new { success = true, message = "" });
            else
                return Json(new { success = false, message = "" });
        }
    }
}



