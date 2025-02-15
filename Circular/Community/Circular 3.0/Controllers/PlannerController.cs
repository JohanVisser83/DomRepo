using AutoMapper;
using Circular.Core.Entity;
using Circular.Core.DTOs;
using Circular.Framework.Utility;
using Circular.Services.Community;
using Circular.Services.Message;
using Circular.Services.Planners;
using CircularWeb.Business;
using CircularWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Circular.Data.Repositories.User;
using CircularWeb.filters;
using DocumentFormat.OpenXml.Drawing;

namespace CircularWeb.Controllers
{
    [Authorize]
    public class PlannerController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IHelper _helper;
        private readonly IGlobal _global;

        private readonly IMessageService _MessageService;
        private readonly IPlannerService _PlannerService;
        private readonly ICommunityService _CommunityService;

        private readonly IWebHostEnvironment _webHostEnvironment;
        public PlannerModel plannerModel = new PlannerModel();

        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CurrentUser currentUser;
        private string OIDCUrl;
        public PlannerController(IPlannerService PlannerService, IMapper mapper, IWebHostEnvironment webHostEnvironment, IConfiguration configuration
            , IHttpContextAccessor httpContextAccessor, IHelper helper, ICommunityService communityService, IMessageService messageService, ICustomerRepository customerRepository)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _PlannerService = PlannerService ?? throw new ArgumentNullException(nameof(PlannerService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _CommunityService = communityService ?? throw new ArgumentNullException(nameof(communityService));
            _MessageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            _global = new Global(_httpContextAccessor, _config, customerRepository);
            if (string.IsNullOrEmpty(configuration["OIDCUrl"]))
                throw new ArgumentNullException("configuration : OIDCUrl is not defined in App Setting");
            OIDCUrl = Convert.ToString(configuration["OIDCUrl"]);
        }


        [HttpGet]
        [Route("Planner/")]
        [ActionLog("Planner", "{0} opened Planner module.")]

        public async Task<IActionResult> Index()
        {
            CurrentUser currentUser = _global.GetCurrentUser();
            
            long Loggedinuser = currentUser.Id;
            plannerModel.PlannerTypes = await _PlannerService.GetMasterPlannerTypeAsync();
            plannerModel.Groups = await _CommunityService.GetCommunityGroups(currentUser.PrimaryCommunityId);
            plannerModel.PassedEvents = await _PlannerService.GetEventsAsync(currentUser.PrimaryCommunityId, 0);
            plannerModel.UpcomingEvents = await _PlannerService.GetEventsAsync(currentUser.PrimaryCommunityId, 1);
            plannerModel.Organizers = await _CommunityService.GetCommunityOrganizers(currentUser.PrimaryCommunityId);
            plannerModel.currencyModel.CurrencyCode = currentUser.Currency;
            plannerModel.CommunityLogo = currentUser.CustomerInfo.PrimaryCommunity.CommunityLogo;
            plannerModel.CommunityFeatures = _CommunityService.Features(currentUser.PrimaryCommunityId,Loggedinuser).Result.ToList();
            plannerModel.IsOwner = currentUser.CustomerInfo.PrimaryCommunity.OwnerCustomerId == Loggedinuser ? true : false;
            plannerModel.SubscriptionStatus = currentUser.SubscriptionStatus;
            if (!plannerModel.IsFeatureAvailable("P-001"))
                throw new ArgumentNullException("Unauthroized : You dont have permission to access this functionality.");
            else


                return View("Planner", plannerModel);
        }

        [ActionLog("Planner", "{0} fetched active bookings details.")]
        public async Task<ActionResult> GetBookingActiveItems()
        {
            var result = await _PlannerService.GetBookingActiveItemsAsync();
            if (result != null)
                return Json(new { success = true, message = "Data fetched successfully!", data = result });
            else
                return Json(new { success = false, message = "No Data Found!", data = "" });
        }
        #region "Planner" 
        [HttpPost]
        [ActionLog("Planner", "{0} saved new item .")]
        public async Task<ActionResult<int>> AddNewItem(PlannerDTO plannerDTO)
        {
            try
            {
                plannerDTO.CommunityId = _global.currentUser.PrimaryCommunityId;
                if (plannerDTO.Mediafile != null)
                    plannerDTO.Media = _helper.SaveFile(plannerDTO.Mediafile, _global.UploadFolderPath, this.Request);
                if (!string.IsNullOrEmpty(plannerDTO.HyperLink))
                {
                    if (!plannerDTO.HyperLink.ToLower().Contains("http"))
                    {
                        plannerDTO.HyperLink = "http://" + plannerDTO.HyperLink;
                    }
                }
                Planner planner = _mapper.Map<Planner>(plannerDTO);



                var result = await _PlannerService.NewPlannerAsync(planner);
                if (result > 0)
                    return Json(new { success = true, message = "Your item is now LIVE in your community" });
                else
                    return Json(new { success = false, message = "Data Not Saved Successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Oops! something went wrong" });
            }
        }

        [ActionLog("Planner", "{0} fetched active planner items.")]
        public async Task<IActionResult> ActivePlannerItems(string plannerTypeId)
        {
            long CommunityId = _global.currentUser.PrimaryCommunityId;
            var lstPlannerActive = await _PlannerService.GetPlannerItemsAsync(plannerTypeId, 0, CommunityId);
            return Json(new { success = true, Data = lstPlannerActive });
        }

        [HttpPost]
        [ActionLog("Planner", "{0} deleted active items.")]
        public async Task<IActionResult> DeleteActiveItems(long Id)
        {

            var result = await _PlannerService.DeletePlannerItemsAsync(Id);
            if (result > 0)
                return Json(new { success = true, message = "Item Deleted Successfully" });
            else
                return Json(new { success = false, message = "Item Not Deleted Successfully" });
        }

        #endregion

        #region "Event\Schedule"
        [HttpPost]
        [ActionLog("Planner", "{0} saved new schedule event.")]
        public async Task<ActionResult> AddNewEvent(EventDTO eventDTO)
        {
            try
            {

                eventDTO.CommunityId = _global.currentUser.PrimaryCommunityId;
                eventDTO.CommunityName = _global.currentUser.PrimaryCommunityName;
                eventDTO.CreatedBy = eventDTO.ModifiedBy = _global.currentUser.Id;
                if (eventDTO.Mediafile != null)
                    eventDTO.EventPdf = _helper.SaveFile(eventDTO.Mediafile, _global.UploadFolderPath, this.Request);
                if (eventDTO.MediaCoverfile != null)
                    eventDTO.CoverImage = _helper.SaveFile(eventDTO.MediaCoverfile, _global.UploadFolderPath, this.Request);
                Event item = _mapper.Map<Event>(eventDTO);

                DateTime dtSchedule = DateTime.Now;
                if (item.ScheduleDate != null)
                {

                    if (item.ScheduleTime != null)
                        dtSchedule = new DateTime(item.ScheduleDate.Value.Year, item.ScheduleDate.Value.Month, item.ScheduleDate.Value.Day, item.ScheduleTime.Value.Hours, item.ScheduleTime.Value.Minutes, 0);
                    else
                        dtSchedule = new DateTime(item.ScheduleDate.Value.Year, item.ScheduleDate.Value.Month, item.ScheduleDate.Value.Day);
                }
                if (item.ScheduleDate == null || item.ScheduleDate?.ToString("yyyy-MM-dd") == "0001-01-01")
                {
                    item.ScheduleDate = DateTime.Now.Date;
                    item.ScheduleTime = DateTime.Now.TimeOfDay;
                }
                if (item.ScheduleTime == null)
                {
                    item.ScheduleTime = DateTime.Now.TimeOfDay;
                }
                DateTime dtStart = new DateTime(item.EventStartDate.Year, item.EventStartDate.Month, item.EventStartDate.Day, item.StartTime.Hours, item.StartTime.Minutes, 0);
                if (dtSchedule > dtStart)
                {
                    return Json(new { success = false, message = "Schedule date & time can not be after Event start date and time!" });
                }

                var result = await _PlannerService.SaveAsync(item, dtSchedule);
                if (result > 0)
                    return Json(new { success = true, message = "Your Event is now LIVE in your community" });
                else
                    return Json(new { success = false, message = "Event not saved successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message + " Inner Expection " + ex.InnerException });
            }
        }



        [ActionLog("Planner", "{0} updated schedule event.")]
        public async Task<ActionResult> UpdateEvent(EventDTO eventDTO)
        {
            try
            {
                Event messagelist = _mapper.Map<Event>(eventDTO);
                var result = await _PlannerService.UpdateEvent(messagelist);
                // var result = 1;
                if (result > 0)
                    return Json(new { success = true, message = "" });
                else
                    return Json(new { success = false, message = "" });
            }
            catch (Exception ex)
            {
                // throw ex;
                return Json(new { success = false, message = "Oops. Something went wrong!" });
            }
        }

        [ActionLog("Planner", "{0} deleted active event items.")]
        public async Task<IActionResult> DeleteEventActiveItems(long Id)
        {
            var result = await _PlannerService.DeleteUpcomingItemsAsync(Id);
            if (result > 0)
                return Json(new { success = true, message = "Event Deleted Successfully" });
            else
                return Json(new { success = false, message = "Event Not Deleted Successfully" });
        }

        [ActionLog("Planner", "{0} fetched specific event details.")]
        public async Task<IActionResult> GetEventDetailById(long Id)
        {
            var result = await _PlannerService.GetActiveBookingById(Id);
            if (result != null && result.Count() > 0)
                return Json(new { success = true, message = "Data fetched successfully!", data = result });
            else
                return Json(new { success = false, message = "No Data Found!" });
        }

        [ActionLog("Planner", "{0} fetched review attendance.")]
        public async Task<ActionResult> ShowReviewAttendance(long EventId)
        {
            var result = await _PlannerService.GetEventAttendace(EventId);
            if (result != null && result.Count() > 0)
                return Json(new { success = true, message = "Data fetched successfully!", data = result });
            else
                return Json(new { success = false, message = "No Data Found!", data = "" });
        }

        [ActionLog("Planner", "{0} updated event checkin.")]
        public async Task<ActionResult> UpdateCheckin(long EnviteeId, bool Checkin)
        {
            var result = await _PlannerService.UpdateCheckin(EnviteeId, Checkin);
            //long communityId = _global.GetCurrentUser().PrimaryCommunityId;
            ///var result = await _PlannerService.Events(EventId, communityId, 0, 0);
            if (result == true)
                return Json(new { success = true, message = "Data update successfully!" });
            else
                return Json(new { success = false, message = "No Data Found!", data = "" });
        }

        #endregion

        #region "Booking"
        [ActionLog("Planner", "{0} saved booking.")]
        public async Task<IActionResult> AddBooking(BookingsDTO bookingsDTO)
        {
            bookingsDTO.CommunityId = _global.GetCurrentUser().PrimaryCommunityId;
            bookingsDTO.CommunityName = _global.currentUser.PrimaryCommunityName;
            if (bookingsDTO.EndDate == null || bookingsDTO.EndDate.ToString() == "01-01-0001 00:00:00")
            {
                bookingsDTO.EndDate = bookingsDTO.StartDate;
                if (string.IsNullOrWhiteSpace(bookingsDTO.Days))
                    bookingsDTO.Days = ((int)bookingsDTO.StartDate.DayOfWeek).ToString();
            }

            bookingsDTO.CreatedBy = bookingsDTO.ModifiedBy = _global.currentUser.Id;
            Bookings bookings = _mapper.Map<Bookings>(bookingsDTO);
            var result = await _PlannerService.SaveBookings(bookings);

            if (result > 0)
                return Json(new { success = true, message = "Booking added successfully." });
            else if (result == -1)
                return Json(new { success = false, message = "Selected day does not occur between selected dates. Please try again!" });
            else
                return Json(new { success = false, message = "Oops! Something went wrong. Please try again!" });
        }

        [ActionLog("Planner", "{0} fetched booking details.")]
        public async Task<ActionResult> GetBookingDetails(long CommunityBookingId, bool IsBookingStatus)
        {
            var result = await _PlannerService.GetBookingDetails(CommunityBookingId, IsBookingStatus);
            if (result != null && result.Count() > 0)
                return Json(new { success = true, message = "Data fetched successfully!", data = result });
            else
                return Json(new { success = false, message = "No Data Found!", data = "" });
        }

        [ActionLog("Planner", "{0} deleted active bookings items.")]
        public async Task<IActionResult> DeleteBookingActiveItems(long Id)
        {
            var result = await _PlannerService.DeleteBookingActiveItemsAsync(Id);
            if (result > 0)
                return Json(new { success = true, message = "Booking Deleted Successfully" });
            else
                return Json(new { success = false, message = "Item Not Deleted Successfully" });
        }

        //[ActionLog("Planner", "{0} fetch booking attendance.")]
        public async Task<ActionResult> BookingAttendance(DateTime Date, long Id)
        {
            var result = await _PlannerService.BookingAttendance(Date, Id);
            if (result != null)
                return Json(new { success = true, message = "Your item is now LIVE in your community", data = result });
            else
                return Json(new { success = false, message = "Oops! Something went wrong. Please try again!" });

        }

        [ActionLog("Planner", "{0} fetched selected booking days.")]
        public async Task<ActionResult> GetBookingDays(long MasterBookingId)
        {
            var result = await _PlannerService.GetBookingDays(MasterBookingId);
            if (result != null)
                return Json(new { success = true, message = "", data = result });
            else
                return Json(new { success = false, message = "" });

        }

        [ActionLog("Planner", "{0} fetched booking members details.")]
        public async Task<ActionResult> BookedMembers(long BookindDayId)
        {
            var result = await _PlannerService.BookedMembers(BookindDayId);
            if (result != null)
                return Json(new { success = true, message = "", data = result });
            else
                return Json(new { success = false, message = "" });

        }

        [ActionLog("Planner", "{0} fetched booking Spaces ")]
        public async Task<ActionResult> BookedSpaces(Bookings bookings)
        {
            long primayCommmunityId = _global.currentUser.PrimaryCommunityId;
            bookings.CommunityId = primayCommmunityId;
            IEnumerable<Bookings> result = await _PlannerService.BookedSpaces(bookings.CommunityId);
            if (result != null)
                return Json(new { success = true, message = "", data = result });
            else
                return Json(new { success = false, message = "" });

        }

        [ActionLog("Planner", "{0} fetched booking attendance list.")]
        public async Task<ActionResult> GetBookingAttendance()
        {
            long primayCommmunityId = _global.currentUser.PrimaryCommunityId;
            var result = await _PlannerService.GetBookingAttendance(primayCommmunityId);
            if (result.Count() > 0)
                return Json(new { success = true, message = "Your item is now LIVE in your community", data = result });
            else
                return Json(new { success = false, message = "Oops! Something went wrong. Please try again!" });
        }


        #endregion

        #region "Ticket"
        [ActionLog("Planner", "{0} fetced search contacts/name.")]
        public async Task<ActionResult> GetUserContactList(string Search)
        {
            long communityId = _global.GetCurrentUser().PrimaryCommunityId;
            var userContactList = await _MessageService.GetUserContactListAsync(communityId, Search);
            var userContact = (userContactList?.Select(u => _mapper.Map<UserContactList>(u)).ToList());
            return Json(userContact);
        }

        [ActionLog("Planner", "{0} fetched user management list.")]
        public async Task<ActionResult> UserManagement(long PermissionId)
        {
            try
            {
                long communityId = _global.GetCurrentUser().PrimaryCommunityId;
                var result = await _PlannerService.GetManageUser(PermissionId, communityId);
                if (result != null)
                    return Json(new { success = true, message = "Your item is now LIVE in your community", data = result });
                else
                    return Json(new { success = false, message = "Oops! Something went wrong. Please try again!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Oops! something went wrong" });
            }
        }

        [ActionLog("Planner", "{0} activated scanner checkin or not.")]
        public async Task<ActionResult> ActivateScanner(long Id, int PermissionId)
        {

            long communityId = _global.GetCurrentUser().PrimaryCommunityId;
            var result = await _PlannerService.ScannerProfile(Id, PermissionId, communityId);
            if (result != null)
                return Json(new { success = true, message = "Your item is now LIVE in your community" });
            else
                return Json(new { success = false, message = "Oops! Something went wrong. Please try again!" });

        }
        [ActionLog("Planner", "{0} deleted selected user.")]
        public async Task<IActionResult> DeleteUser(long Id)
        {
            var result = await _PlannerService.DeleteUserManagement(Id);
            if (result > 0)
                return Json(new { success = true, message = " Profile Deleted Successfully." });
            else
                return Json(new { success = false, message = "Item Not Deleted Successfully" });
        }

        [ActionLog("Planner", "{0}IsScanner activate.")]
        public async Task<IActionResult> UpdateUser(long Id, bool IsScannerActive)
        {
            var result = await _PlannerService.UpdateUserManagement(Id, Convert.ToBoolean(IsScannerActive));
            if (result > 0)
                return Json(new { success = true, message = "Item Update Successfully" });
            else
                return Json(new { success = false, message = "Item Not Deleted Successfully" });
        }
        #endregion

    }
}
