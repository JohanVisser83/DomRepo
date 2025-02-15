using AutoMapper;
using Circular.Framework.Utility;
using Circular.Services.Community;
using Circular.Services.CommunityFeatures;
using Microsoft.AspNetCore.Mvc;
using NewCircularSubscription.Models;

namespace NewCircularSubscription.Controllers
{
    public class CommunityLinkController : Controller
    {
        private readonly ICommunityService _CommunityService;
        private readonly IMapper _mapper;
        private readonly IHelper _helper;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CommunityMembershipModel communityMembership = new CommunityMembershipModel();
        public CommunityLinkController(ICommunityService _communityService, ICommunityFeaturesServices _communityFeatures, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IHelper helper)
        {
            _CommunityService = _communityService;
            _mapper = mapper;
            _config = configuration;
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _httpContextAccessor = httpContextAccessor;
        }

        [Route("CommunityLink/")]
        public async Task<IActionResult> CommunityLink()
        {
            if (TempData["CommunityURL"] is null)
                return RedirectToAction("Discover", "Discover");

            ViewBag.LearnMoreCircularURl = _config["LearnMoreCircularURl"];
            ViewBag.CommunityPortalURl = _config["CommunityPortalURL"];
            ViewBag.TermsOfUse = _config["TermsOfUse"];
            ViewBag.PrivacyPolicy = _config["PrivacyPolicy"];
            ViewBag.DiscoverURL = _config["DiscoverURL"];

            ViewBag.LinkedIn = _config["LinkedIn"];
            ViewBag.WhatsApp = _config["WhatsApp"];
            ViewBag.Contactus = _config["Contactus"];

            string CommunityURL = TempData["CommunityURL"].ToString();
            
            communityMembership.CommunityURL = _config["DiscoverURL"].ToString() + "/explore/" + CommunityURL;
            return View(communityMembership);
        }
    }
}
