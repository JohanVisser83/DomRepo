using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Filters;
using Circular.Framework.Logger;
using Circular.Services.Master;
using Circular.Services.User;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Circular.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MasterController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMasterService _masterService;
        private readonly ICustomerService _customerService;

        private readonly ILoggerManager _logger;
        private readonly ICommon _common;
      //  private readonly IGlobal _global;

        public MasterController(IMapper mapper, IMasterService masterService, ILoggerManager logger, ICommon common
            , ICustomerService customerService, IConfiguration configuration)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _masterService = masterService ?? throw new ArgumentNullException(nameof(masterService));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
           
            _common = common;
        }


        // GET <MasterController>
      //  [AuthorizeOIDC]
        [HttpPost]
        [Route("Get")]
        [ProducesResponseType(typeof(IList<MasterDTO>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Master", "{UserName} Get Master")]
        public async Task<ActionResult<IList<MasterDTO>?>> GetMaster([FromBody] MasterTypeDTO masterTypeDTO)
        {
            return await GetMasterAsync(masterTypeDTO);
        }


        // GET <MasterController>
        [HttpPost]
        [Route("Get/Public")]
        [ProducesResponseType(typeof(IList<MasterDTO>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Master", "{UserName} Requested Master List")]
        public async Task<ActionResult<IList<MasterDTO>?>> GetMasterList([FromBody] MasterTypeDTO masterTypeDTO)
        {
            if (masterTypeDTO.masterType.ToLower() == "country".ToLower() ||
                masterTypeDTO.masterType.ToLower() == "privacypolicies".ToLower() ||
                masterTypeDTO.masterType.ToLower() == "systemalerts".ToLower() ||
                masterTypeDTO.masterType.ToLower() == "devices".ToLower()
                )
                return await GetMasterAsync(masterTypeDTO);
            else
                return BadRequest();
        }

        [NonAction]
        private async Task<ActionResult<IList<MasterDTO>?>> GetMasterAsync(MasterTypeDTO masterTypeDTO)
        {
            var masters = await _masterService.GetAllAsync(masterTypeDTO.masterType, masterTypeDTO.allRecords, masterTypeDTO.customerId);
            APIResponse objResponse = new APIResponse();
            objResponse.StatusCode = (int)APIResponseCode.Success;
            objResponse.Data = masters;
            return Ok(objResponse);
        }

        // GET api/<MasterController>/UploadMedia

        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [AuthorizeOIDC]
        [HttpPost("UploadFile")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Master", "{UserName} Upload File")]
        [HttpPost("upload")]
       
        public async Task<ActionResult<string>> UploadFile(IFormFileCollection files)
        {
            var filePaths = "";
            List<string> results = new List<string>();
            var path = "";
            if (files != null)
                foreach (var fileName in files)
                {
                    var filesPath = Directory.GetCurrentDirectory() + "/Uploads";
                    var browsePath = "http://" + HttpContext.Request.Host + "/Uploads/";
                    if (!System.IO.Directory.Exists(filesPath))//create path 
                        Directory.CreateDirectory(filesPath);
                    String datetick = DateTime.Now.Ticks.ToString();
                    path = Path.Combine(filesPath, datetick + "_" + Path.GetFileName(fileName.FileName));
                    await fileName.CopyToAsync(new FileStream(path, FileMode.Create));
                    results.Add(Path.Combine(browsePath, datetick + "_" + Path.GetFileName(fileName.FileName)));
                }

            return Ok(results.ToList());
        }

        [HttpPost]
        [Route("RequestSupport")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Master", "{UserName} Request Support")]
        public async Task<IActionResult> RequestSupport(CustomerIssuesDTO customerIssues)
        {
            CustomerIssues customerIssue = _mapper.Map<CustomerIssues>(customerIssues);
           // customerIssue.CommunityName = _common.CurrentUser().PrimaryCommunity.CommunityName;

            APIResponse apiResponse = new APIResponse();
            if (customerIssue != null && (customerIssue.CustomerId > 0 || !string.IsNullOrEmpty(customerIssue.Mobile))
                && !string.IsNullOrEmpty(customerIssue.IssueDescription))
            {
                var response = await _masterService.RequestSupport(customerIssue);
                if (response > 0)
                    apiResponse.StatusCode = (int)APIResponseCode.Success;
                else
                    apiResponse.StatusCode = (int)APIResponseCode.Failure;
            }
            else
                apiResponse.StatusCode = (int)APIResponseCode.Incomplete_Details;
            apiResponse.Message = "";
            return Ok(apiResponse);

        }

        

        [HttpPost]
        [AuthorizeOIDC]
        [Route("NotificationTopics")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Master", "{UserName} Requested Notification Topics")]
        public async Task<IActionResult> NotificationTopics()
        {
            List<string> tags = new List<string>();
            Customers currentcustomer = _common.CurrentUser();
            string environment = currentcustomer.Environment;
            tags.Add(environment + "_" + "Circular_all");
            tags.Add(environment + "_" + "Circular_user_" + currentcustomer.Id.ToString());

            //Community Tags
            string communityTag = environment + "_" + "Circular_community_";
            List<CustomerCommunity> communities = currentcustomer.CustomerCommunities;
            foreach (CustomerCommunity item in communities)
            {
                string tag = communityTag + item.CommunityId.ToString();
                tags.Add(tag);
            }

            //Group tags - pending
            string groupTag = environment + "_" + "Circular_communityGroups_";
            List<CustomerGroups> groups = currentcustomer.CustomerGroups;
            foreach (CustomerGroups item in groups)
            {
                string tag = groupTag + item.GroupId.ToString();
                tags.Add(tag);
            }


            var linkedMembers = await _customerService.GetLinkedMembers(_common.UserId);

            groupTag = environment + "_" + "Circular_linkedUser_";
            foreach (var item in linkedMembers)
            {
                IDictionary<string, Object> data = item;
                if (data != null)
                {
                    if (data["LinkedMemberId"] != null && data["LinkRequestByLoggedInUser"] != null)
                    {
                        if (data["LinkRequestByLoggedInUser"].ToString().Equals("1"))
                        {
                            string tag = groupTag + data["LinkedMemberId"].ToString();
                            tags.Add(tag);
                        }
                    }
                }

            }

            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = tags;
            return Ok(apiResponse);
        }


        [HttpPost]
        [AuthorizeOIDC]
        [Route("QRScan")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Master", "{UserName} Requested QR Scan")]
        public async Task<IActionResult> QRScan(QRScanRequest scanRequest)
        {

            APIResponse apiResponse = new APIResponse();
            var result = await _masterService.QRScan(scanRequest);
            if (result)
                apiResponse.StatusCode = (int)APIResponseCode.Success;
            else
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            apiResponse.Message = "";
            return Ok(apiResponse);

        }


        [HttpPost]
       [AuthorizeOIDC]
        [Route("Get/CommunityHouse")]
        [ActionLog("Master", "{UserName} Requested Master CommunityHouse")]

        public async Task<IActionResult> GetCommunityHouses([FromBody] MasterTypeHouseClassesDTO masterHouseClassesDTO)
        {
            var masters = await _masterService.GetCommunityHouseAllAsync(masterHouseClassesDTO.masterType, masterHouseClassesDTO.CommunityId);
            APIResponse objResponse = new APIResponse();
            objResponse.StatusCode = (int)APIResponseCode.Success;
            objResponse.Data = masters;
            return Ok(objResponse);

        }


        [HttpPost]
       [AuthorizeOIDC]
        [Route("Get/CommunityClasses")]
        [ActionLog("Master", "{UserName} Requested Master CommunityClasses")]

        public async Task<IActionResult> GetCommunityClasses([FromBody] MasterTypeHouseClassesDTO masterTypeHouseClasses)
        {
            var masters = await _masterService.GetCommunityClassesAllAsync(masterTypeHouseClasses.masterType, masterTypeHouseClasses.CommunityId);
            APIResponse objResponse = new APIResponse();
            objResponse.StatusCode = (int)APIResponseCode.Success;
            objResponse.Data = masters;
            return Ok(objResponse);

        }
    }
}
