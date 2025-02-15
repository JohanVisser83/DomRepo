using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Framework.Utility;
using Circular.Services.CommunityFeatures;
using Circular.Services.CreateCommunity;
using CircularSubscriptions.Models;
using Microsoft.AspNetCore.Mvc;

namespace CircularSubscriptions.Controllers
{
    public class FeaturesController : Controller
    {
        private readonly ICommunityFeaturesServices _CommunityFeaturesService;
        private readonly IMapper _mapper;
        private readonly IHelper _helper;


        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor; 
        public CommunityFeaturesModel communityFeatures = new CommunityFeaturesModel();

        public FeaturesController(ICommunityFeaturesServices _communityFeatures, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IHelper helper)
        {
            _CommunityFeaturesService = _communityFeatures;
            _mapper = mapper;
            _config = configuration;
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("Features/")]
        public async Task<IActionResult> Index()
        {
           if (TempData["AuthCode"] == null || TempData["AuthCode"] == "")
                return RedirectToAction("EmailVerification", "EmailVerification");
            communityFeatures.AllSubscriptionfeature = await _CommunityFeaturesService.GetCommunityFeatures();
            communityFeatures.lstfeature = communityFeatures.AllSubscriptionfeature.Where(x => x.Code == "F-001").ToList();
            communityFeatures.lstSupportfeature = communityFeatures.AllSubscriptionfeature.Where(x => x.Code == "Support-001").ToList();
            communityFeatures.lstHelpReqfeature = communityFeatures.AllSubscriptionfeature.Where(x => x.Code == "HelpReq-001").ToList();
            communityFeatures.redirectContactSales = _config["ContactSales"];
            TempData["CustomerId"] = TempData["CustomerId"];
            TempData["AuthCode"] = TempData["AuthCode"];
            TempData["UserName"] = TempData["UserName"];
            TempData["TempCommunityId"] = TempData["TempCommunityId"];
            TempData["Mobile"] = TempData["Mobile"];
            TempData["FirstName"] = TempData["FirstName"];
            TempData["LastName"] = TempData["LastName"];
            return View("Features",communityFeatures);
        }

        [HttpPost]
        public async Task<IActionResult> SaveFeatureDetails(SelectedCommunityFeaturesDTO selectedCommunityFeaturesDTO)
        {
            try
            {
                selectedCommunityFeaturesDTO.CommunityId = long.Parse(TempData["TempCommunityId"].ToString());
                selectedCommunityFeaturesDTO.CustomerId = long.Parse(TempData["CustomerId"].ToString());
                SelectedCommunityFeatures selectedCommunityFeatures = _mapper.Map<SelectedCommunityFeatures>(selectedCommunityFeaturesDTO);
                var result = await _CommunityFeaturesService.SaveFeatureDetails(selectedCommunityFeatures);
                decimal ActualCostThisMonth = 0;
                //if (result > 0)
                //{
                    if (_config["IfProrated"] == "TRUE")
                    {
                        decimal totalcost = selectedCommunityFeaturesDTO.monthlysubscription;
                        int currentdate = DateTime.Now.Day;
                        int totaldaysinmonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                        int remainingdays = totaldaysinmonth - currentdate + 1;
                        ActualCostThisMonth = Decimal.Round(((totalcost * remainingdays) / totaldaysinmonth), 2);
                        ActualCostThisMonth = ActualCostThisMonth + selectedCommunityFeaturesDTO.addons + selectedCommunityFeaturesDTO.onceOff;
                    }
                    else
                        ActualCostThisMonth = selectedCommunityFeaturesDTO.monthlysubscription + selectedCommunityFeaturesDTO.addons + selectedCommunityFeaturesDTO.onceOff;


                    TempData["TempCommunityId"] = selectedCommunityFeaturesDTO.CommunityId.ToString();
                    TempData["CustomerId"] = selectedCommunityFeaturesDTO.CustomerId.ToString();
                    TempData["Monthlycost"] = selectedCommunityFeaturesDTO.Totalmonthlysubscription.ToString();
                    TempData["ActualCostThisMonth"] = ActualCostThisMonth.ToString();
                    TempData["Mobile"] = TempData["Mobile"];
                    TempData["UserName"] = TempData["UserName"];
                    TempData["FirstName"] = TempData["FirstName"];
                    TempData["LastName"] = TempData["LastName"];
                    return Json(new { success = true, message = "", data = result });
                //}
                //else
                //    return Json(new { success = false, message = "", data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, data = 0 });
            }

        }
    }
}
