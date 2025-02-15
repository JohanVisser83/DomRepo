using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Filters;
using Circular.Framework.Logger;
using Circular.Services.Community;
using Circular.Services.Message;
using Circular.Services.User;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.IdentityModel.Tokens;

namespace Circular.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunityController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICommunityService _communityService;
        private readonly ICustomerService _customerService;
        private readonly IMessageService _messsageService;
        private readonly ILoggerManager _logger;
        private readonly ICommon _common;

        public CommunityController(IMapper mapper, ICommunityService communityService, IMessageService messageService, ILoggerManager logger, ICustomerService customerService, ICommon common)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _communityService = communityService ?? throw new ArgumentNullException(nameof(communityService));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _messsageService = messageService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _common = common;
        }

        #region "Community"

        [AuthorizeOIDC]
        [HttpPost]
        [Route("search")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Community", "{UserName}  Search Community")]
        public async Task<IActionResult> SearchCommunity([FromBody] SearchCommunityDTO searchCommunityDTO)
        {
            try
            {
                var communities = _communityService.SearchCommunity(searchCommunityDTO.communityName, searchCommunityDTO.communityId,searchCommunityDTO.pagesize,searchCommunityDTO.pagenumber,searchCommunityDTO.search, _common.CurrentUser().Id);
                var _comm = communities?.Select(u => _mapper.Map<CustomerCommunityDTO>(u)).ToList();

                APIResponse response = new APIResponse
                {
                    StatusCode = (int)APIResponseCode.Success,
                    Data = _comm
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return null;
            }
        

        }

        [AuthorizeOIDC]
        [HttpPost]
        [Route("member/search")]
		[ActionLog("Community", "{UserName}  Search Member")]
		public async Task<IActionResult> SearchUser([FromBody] SearchUserDTO searchUserDTO)
        {
            var users = await _messsageService.GetUserContactListAsync(searchUserDTO.communityId, searchUserDTO.searchText ?? "");

            APIResponse response = new APIResponse
            {
                StatusCode = (int)APIResponseCode.Success,
                Data = users
            };
            return Ok(response);


        }


        [AuthorizeOIDC]
        [HttpPost]
        [Route("Staff")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Community", "{UserName} Add Staff")]
		public async Task<IActionResult> Staff(CommunityIdDTO communityIdDTO)
        {
            var communityStaff = await _communityService.Staff(communityIdDTO.communityId);
            APIResponse apiResponse = new APIResponse();
            if (communityStaff != null)
            {
                apiResponse.StatusCode = (int)APIResponseCode.Success;
                apiResponse.Data = communityStaff;
            }
            else
                apiResponse.StatusCode = (int)APIResponseCode.Success;
            return Ok(apiResponse);
        }

        [AuthorizeOIDC]
        [HttpPost]
        [Route("TeamProfile")]
        [ActionLog("Community", "{UserName} Add Team Profile")]
        public async Task<IActionResult> TeamProfile(CommunityIdDTO communityIdDTO)
        {
            var communityTeamProfile = await _communityService.CommunityTeamProfile(communityIdDTO.communityId);
            APIResponse apiResponse = new APIResponse();
            if (communityTeamProfile != null)
            {
                apiResponse.StatusCode= (int)APIResponseCode.Success;
                apiResponse.Data = communityTeamProfile;
            }
            else
                apiResponse.StatusCode = (int) APIResponseCode.Success;
            return Ok(apiResponse);
        }


            [AuthorizeOIDC]
            [HttpPost]
            [Route("JoinCommunity")]
            [ActionLog("Community", "{User} Join Community")]

            public async Task<IActionResult> JoinCommunity(JoinCommunityDTO customerCommunityDTO)
            {
            try
            {
                long result = 0;
                if (customerCommunityDTO.AccessType.ToLower() != "Request access".ToLower())
                {
                     CustomerCommunity customerCommunity = _mapper.Map<CustomerCommunity>(customerCommunityDTO);
                    result = await _communityService.SaveNewCustomerCommunity(customerCommunity);
                }
                else
                {
                    CommunityAccessRequests communityAccessRequests = _mapper.Map<CommunityAccessRequests>(customerCommunityDTO);
                    result = await _communityService.SaveCommunityAccessRequest(communityAccessRequests);
                }
               
                if (result == -1)
                {
                    APIResponse objRes = new APIResponse();
                    objRes.StatusCode = (int)APIResponseCode.Record_Already_Exists;
                    objRes.Message = "You are either an existing member or your earlier request is pending.";
                    objRes.Data = result;
                    return Ok(objRes);
                }
                if (result == 0)
                    return BadRequest("Error while joining the community.");
                APIResponse objResponse = new APIResponse();
                objResponse.StatusCode = (int)APIResponseCode.Success;
                objResponse.Data = result;
                return Ok(objResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in JoinCommunity method-" + ex.Message);
                return BadRequest(ex.Message);
            }
            }

        [AuthorizeOIDC]
        [HttpPost]
        [Route("CancelCommunitySubscriptions")]
        [ActionLog("Community", "{User} Join Community")]

        public async Task<IActionResult> CancelCommunitySubscriptions()
        {
           var  result = await _communityService.CancelCommunitySubscriptions();
            APIResponse objResponse = new APIResponse();
            objResponse.StatusCode = (int)APIResponseCode.Success;
            objResponse.Data = result;
            return Ok(objResponse);
        }



        #endregion

        #region "Customer Community"

        [AuthorizeOIDC]
        [HttpPost]
        [Route("CustomerCommunities")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Community", "{UserName}  Search Member")]
		public async Task<ActionResult<IList<CustomerCommunityDTO>?>> CustomerCommunities(CustomerIdRequestDTO customerIdRequestDTO)
        {
            try
            {
                var communities = await _communityService.CommunityListByCustomerId(customerIdRequestDTO.CustomerId);
                communities?.Select(u => _mapper.Map<CustomerCommunityDTO>(u)).ToList();

                APIResponse response = new APIResponse();
                response.StatusCode = (int)APIResponseCode.Success;
                response.Data = communities;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in CommunityDetailByCustomerId method-" + ex.Message);
                return BadRequest(ex.Message);
            }

        }

        //[AuthorizeOIDC]
        //[HttpPost]
        //[Route("Community/Link")]
        //[SwaggerOperation(Summary = "Reviewed")]
        //public async Task<ActionResult<APIResponse>> saveCustomerCommunity([FromBody] CustomerCommunityRequestDTO customerCommunityDTO)
        //{
        //    try
        //    {
        //        CustomerCommunity customerCommunity = _mapper.Map<CustomerCommunity>(customerCommunityDTO);
        //        int var = await _communityService.SaveCustomerCommunity(customerCommunity);
        //        if (var == 0)
        //            return BadRequest("Error while saving communities");

        //        Customers customer = _customerService.getcustomerbyId(customerCommunity.CustomerId, true);
        //        CustomersDTO customerDTO = _mapper.Map<CustomersDTO>(customer);

        //        APIResponse objResponse = new APIResponse();
        //        objResponse.StatusCode = (int)APIResponseCode.Success;
        //        objResponse.Data = customerDTO;
        //        return Ok(objResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Error in saveCustomerCommunity method-" + ex.Message);
        //        return BadRequest(ex.Message);
        //    } 
        //}



        // [AuthorizeOIDC]

        [AuthorizeOIDC]
        [HttpPost]
        [Route("Community/Link")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Community", "{UserName}  Requested Save Customer")]
		public async Task<ActionResult<APIResponse>> saveCustomerCommunity([FromBody] CustomerCommunityRequestDTO customerCommunityDTO)
        {
            try
            {
                
                CustomerCommunity customerCommunity = _mapper.Map<CustomerCommunity>(customerCommunityDTO);
                int var = await _communityService.SaveCustomerCommunity(customerCommunity);

                if (var < 0)
                    return BadRequest("Oops! You can't switch to community having different currency.");

                if (var == 0)

                   return BadRequest("Error while saving communities");
                var Toemail = await _communityService.SendEmailSigupUser((long)customerCommunityDTO.CustomerId);

                Customers customer = _customerService.getcustomerbyId(customerCommunity.CustomerId, true);
                CustomersDTO customerDTO = _mapper.Map<CustomersDTO>(customer);

                APIResponse objResponse = new APIResponse();
                objResponse.StatusCode = (int)APIResponseCode.Success;
                objResponse.Data = customerDTO;
                return Ok(objResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in saveCustomerCommunity method-" + ex.Message);
                return BadRequest(ex.Message);
            }

        }
        






        [AuthorizeOIDC]
        [HttpPost]
        [Route("Community/Delink")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Community", "{UserName} Delete Customer")]
		public async Task<ActionResult<APIResponse>?> DeleteCustomerCommunity(CommunityIdDTO community)
        {
            int status = await _communityService.DeleteCustomerCommunity(community.communityId, _common.UserId);
            APIResponse apiResponse = new APIResponse();
            if (status <= 0)
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            else
            {
                apiResponse.StatusCode = (int)APIResponseCode.Success;
                apiResponse.Message = "Community has been dissociated from your account.";
            }
            return Ok(apiResponse);

        }

        #endregion

        #region "Dashboard"
        [AuthorizeOIDC]
        [HttpPost]
        [Route("Features")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Community", "{UserName}  Requested Dashboard Feature")]
		public async Task<ActionResult<IList<FeaturesDTO>?>> Features(CommunityIdDTO dashboardDTO)
        {
            var dashboardicons = await _communityService.Features_App(dashboardDTO.communityId, dashboardDTO.loggedInUserId);
            dashboardicons?.Select(u => _mapper.Map<FeaturesDTO>(u)).ToList();

            APIResponse response = new APIResponse();
            response.StatusCode = (int)APIResponseCode.Success;
            response.Data = dashboardicons;
            return Ok(response);

        }

        #endregion

        #region "Business"

        [HttpPost]
        [AuthorizeOIDC]
        [Route("Business/Add")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Community", "{UserName} Add Business")]
		public async Task<ActionResult<APIResponse>> AddBusinessIndex([FromBody] CustomerBusinessIndexDTO customerBusinessIndexDTO)
        {
            CustomerBusinessIndex customerBusinessIndex = _mapper.Map<CustomerBusinessIndex>(customerBusinessIndexDTO);
            var response = await _communityService.AddBusinessIndex(customerBusinessIndex);
            APIResponse apiResponse = new APIResponse();
            apiResponse.Message = "";
            if (response > 0)
                apiResponse.StatusCode = (int)APIResponseCode.Success;
            else
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            return Ok(apiResponse);
        }

        [HttpGet]
        [AuthorizeOIDC]
        [Route("Business/List")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Community", "{UserName}  Requested Business")]
		public async Task<ActionResult<APIResponse>> GetBusinessIndex(long CommunityId, long? id, long? UserId, int BusinessCategoryId,
            string? searchText, int pageNumber, int pageSize
            )
        {
            List<CustomerBusinessIndex> businessIndex = _communityService.GetBusinessIndex(CommunityId, id, UserId, BusinessCategoryId,
             searchText, pageNumber, pageSize,false
                ).Result.ToList();
            APIResponse clsRespone = new APIResponse();
            clsRespone.StatusCode = (int)APIResponseCode.Success;
            clsRespone.Data = businessIndex;
            return Ok(clsRespone);
        }


        [HttpPost]
        [AuthorizeOIDC]
        [Route("Business/Remove")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Community", "{UserName} Delete Business")]
		public async Task<ActionResult> DeleteBusiness(CustomerBusinessIndexDTO businessDTO)
        {
            CustomerBusinessIndex business = _mapper.Map<CustomerBusinessIndex>(businessDTO);
            var response = await _communityService.DeleteBusiness(business);
            APIResponse apiResponse = new APIResponse();
            if (response > 0)
            {
                apiResponse.StatusCode = (int)APIResponseCode.Success;
                apiResponse.Data = response;
            }
            else
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            return Ok(apiResponse);

        }

        [HttpPost]
        [AuthorizeOIDC]
        [Route("Business/Edit")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Community", "{UserName} Edit Business")]
		public async Task<ActionResult> EditBusiness(CustomerBusinessIndexDTO businessDTO)
        {
            CustomerBusinessIndex business = _mapper.Map<CustomerBusinessIndex>(businessDTO);
            var response = await _communityService.EditBusiness(business);
            APIResponse apiResponse = new APIResponse();
            if (response > 0)
            {
                apiResponse.StatusCode = (int)APIResponseCode.Success;
                apiResponse.Data = response;
            }
            else
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            return Ok(apiResponse);
        }
        #endregion

        #region "Job"

        [HttpPost]
        [AuthorizeOIDC]
        [Route("Job/Add")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Community", "{UserName} Add Job Posting")]
		public async Task<ActionResult> AddJobPosting(JobsDTO jobsDTO)
        {
            Jobs job = _mapper.Map<Jobs>(jobsDTO);
            job.IsApproved = false;
            var response = await _communityService.AddJobPosting(job);
            APIResponse apiResponse = new APIResponse();
            apiResponse.Message = "";
            if (response > 0)
            {
                apiResponse.StatusCode = (int)APIResponseCode.Success;
                apiResponse.Data = response;
            }
            else
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            return Ok(apiResponse);
        }

        [HttpPost]
        [AuthorizeOIDC]
        [Route("Job/Edit")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Community", "{UserName} Edit Job Posting")]

		public async Task<ActionResult> EditJobPosting(JobsDTO jobsDTO)
        {
            Jobs job = _mapper.Map<Jobs>(jobsDTO);
            var response = await _communityService.EditJobPosting(job);
            APIResponse apiResponse = new APIResponse();
            if (response > 0)
            {
                apiResponse.StatusCode = (int)APIResponseCode.Success;
                apiResponse.Data = response;
            }
            else
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            return Ok(apiResponse);
        }

        [HttpPost]
        [AuthorizeOIDC]
        [Route("Job/Remove")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Community", "{UserName} Delete Job Posting")]

		public async Task<ActionResult> DeleteJobPosting(JobsDTO jobsDTO)
        {
            Jobs job = _mapper.Map<Jobs>(jobsDTO);
            var response = await _communityService.DeleteJobPosting(job);
            APIResponse apiResponse = new APIResponse();
            if (response > 0)
            {
                apiResponse.StatusCode = (int)APIResponseCode.Success;
                apiResponse.Data = response;
            }
            else
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            return Ok(apiResponse);
        }

        [HttpPost]
        [AuthorizeOIDC]
        [Route("Job/Views")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Community", "{UserName}  Job View Count")]

		public async Task<ActionResult> UpdateViewCount(JobsDTO jobsDTO)
        {
            Jobs job = _mapper.Map<Jobs>(jobsDTO);
            var response = await _communityService.UpdateViewCount(job);
            APIResponse apiResponse = new APIResponse();
            if (response > 0)
                apiResponse.StatusCode = (int)APIResponseCode.Success;
            else
            {
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
                apiResponse.Data = response;
            }
            return Ok(apiResponse);
        }

        [HttpPost]
        [AuthorizeOIDC]
        [Route("Job/Apply")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Community", "{UserName} Apply Job")]

		public async Task<ActionResult> ApplyJob(JobApplicationDTO jobApplicationDTO)
        {
            JobApplication jobApplication = _mapper.Map<JobApplication>(jobApplicationDTO);
            var response = await _communityService.ApplyJob(jobApplication);
            APIResponse apiResponse = new APIResponse();
            if (response > 0)
            {
                apiResponse.StatusCode = (int)APIResponseCode.Success;
                apiResponse.Data = response;
            }
            else
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            return Ok(apiResponse);
        }

        [HttpGet]
        [AuthorizeOIDC]
        [Route("Job/List")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Community", "{UserName} Requested Job List")]
		public async Task<ActionResult<APIResponse>> List(long BusinessId, long id, long customerId, int jobCategoryId,
            string? searchText, int pageNumber, int pageSize)
        {
            if (searchText.IsNullOrEmpty())
                searchText = "";
            long communityId = _common.CurrentUser().PrimaryCommunity.CommunityId ?? 0;
            List<Jobs> getJobList = _communityService.GetJobPosting(BusinessId, id, customerId, jobCategoryId, searchText, communityId, pageNumber, pageSize).Result.ToList();
            APIResponse clsResponse = new APIResponse();
            clsResponse.StatusCode = (int)APIResponseCode.Success;
            clsResponse.Data = getJobList;
            return Ok(clsResponse);
        }

        #endregion

        #region Network

        [HttpGet]
        [AuthorizeOIDC]
        [Route("Network")]
        [SwaggerOperation(Summary = "Reviewed - To get all members of community, friends, blocked list, sent requests, recieved requests.")]
		[ActionLog("Community", "{UserName} Requested Community Network")]
		public async Task<ActionResult> GetCommunityNetwork(long CommunityId, long? LoggedInUserId, string? SearchText, int IsFriend, int pageNumber, int pageSize)
        {
            var response = _communityService.GetCommunityNetwork(CommunityId, LoggedInUserId, SearchText, IsFriend, pageNumber, pageSize);
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = response;

            return Ok(apiResponse);
        }



        [HttpPost]
        [AuthorizeOIDC]
        [Route("Network/Action")]
        [SwaggerOperation(Summary = " Reviewed- To send(102), accept(101), reject(103), block(104), unblock(107) from network.")]
		[ActionLog("Community", "{UserName} Action On Network")]


		public async Task<ActionResult> ActionOnNetwork(NetworkActionDTO action)
        {

            var response = _communityService.ActionOnNetwork(action.CustomerId, action.LoggedInUserId, action.FriendRequestStatusId, _common.CurrentUser());
            APIResponse apiResponse = new APIResponse();
            if (response > 0)
            {
                apiResponse.StatusCode = (int)APIResponseCode.Success;
                apiResponse.Data = response;
            }
            else
            {
                apiResponse.StatusCode = (int)APIResponseCode.Invalid_Request;

            }
            return Ok(apiResponse);
        }


        [HttpGet]
        [AuthorizeOIDC]
        [Route("Fundraiser")]
		[ActionLog("Community", "{UserName} Requested Fundraiser")]


		public async Task<ActionResult<APIResponse>> GetFundraiser(long CommunityId, long? FundraiserId)
        {
            var fund = _communityService.GetFundraiserAsync(CommunityId, FundraiserId);
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = fund;
            return Ok(apiResponse);

        }


        [HttpPost]
        [AuthorizeOIDC]
        [Route("Fundraiser/Pay")]
		[ActionLog("Community", "{UserName} Pay Fundraiser")]
		public async Task<ActionResult<APIResponse>> PayFundraiser(PayFundraiserDTO payFundraiser)
        {
            var payFundhub = _communityService.PayFundraiser(payFundraiser.FundraiserTypeId, payFundraiser.PayForUserId, payFundraiser.Amount, payFundraiser.LoggedInUserId, payFundraiser.Currency);
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = payFundhub;
            return Ok(apiResponse);


        }

        [HttpPost]
        [AuthorizeOIDC]
        [Route("Fundraiser/ActiveOrders")]
		[ActionLog("Community", "{UserName} Requested Actove Orders")]

		public async Task<ActionResult<APIResponse>> ActiveOrders(FundhubActiveOrderDTO fundhubActiveOrderDTO)
        {
            try
            {
                var activeOrders = _communityService.GetFundhubActiveOrders(fundhubActiveOrderDTO.loggedinuser ?? 0, fundhubActiveOrderDTO.CommunityId ?? 0, fundhubActiveOrderDTO.IsCollected);
                APIResponse apiResponse = new APIResponse();
                apiResponse.StatusCode = (int)APIResponseCode.Success;
                apiResponse.Data = activeOrders;
                return Ok(apiResponse);
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
        }

    }


    #endregion


    

}

