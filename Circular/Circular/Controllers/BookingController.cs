using AutoMapper;
using Circular.Core.Entity;
using Circular.Core.DTOs;
using Circular.Filters;
using Circular.Services.Planners;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
namespace Circular.Controllers
{
    [ApiController]
    [Route("api/planner/[controller]")]
    [Tags("Planner - Contains API for Planner, Schedule, Ticket and Booking")]
    public class BookingController : ControllerBase
    {
        private readonly IPlannerService _PlannerService;
        private readonly IMapper _mapper;

        public BookingController(IMapper mapper, IPlannerService PlannerService)
        {
            _mapper = mapper;
            _PlannerService = PlannerService;
        }


        [HttpPost]
        [AuthorizeOIDC]
        [Route("Book")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Booking", "{UserName}  Add Booking")]
        public async Task<IActionResult> AddBooking(CustomerBookingDTO customerBookingDTO)
        {
            customerBookingDTO.IsBookingStatus = false;
            CustomerBooking booking = _mapper.Map<CustomerBooking>(customerBookingDTO);
            var Booking = await _PlannerService.CustomerBooking(booking);
            APIResponse clsResponse = new APIResponse();
            clsResponse.StatusCode = (int)APIResponseCode.Success;
            clsResponse.Data = booking;
            return Ok(clsResponse);
        }
        [HttpPost]
        [AuthorizeOIDC]
        [Route("Cancel")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Booking", "{UserName}  Cancel Booking")]
        public async Task<IActionResult> CancelBooking(CustomerBookingDTO customerBookingDTO)
        {
            customerBookingDTO.IsBookingStatus = false;
            CustomerBooking booking = _mapper.Map<CustomerBooking>(customerBookingDTO);
            var Booking = await _PlannerService.CancelCustomerBooking(booking);
            APIResponse clsResponse = new APIResponse();
            clsResponse.StatusCode = (int)APIResponseCode.Success;
            return Ok(clsResponse);
        }
        [HttpPost]
        [AuthorizeOIDC]
        [Route("List")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Booking", "{UserName} requested booking list")]
        public async Task<IActionResult> BookingList(BookingListDTO bookingListRequest)
        {
            var booking = await _PlannerService.GetBookingDetails(bookingListRequest.CommunityId,
                bookingListRequest.Date, bookingListRequest.CustomerId, bookingListRequest.BookingId, false);
            APIResponse clsResponse = new APIResponse();
            clsResponse.StatusCode = (int)APIResponseCode.Success;
            clsResponse.Data = booking;
            return Ok(clsResponse);
        }


        [HttpPost]
        [AuthorizeOIDC]
        [Route("MyList")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Booking", "{UserName} requested my Booking list")]
        public async Task<IActionResult> MyBooking(BookingListDTO bookingListRequest)
        {
           
            var booking = await _PlannerService.GetBookingDetails(bookingListRequest.CommunityId,
            bookingListRequest.Date, bookingListRequest.CustomerId, bookingListRequest.BookingId, true);
            APIResponse clsResponse = new APIResponse();
            clsResponse.StatusCode = (int)APIResponseCode.Success;
            clsResponse.Data = booking;
            return Ok(clsResponse);
        }

        [HttpGet]
        [AuthorizeOIDC]
        [Route("Members")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Booking", "{UserName} requested Booking Member Details")]
        public async Task<IActionResult> Members(long BookingId)
        {
            var booking = await _PlannerService.GetBookingMemberDetails(BookingId);
            APIResponse clsResponse = new APIResponse();
            clsResponse.StatusCode = (int)APIResponseCode.Success;
            clsResponse.Data = booking;
            return Ok(clsResponse);
        }
    }
}

