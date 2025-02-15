using AutoMapper;
using Circular.Framework.Utility;
using Circular.Services.Community;
using Circular.Services.CommunityFeatures;
using Microsoft.AspNetCore.Mvc;
using NewCircularSubscription.Models;

namespace NewCircularSubscription.Controllers
{
    public class CommunityController : Controller
    {
        private readonly ICommunityService _CommunityService;
        private readonly IMapper _mapper;
        private readonly IHelper _helper;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CommunityDetailsModel communitydetails = new CommunityDetailsModel();
        public CommunityController(ICommunityService _communityService, ICommunityFeaturesServices _communityFeatures, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IHelper helper) 
        {
            _CommunityService = _communityService;
            _mapper = mapper;
            _config = configuration;
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _httpContextAccessor = httpContextAccessor;
        }


       
        [Route("Community/")]
        public async Task<IActionResult> Index()
        {
            communitydetails.currency = _config["Currency"];
            ViewBag.LearnMoreCircularURl = _config["LearnMoreCircularURl"];
            ViewBag.CommunityPortalURl = _config["CommunityPortalURL"];
            ViewBag.TermsOfUse = _config["TermsOfUse"];
            ViewBag.PrivacyPolicy = _config["PrivacyPolicy"];

            ViewBag.LinkedIn = _config["LinkedIn"];
            ViewBag.WhatsApp = _config["WhatsApp"];
            ViewBag.Contactus = _config["Contactus"];

            var communityId = long.Parse(TempData["CommunityId"].ToString());
            TempData["CommunityId"] = communityId.ToString();
            communitydetails.lstCommunitydetails = await _CommunityService.GetCommunities(communityId, "", 1, 10);

            TempData["MembershipType"] = communitydetails.lstCommunitydetails?.FirstOrDefault()?.MembershipType.ToString();
            TempData["AccessType"] = communitydetails.lstCommunitydetails?.FirstOrDefault()?.AccessType.ToString();
            TempData["Price"] = communitydetails.lstCommunitydetails?.FirstOrDefault()?.Price.ToString();
            TempData["Currencycode"] = communitydetails.lstCommunitydetails?.FirstOrDefault()?.currencyCode.ToString();
            TempData["CurrencyToken"] = communitydetails.lstCommunitydetails?.FirstOrDefault()?.CurrencyToken.ToString();



            return View("Community", communitydetails);  
        }

        [Route("Explore/{CM}")]
        public async Task<IActionResult> Explore(string CM)
        {
            long CommunityId = await _CommunityService.GetCommunityId(CM);
            if (CommunityId == 0) 
                return RedirectToAction("Discover", "Discover");
            else 
            {
                ViewBag.LearnMoreCircularURl = _config["LearnMoreCircularURl"];
                ViewBag.CommunityPortalURl = _config["CommunityPortalURL"];
                ViewBag.TermsOfUse = _config["TermsOfUse"];
                ViewBag.PrivacyPolicy = _config["PrivacyPolicy"];
                ViewBag.DiscoverCommunityAboutusURL = _config["DiscoverCommunityAboutusURL"];
                ViewBag.LinkedIn = _config["LinkedIn"];
                ViewBag.WhatsApp = _config["WhatsApp"];
                ViewBag.Contactus = _config["Contactus"];

                communitydetails.currency = _config["Currency"];
                TempData["CommunityId"] = CommunityId.ToString();
                communitydetails.lstCommunitydetails = await _CommunityService.GetCommunities(CommunityId, "", 1, 10);

                TempData["MembershipType"] = communitydetails.lstCommunitydetails?.FirstOrDefault()?.MembershipType.ToString();
                TempData["AccessType"] = communitydetails.lstCommunitydetails?.FirstOrDefault()?.AccessType.ToString();
                TempData["Price"] = communitydetails.lstCommunitydetails?.FirstOrDefault()?.Price.ToString();



                return View("Community", communitydetails);
            }
        }
    }
}
