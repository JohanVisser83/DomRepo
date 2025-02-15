using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Data.Repositories.User;
using Circular.Framework.Utility;
using Circular.Services.CommunityManagement;
using Circular.Services.CreateCommunity;
using Circular.Services.Message;
using Circular.Services.User;
using CircularHQ.Business;
using CircularHQ.filters;
using CircularHQ.Models;
using Microsoft.AspNetCore.Mvc;

namespace CircularHQ.Controllers
{
    public class CommunityManagementController : Controller
    {
        private readonly IMessageService _MessageService;
        private readonly ICommunityManagementService _CommunityManagementService;
        private readonly IMapper _mapper;
        private readonly IHelper _helper;
        private readonly IGlobal _global;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CurrentUser currentUser;
        private readonly ICustomerService _CustomerServives;
        private readonly ICreateCommunityServices _CreateCommunityServices;
        public HQCommunityManagementModel community = new HQCommunityManagementModel();
        public CommunityManagementController(IMessageService MessageService, ICommunityManagementService CommunityManagementService, IMapper mapper, IWebHostEnvironment webHostEnvironment, IConfiguration configuration
           , IHelper helper, IHttpContextAccessor httpContextAccessor, ICustomerRepository customerRepository, ICreateCommunityServices createCommunityServices)
        {
            _MessageService = MessageService;
            _CommunityManagementService = CommunityManagementService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _config = configuration;
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _global = new Global(_httpContextAccessor, _config, customerRepository);
            _CreateCommunityServices = createCommunityServices;
        }

        [HttpGet]
        [ActionLog("CommunityManagement", "{0} opened CommunityManagement.")]
        [Route("CommunityManagement/")]
        public async Task<IActionResult> CommunityManagement()
        {
            CurrentUser cUser = _global.GetCurrentUser();
            long primaryId = cUser.PrimaryCommunityId;
            long loggedinuserid = cUser.Id;
            long Community = cUser.PrimaryCommunityId;

            community.Communities = await _CommunityManagementService.GetCommunityMaxMemberlist();
            community.CommunitiesCategories = await _CommunityManagementService.GetCommunityCategory();
            community.HalfBakedCommunity = await _CommunityManagementService.HalfBakedCommunityMember();
            community.lstCountryName = await _CreateCommunityServices.GetCountryName();

            return View("CommunityManagement", community);

        }

        public async Task<IActionResult> HQAddCommunity(CommunityDTO communityDTO)
        {
            if (communityDTO.OrgLogoImg != null)
                communityDTO.OrgLogo = _helper.SaveFile(communityDTO.OrgLogoImg, _global.UploadFolderPath, this.Request);

            if (communityDTO.OrgCoverImage != null)
                communityDTO.DashboardBanner = _helper.SaveFile(communityDTO.OrgCoverImage, _global.UploadFolderPath, this.Request);
            
            var result =  _CommunityManagementService.HQAddCommunity(communityDTO.AccountMobileNo,communityDTO.OrgName,communityDTO.AccessCode,communityDTO.PrimaryEmail,communityDTO.OrgLogo,communityDTO.DashboardBanner,communityDTO.Country, (long)communityDTO.CountryId, communityDTO.currencyCode,  communityDTO.CurrencyToken,communityDTO.About,communityDTO.PrimaryMobileNo, communityDTO.Website, communityDTO.OrgAddress1, communityDTO.AffiliateCode);
            
            
                return Json(new { success = true, message = "" , data= result});
           
            
            
        }
       
        public async Task<IActionResult> EditCommunityDetails(long Id)
        {
            TempData["Id"] = Id.ToString();
            return RedirectToAction("CommunityProfile", "CommunityProfile");
        }

        public async Task<IActionResult> EditHalfBakedCommunityDetails(long Ids)
        {
            TempData["FinalCommunityId"] = Ids.ToString();
            return RedirectToAction("CommunityProfile", "CommunityProfile");
        }



        //public async Task<IActionResult> EditHalfBakedCommunityDetails(long Id)
        //{
        //    long FinalCommunityId = 0;
        //    TempData["FinalCommunityId"] = Id.ToString();

        //    if (long.TryParse(TempData["FinalCommunityId"] as string, out long finalCommunityId))
        //    {
        //        FinalCommunityId = finalCommunityId;

        //    }
        //    community.HalfBakedCommunityDetails =   _CommunityManagementService.GetEditHalfBakedCommunityDetails(FinalCommunityId).Result.FirstOrDefault();
        //    if (community.HalfBakedCommunityDetails != null)
        //    {
        //        community.HalfBakedCommunityDetails.OrgLogo = community.HalfBakedCommunityDetails.OrgLogo.Replace("\\", "/");
        //        community.HalfBakedCommunityDetails.coverimage = community.HalfBakedCommunityDetails.coverimage.Replace("\\", "/");
        //    }
        //    return PartialView("~/Views/Partials/_HalfBakedCommunity.cshtml", community.HalfBakedCommunityDetails);
        //}

        public async Task<IActionResult> DeleteCommunity(long id)
        {
            long community = await _CommunityManagementService.DeleteCommunity(id);

            if (community > 0)
                return Json(new { success = true, message = "community Deleted Successfully!" });
            else
                return Json(new { success = false, message = "community Deleted not Successfully!" });
        }
        
       
    }
}
