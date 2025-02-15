using AutoMapper;
using Circular.Core.Entity;
using Circular.Data.Repositories.User;
using Circular.Framework.Utility;
using Circular.Services.CommunityManagement;
using Circular.Services.Finance;
using Circular.Services.Safety;
using Circular.Services.User;
using CircularHQ.Business;
using CircularHQ.Models;
using Microsoft.AspNetCore.Mvc;


namespace CircularHQ.Controllers
{
    public class MemberManagementController : Controller
    {
        private readonly ICommunityManagementService _CommunityManagementService;
        private readonly IFinanceService _FinanceService;
        private readonly IMapper _mapper;
        private readonly IHelper _helper;
        private readonly IGlobal _global;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CurrentUser currentUser;
        private readonly ICustomerService _CustomerServives;
        private readonly ISafetyService _safetyService;
        public HQCommunityManagementModel HQCommunityMember = new HQCommunityManagementModel();

        public MemberManagementController(ICommunityManagementService CommunityManagementService, IMapper mapper, IWebHostEnvironment webHostEnvironment, IConfiguration configuration
           , IHelper helper, IHttpContextAccessor httpContextAccessor, ICustomerRepository customerRepository, IFinanceService financeService,ISafetyService safetyService)
        {
            _CommunityManagementService = CommunityManagementService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _config = configuration;
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _FinanceService = financeService ?? throw new ArgumentNullException(nameof(financeService));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _global = new Global(_httpContextAccessor, _config, customerRepository);
            _safetyService = safetyService ?? throw new ArgumentNullException();

        }


        [Route("MemberManagement/")]
        public async Task<IActionResult> MemberManagement()
        {
            HQCommunityMember.HQAllMemberDetails = await _CommunityManagementService.GetAllMemberDetails(0, 0);
            return View("MemberManagement",HQCommunityMember);
        }

        public async Task<IActionResult> GetMemberDetails(long Id)
        {
            var result = await _CommunityManagementService.GetAllMemberDetails(0,Id);

            if (result is not null)
            {
                return Json(new { success = true, message = "", data = result });
            }
            else
            {
                return Json(new { success = false, });
            }
        }


        public async Task<IActionResult> DownloadQRCode(long Id)
        {
            QR attendance = await _CommunityManagementService.GetMemberQRCode(Id);
            return Json(new { success = true, data = attendance });

        }


        public async Task<IActionResult> IsBlockUser(long CommunityId, long CustomerId, bool Isblocked)
        {
            
            var result = await _CommunityManagementService.IsBlockUser(CommunityId, CustomerId, Isblocked);
            if (result > 0)
            {
                return Json(new { success = true, message = "User Blocked Successfully" });
            }
            else
                return Json(new { success = false, message = "Somethings went wrong" });

        }


    }
}
