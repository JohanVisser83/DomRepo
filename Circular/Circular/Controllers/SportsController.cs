using AutoMapper;
using Circular.Core.DTOs;
using Circular.Data.Repositories.User;
using Circular.Filters;
using Circular.Framework.Utility;
using Circular.Services.Sports;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Circular.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SportsController : ControllerBase
    {
        private readonly ISportsService _SportsService;
        private readonly IMapper _mapper;
        private readonly IHelper _helper;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
     //   public SportsModel sportsModel = new SportsModel();

        public SportsController(ISportsService SportsService, IMapper mapper , IHelper helper, IConfiguration configuration, ICustomerRepository customerRepository, IHttpContextAccessor httpContextAccessor)
        {
            _SportsService = SportsService;
            _mapper = mapper;        
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _config = configuration;         
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

      

        [HttpGet]
        [AuthorizeOIDC]
        [Route("List")]
		[SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Sports", "{UserName}  Requested Sports")]
		public async Task<ActionResult<APIResponse>> GetSports(long CommunityId, int UpcomingOrPast)
        {
      
            var accountList = _SportsService.GetSportsAsync(CommunityId, UpcomingOrPast,true);
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = accountList;
            return Ok(apiResponse);
        }

        [HttpPost]
        [AuthorizeOIDC]
        [Route("Type")]
		[SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Sports", "{UserName}  Requested Sports Types")]
		public async Task<IActionResult> GetSportsTypes()
        {
            var result = await _SportsService.GetSportsTypes();
            APIResponse apiResponse = new APIResponse();
            if (result != null && result.Count() > 0)
                apiResponse.StatusCode = (int)APIResponseCode.Success;
            else
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            apiResponse.Data = result;
            return Ok(apiResponse);

        }

		[HttpPost]
		[AuthorizeOIDC]
		[Route("Fixtures")]
		[SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Sports", "{UserName}  Requested Sports Fixtures")]
        public async Task<ActionResult<APIResponse>> GetSportsFixtures(FixtureRequestDTO obj)
		{
            IEnumerable<GetFixtures> result = await _SportsService.GetSportsFixtures(obj.SportId, obj.CommunityId, obj.SportsTypeId);
			APIResponse apiResponse = new APIResponse();
				apiResponse.StatusCode = (int)APIResponseCode.Success;
			apiResponse.Data = result;
			return Ok(apiResponse);

		}


		[HttpPost]
        [AuthorizeOIDC]
        [Route("Fixtures/TeamMember")]
		[SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Sports", "{UserName}  Requested Team Member")]
		public async Task<IActionResult> GetTeamMember(FixtureTeamMemberRequestDTO obj)
        {
            var result = await _SportsService.GetTeamMemberAsync(obj.fixtureId);
            APIResponse apiResponse = new APIResponse();
            if (result != null && result.Count() > 0)
                apiResponse.StatusCode = (int)APIResponseCode.Success;
            else
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            apiResponse.Data = result;
            return Ok(apiResponse);

        }

        
    }
}