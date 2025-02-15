using AutoMapper;
using Circular.Framework.Utility;
using Circular.Services.Community;
using Circular.Services.CommunityFeatures;
using Microsoft.AspNetCore.Mvc;
using NewCircularSubscription.Models;

namespace NewCircularSubscription.Controllers
{
    public class ThankyouController : Controller
    {
        private readonly ICommunityService _CommunityService;
        private readonly IMapper _mapper;
        private readonly IHelper _helper;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CommunityMembershipModel communityMembership = new CommunityMembershipModel();
        public ThankyouController(ICommunityService _communityService, ICommunityFeaturesServices _communityFeatures, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IHelper helper) 
        {
            _CommunityService = _communityService;
            _mapper = mapper;
            _config = configuration;
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _httpContextAccessor = httpContextAccessor;

        }

        [Route("Thankyou/")]
        public async Task<IActionResult> Thankyou()
        {
            if (TempData["AuthCode"] == null || TempData["AuthCode"] == "")
                return RedirectToAction("Discover", "Discover");
            ViewBag.LearnMoreCircularURl = _config["LearnMoreCircularURl"];
            ViewBag.CommunityPortalURl = _config["CommunityPortalURL"];
            ViewBag.TermsOfUse = _config["TermsOfUse"];
            ViewBag.PrivacyPolicy = _config["PrivacyPolicy"];
            ViewBag.LinkedIn = _config["LinkedIn"];
            ViewBag.WhatsApp = _config["WhatsApp"];
            ViewBag.Contactus = _config["Contactus"];

            ViewBag.CircularSubscrptionFeatures = _config["CircularSubscrptionFeatures"];
            return View();
        }
        
    }
}
