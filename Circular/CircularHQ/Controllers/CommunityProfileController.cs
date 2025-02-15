using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Data.Repositories.User;
using Circular.Framework.Utility;
using Circular.Services.Community;
using Circular.Services.CommunityManagement;
using Circular.Services.Finance;
using Circular.Services.Message;
using Circular.Services.User;
using CircularHQ.Business;
using CircularHQ.filters;
using CircularHQ.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CircularHQ.Controllers
{
    public class CommunityProfileController : Controller
    {
        private readonly IMessageService _MessageService;
        private readonly IFinanceService _FinanceService;
        private readonly ICommunityService _communityService;
        private readonly ICommunityManagementService _CommunityManagementService;
        private readonly IMapper _mapper;
        private readonly IHelper _helper;
        private readonly IGlobal _global;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomerService _CustomerServives;
        private readonly CurrentUser currentUser;
        public HQCommunityManagementModel community = new HQCommunityManagementModel();
        public HQMessageModel messageModel = new HQMessageModel();
        public CommunityProfileController(ICommunityManagementService CommunityManagementService, ICommunityService communityService, IMapper mapper, IWebHostEnvironment webHostEnvironment, IConfiguration configuration
           , IHelper helper, IHttpContextAccessor httpContextAccessor, ICustomerRepository customerRepository, IMessageService MessageService, IFinanceService FinanceService)
        {

            _MessageService = MessageService;
            _FinanceService = FinanceService;
            _communityService = communityService ?? throw new ArgumentNullException(nameof(communityService));
            _CommunityManagementService = CommunityManagementService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _config = configuration;
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _global = new Global(_httpContextAccessor, _config, customerRepository);
        }


        [Route("CommunityProfile/")]
        public async Task<IActionResult> CommunityProfile()
        {
            long Id = 0;
            long FinalCommunityId = 0;

            CurrentUser cUser = _global.GetCurrentUser();
            long primaryId = cUser.PrimaryCommunityId;
            long loggedinuserid = cUser.Id;
            long Community = cUser.PrimaryCommunityId;

            if (TempData.ContainsKey("Id") || TempData.ContainsKey("FinalCommunityId"))
            {
                if (long.TryParse(TempData["Id"] as string, out long tempId))
                {
                    Id = tempId;
                    TempData.Keep();
                }
                if (long.TryParse(TempData["FinalCommunityId"] as string, out long finalCommunityId))
                {
                    FinalCommunityId = finalCommunityId;
                    TempData.Keep();
                }
            }

            community.ddlCommunities = await _MessageService.GetCommunitiesAsync();
            community.CommunitiesEditDetails =  _CommunityManagementService.GetEditCommunityDetails(Id).Result.FirstOrDefault();
            if(community.CommunitiesEditDetails != null)
            {
                community.CommunitiesEditDetails.OrgLogo = community.CommunitiesEditDetails.OrgLogo.Replace("\\", "/");
                community.CommunitiesEditDetails.coverimage = community.CommunitiesEditDetails.coverimage.Replace("\\", "/");
            }
            
            community.HalfBakedCommunityDetails = _CommunityManagementService.GetEditHalfBakedCommunityDetails(FinalCommunityId).Result.FirstOrDefault();
            if (community.HalfBakedCommunityDetails != null)
            {
                community.HalfBakedCommunityDetails.OrgLogo = community.HalfBakedCommunityDetails.OrgLogo.Replace("\\", "/");
                community.HalfBakedCommunityDetails.coverimage = community.HalfBakedCommunityDetails.coverimage.Replace("\\", "/");
            }
            community.Groups = await _communityService.GetCustomerGroup(Community);

            community.hQCommunityTransactionDetails = await _FinanceService.GetHQCommunityTransactions(Id);

            return View("CommunityProfile", community);
        }



        [HttpPost]
        [ActionLog("CommunityProfile", "{0} updated community information.")]
        public async Task<ActionResult> UpdateCommunityInfo(CommunityDTO communityDTO)
        {
            try
            {
                //communityDTO.Id = _global.GetCurrentUser().PrimaryCommunityId;

                if (communityDTO.OrgLogoImg != null)
                    communityDTO.OrgLogo = _helper.SaveFile(communityDTO.OrgLogoImg, _global.UploadFolderPath, this.Request);

                if (communityDTO.OrgCoverImage != null)
                    communityDTO.coverimage = _helper.SaveFile(communityDTO.OrgCoverImage, _global.UploadFolderPath, this.Request);


                if (communityDTO.DasboardBannerImg != null)
                    communityDTO.DashboardBanner = _helper.SaveFile(communityDTO.DasboardBannerImg, _global.UploadFolderPath, this.Request);

                Communities updatecommunities = _mapper.Map<Communities>(communityDTO);
                var result = await _CommunityManagementService.UpdateCommunityInfo(updatecommunities);
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
        [ActionLog("CommunityProfile", "{0} updated HalfBaked community information.")]
        public async Task<ActionResult> UpdateHalfbakedCommunityInfo(CommunityDTO communityDTO)
        {
            try
            {
                //communityDTO.Id = _global.GetCurrentUser().PrimaryCommunityId;

                if (communityDTO.OrgLogoImg != null)
                    communityDTO.OrgLogo = _helper.SaveFile(communityDTO.OrgLogoImg, _global.UploadFolderPath, this.Request);

                if (communityDTO.OrgCoverImage != null)
                    communityDTO.coverimage = _helper.SaveFile(communityDTO.OrgCoverImage, _global.UploadFolderPath, this.Request);


                if (communityDTO.DasboardBannerImg != null)
                    communityDTO.DashboardBanner = _helper.SaveFile(communityDTO.DasboardBannerImg, _global.UploadFolderPath, this.Request);

                Communities updatecommunities = _mapper.Map<Communities>(communityDTO);
                var result = await _CommunityManagementService.UpdateCommunityInfo(updatecommunities);
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

        


        public async Task<IActionResult> CreatelandingPage(AdvertisementDTOs advertisementDTOs)
        {
            Advertisement advertisement = _mapper.Map<Advertisement>(advertisementDTOs);
            var result = await _CommunityManagementService.CreatelandingPage(advertisement);
            return Json(result);
        }


        public async Task<IActionResult> GetCommunityMemberList(long Id)
        {
            CurrentUser cUser = _global.GetCurrentUser();
            List<CustomerDetails> CommunityMembers = _communityService.GetCommunityMemberDetails(Id, 0, cUser.Id, 0, 0).Result.ToList();

            if (CommunityMembers != null)
                return Json(new { success = true, message = "", data= CommunityMembers });
            else
                return Json(new { success = false, message = "" });



            return Json(community);
        }

        public async Task<IActionResult> BindTransactionHistory()
        {
            long communityId = _global.GetCurrentUser().PrimaryCommunityId;
            community.hQCommunityTransactionDetails = await _FinanceService.GetHQCommunityTransactions(communityId);
            if (community.hQCommunityTransactionDetails != null)
                return Json(new { success = true, message = "Your item is now LIVE in your community", data = community.hQCommunityTransactionDetails });
            else
                return Json(new { success = false, message = "Data Not Saved Successfully!" });
        }

        public async Task<ActionResult> GroupMemberList(long Id)
        {
            var key = await _communityService.GetCustomerGroup(Id);
            var GroupmemberList = (key?.Select(u => _mapper.Map<Groups>(u)).ToList());

            if (GroupmemberList != null)
                return Json(new { success = true, message = "", data = GroupmemberList });
            else
                return Json(new { success = false, message = "" });
        }

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

        public async Task<ActionResult> AddNewCustomerGroup(GroupsDTO groupsDTO)
        {
            try
            {
                
                Groups customerGroupDetails = _mapper.Map<Groups>(groupsDTO);
                groupsDTO.IsAddedByHQ = true;
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


        public async Task<ActionResult> GetUserContactList(string Search)
        {
            long communityId = _global.GetCurrentUser().PrimaryCommunityId;
            var userContactList = await _MessageService.GetUserContactListAsync(communityId, Search);
            var result = (userContactList?.Select(u => _mapper.Map<UserContactList>(u)).ToList());
            return Json(result);
        }


        public async Task<ActionResult> AddUserInGroup(CustomerGroups customerGroups)
        {

            var user = await _communityService.AddUserInGroup(customerGroups);
            return Json(user);
        }

        public async Task<IActionResult> CommunityQR(long Id)
        {
            QR attendance = await _CommunityManagementService.GetMemberQRCode(Id);
            return Json(new { success = true, data = attendance });

        }

    }
}
