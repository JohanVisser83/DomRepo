using AutoMapper;
using Circular.Filters;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Microsoft.AspNetCore.Mvc;
using Circular.Services.Safety;
using Circular.Services.Transport;
using Swashbuckle.AspNetCore.Annotations;
using Circular.Services.Audit;
using MailKit.Search;

namespace Circular.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TransportController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly ISafetyService _SafetyService;
		private readonly ITransportService _TransportService;
		public TransportController(IMapper mapper, ISafetyService safetyService, ITransportService TransportService)
		{
			_SafetyService = safetyService;
			_mapper = mapper;
			_TransportService = TransportService;
		}

		//this is ticket api
		[HttpPost]
		[AuthorizeOIDC]
		[Route("Tickets")]
		[SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Transport", "{UserName} Requested Ticket")]
		public async Task<IActionResult> GetTickets(TicketRequestDTO ticketRequestDTO)
		{
				var ticket = await _TransportService.GetTicketsAsync(ticketRequestDTO.CommunityId, ticketRequestDTO.CustomerId,
					ticketRequestDTO.Date, ticketRequestDTO.TicketId);
				APIResponse apiResponse = new APIResponse();
				if (ticket != null && ticket.Count() > 0)
					apiResponse.StatusCode = (int)APIResponseCode.Success;
				else
					apiResponse.StatusCode = (int)APIResponseCode.Failure;
				apiResponse.Data = ticket;
				return Ok(apiResponse);
			

		}

		[HttpPost]
		[Route("Ticket/Buy")]
		[AuthorizeOIDC]
		[SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Transport", "{UserName} Buy Ticket")]
		public async Task<IActionResult> Buy(TicketBuyRequest ticketRegistrationRequest)
		{
			APIResponse apiResponse = new APIResponse();
			var response = await _TransportService.BuyTicket(ticketRegistrationRequest.TicketDayId,
				 ticketRegistrationRequest.CustomerId);
			apiResponse.Data = response;
			if (response != null)
				apiResponse.StatusCode = (int)APIResponseCode.Success;
			else
				apiResponse.StatusCode = (int)APIResponseCode.Failure;
			return Ok(apiResponse);
		}

		[HttpPost]
		[Route("Flexipass")]
		[AuthorizeOIDC]
		[SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Transport", "{UserName} Buy Flexipass")]
		public async Task<IActionResult> Flexipass(FlexipassBuyRequest flexiRequest)
		{
			APIResponse apiResponse = new APIResponse();
			var response = await _TransportService.BuyFlexipass(flexiRequest);
			apiResponse.Data = response;
			if (response != null)
				apiResponse.StatusCode = (int)APIResponseCode.Success;
			else
				apiResponse.StatusCode = (int)APIResponseCode.Failure;
			return Ok(apiResponse);
		}


        [HttpGet]
        [AuthorizeOIDC]
        [Route("Vehicles")]
		[ActionLog("Transport", "{UserName} Requested Transport Vechile")]
		public async Task<IActionResult> GetTransportVechile(long CommunityId)
        {
            var response = await _TransportService.GetTransportVehciles(CommunityId);
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = response;

            return Ok(apiResponse);



        }
    }
}
