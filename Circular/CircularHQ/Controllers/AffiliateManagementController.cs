using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Data.Repositories.User;
using Circular.Framework.Utility;
using Circular.Services.CommunityManagement;
using Circular.Services.User;
using CircularHQ.Business;
using CircularHQ.Models;
using Microsoft.AspNetCore.Mvc;

namespace CircularHQ.Controllers
{
    public class AffiliateManagementController : Controller
    {
        
        private readonly ICommunityManagementService _CommunityManagementService;
        private readonly IMapper _mapper;
        private readonly IHelper _helper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private readonly IGlobal _global;
        private readonly CurrentUser currentUser;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomerService _CustomerServives;
        
        public AffiliateManagementModel affiliateManagementModel = new AffiliateManagementModel();
        public AffiliateManagementController(ICommunityManagementService CommunityManagementService, IMapper mapper, IWebHostEnvironment webHostEnvironment, IConfiguration configuration
           , IHelper helper, IHttpContextAccessor httpContextAccessor, ICustomerRepository customerRepository) 
        {
           
            _CommunityManagementService = CommunityManagementService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _config = configuration;
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _global = new Global(_httpContextAccessor, _config, customerRepository);

        }


        [Route("AffiliateManagement/")]
        public async Task<IActionResult> AffiliateManagement()
        {
            CurrentUser cUser = _global.GetCurrentUser();
            //long CommunityId = cUser.PrimaryCommunityId;

            affiliateManagementModel.AffiliateCode = await _CommunityManagementService.GetAllAffiliatedCode();
            affiliateManagementModel.lstaffiliateCode = await _CommunityManagementService.GetAffiliateCodelist(0);
            return View("AffiliateManagement",affiliateManagementModel);
        }

        public async Task<IActionResult> AddAffiliateCode(AffiliatedCodeDTO affiliatedCodeDTO)
        {
            CurrentUser cUser = _global.GetCurrentUser();
            AffiliatedCodeDetails affiliateCode = _mapper.Map<AffiliatedCodeDetails>(affiliatedCodeDTO);
            var result = await _CommunityManagementService.AffiliateCode(affiliateCode);
            if (result !=0)
                return Json(new { success = true, message = "" });
            else
                return Json(new { success = false, message = "An affiliate with the email, phone  already exists. Please use a different details", });
        }


        public async Task<IActionResult> UpdateAffiliateCodeDetails(AffiliatedCodeDTO affiliatedCodeDTO)
        {
            AffiliatedCodeDetails affiliateCode = _mapper.Map<AffiliatedCodeDetails>(affiliatedCodeDTO);
            var result =  _CommunityManagementService.UpdateAffiliateCodeDetails(affiliateCode);
            if (result != null)
                return Json(new { success = true, message = "" });
            else
                return Json(new { success = false, message = "" });
        }


        public async Task<IActionResult> GetEditAffiliateCode(long id)
        {
            var affiliatedata = _CommunityManagementService.GetAffiliateCodelist(id);

            if (affiliatedata != null)
                return Json(new { success = true, message = "Data fetched successfully!", data = affiliatedata });
            else
                return Json(new { success = false, message = "No Data Found!", data = "" });
        }
    }
}
