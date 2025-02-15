using AutoMapper;
using Circular.Framework.Utility;
using Circular.Services.Community;
using Circular.Services.CommunityFeatures;
using Microsoft.AspNetCore.Mvc;
using NewCircularSubscription.Models;

namespace NewCircularSubscription.Controllers
{
    public class DiscoverController : Controller
    {
        private readonly ICommunityService _CommunityService;
        private readonly IMapper _mapper;
        private readonly IHelper _helper;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CommunityMembershipModel communityMembership = new CommunityMembershipModel();  
        public DiscoverController(ICommunityService _communityService, ICommunityFeaturesServices _communityFeatures, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IHelper helper) 
        {
           
            _CommunityService = _communityService;
            _mapper = mapper;
            _config = configuration;
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _httpContextAccessor = httpContextAccessor;
        } 


        public async Task<IActionResult> Discover(long id, string search="", long pageNumber=1, long pageSize = 10)
        {
            //pageSize = 5;
            TempData.Clear();
            communityMembership.currency = _config["Currency"];
            ViewBag.LearnMoreCircularURl = _config["LearnMoreCircularURl"];
            ViewBag.CommunityPortalURl = _config["CommunityPortalURL"];
            ViewBag.TermsOfUse = _config["TermsOfUse"];
            ViewBag.PrivacyPolicy = _config["PrivacyPolicy"];
            ViewBag.LinkedIn = _config["LinkedIn"];
            ViewBag.WhatsApp = _config["WhatsApp"];
            ViewBag.Contactus = _config["Contactus"];

            communityMembership.CommunityMembership = await _CommunityService.GetCommunities(id, search, pageNumber, pageSize);
            communityMembership.DiscoverCommunityAboutusURL = _config["DiscoverCommunityAboutusURL"];
            return View(communityMembership);  
        }

        public async Task<IActionResult> GetCommunityDetails(long id)
        {
            TempData["CommunityId"] = id.ToString();
            var result =  await _CommunityService.GetCommunities(id,"",1,10);
            return Json(new { success = true, message = "" , data= result });
        }

        public async Task<IActionResult> GetCommunitieslistSearch(string search, long page, long pageSize)
        {
           
            var Communitylist =  await _CommunityService.GetCommunities(0, search, page, pageSize);
           
            return Json(new { success = true, message = "", data= Communitylist });
        }
       
    }
}
