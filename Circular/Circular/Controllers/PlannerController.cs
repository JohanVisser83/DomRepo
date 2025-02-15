using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Filters;
using Circular.Services.Planners;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Circular.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Planner - Contains API for Planner, Schedule, Ticket and Booking")]
    public class PlannerController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IPlannerService _PlannerService;
        private readonly ICommon _common;
       
        public PlannerController(IMapper mapper, IPlannerService PlannerService, ICommon common )
        {
            _mapper = mapper;
            _PlannerService = PlannerService;
            _common = common;
        }


        #region "Planner"
        [HttpPost]
        [AuthorizeOIDC]
        [Route("Items")]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Planner", "{UserName} Requested Planner Items")]
		public async Task<IActionResult> PlannerItems(PlannerRequestDTO plannerRequestDTO)
        {
            Customers currentCustomer = _common.CurrentUser();
            var plannerItems = await _PlannerService.GetPlannerItemsAsync(plannerRequestDTO.PlannerType, plannerRequestDTO.PlannerId,currentCustomer.PrimaryCommunity.CommunityId??0);
            var paidDocuments = await _PlannerService.GetPaidDocument(currentCustomer.PrimaryCommunity.CustomerId);
            if (paidDocuments is not null && plannerItems is not null)
            {
                foreach (var document in paidDocuments)
                {
                    List<Planner> planners = plannerItems.Where(p => p.Id == document.DocumentId).ToList();
                    if(planners.Any())
                    {
                        planners.FirstOrDefault<Planner>().Isbought = true;
                    }
                }
            }
            
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = plannerItems;
            return Ok(apiResponse);

        }
        [HttpPost]
        [Route("Item/Email")]
        [AuthorizeOIDC]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Planner", "{UserName}  Planner Email")]
		public async Task<ActionResult> PlannerEmail(EmailPlannerDTO emailPlannerDTO)
        {
            try
            {
                Customers currentCustomer = _common.CurrentUser();
                APIResponse apiResponse = new APIResponse();          
                if (await _PlannerService.EmailPlanner(emailPlannerDTO, currentCustomer.PrimaryCommunity.CommunityId ?? 0))
                    apiResponse.StatusCode = (int)APIResponseCode.Success;
                else
                    apiResponse.StatusCode = (int)APIResponseCode.Failure;
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        [HttpPost]
        [AuthorizeOIDC]
        [Route("Pay")]
        [ActionLog("Planner", "{UserName}  Paid Document")]
        public async Task<IActionResult> AddPaidDocument(PaidDocumentDTO paidDocumentDTO)
        {
            PaidDocument add = _mapper.Map<PaidDocument>(paidDocumentDTO);
            var Add = await _PlannerService.AddPaidDocument(add);
            APIResponse clsResponse = new APIResponse();
            clsResponse.StatusCode = (int)APIResponseCode.Success;

            return Ok(clsResponse);
        }


        [HttpPost]
        [AuthorizeOIDC]
        [Route("")]
        [ActionLog("Planner", "{UserName}   ")]
        public async Task<IActionResult> AddPlanner(PlannerAddDTO plannerDTO)
        {
            Planner add = _mapper.Map<Planner>(plannerDTO);
            var Add = await _PlannerService.NewPlannerAsync(add);
            APIResponse clsResponse = new APIResponse();
            clsResponse.StatusCode = (int)APIResponseCode.Success;

            return Ok(clsResponse);
        }

        #endregion

        #region "Event"


        [HttpPost]
        [Route("Schedule/Events")]
        [AuthorizeOIDC]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Planner", "{UserName}  Requested Events")]
		public async Task<IActionResult> Events(EventsRequestDTo eventRequestDTO)
        {
            APIResponse apiResponse = new APIResponse();
            var response = await _PlannerService.Events(eventRequestDTO.EventId, eventRequestDTO.CommunityId, eventRequestDTO.CustomerId, eventRequestDTO.IsAllUpcomingOrCompleted);
            apiResponse.Data = response;
            if (response != null)
                apiResponse.StatusCode = (int)APIResponseCode.Success;
            else
                apiResponse.StatusCode = (int)APIResponseCode.Failure;

            return Ok(apiResponse);

        }

        [HttpPost]
        [Route("Schedule/Event/Register")]
        [AuthorizeOIDC]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Planner", "{UserName}  Event Registered")]
		public async Task<IActionResult> Register(EventsRegistrationRequest eventRegRequest)
        {
            try
            {
                
                APIResponse apiResponse = new APIResponse();
                /*  string currency =  "$"; */// _common.CurrentUser().PrimaryCommunity.currencyCode ?? "";
                string currency = _common.CurrentUser().PrimaryCommunity.currencyCode ?? "";
                var response = await _PlannerService.RegisterForEvents(eventRegRequest.EventId,
                currency, eventRegRequest.LoggedInCustomerId,
                eventRegRequest.RegistrationForCustomerId, eventRegRequest.Amount);
                
                apiResponse.Data = response;
                if (response != null)
                {
                    var Emails = await _PlannerService.SendEmailPlanner((long)eventRegRequest.LoggedInCustomerId);
                    apiResponse.StatusCode = (int)APIResponseCode.Success;
                }
                else
                    apiResponse.StatusCode = (int)APIResponseCode.Failure;
                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        [Route("Schedule/Event/Deregister")]
        [AuthorizeOIDC]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Planner", "{UserName}  Event Deregister")]
		public async Task<IActionResult> Deregister(EventsDeRegistrationRequest eventDeRegRequest)
        {
            APIResponse apiResponse = new APIResponse();
            string currency = _common.CurrentUser().PrimaryCommunity.currencyCode ?? "";
            var response = await _PlannerService.DeregisterForEvents(eventDeRegRequest.InviteId);
            apiResponse.Data = response;
            if (response != null)
                apiResponse.StatusCode = (int)APIResponseCode.Success;
            else
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            return Ok(apiResponse);
        }

        #endregion

    }





}
