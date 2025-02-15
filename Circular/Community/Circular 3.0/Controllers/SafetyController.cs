using AutoMapper;
using Circular.Core.Entity;
using Circular.Core.DTOs;
using Circular.Framework.Utility;
using Circular.Services.Message;
using Circular.Services.Safety;
using CircularWeb.Business;
using CircularWeb.Models;
using IronBarCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Circular.Data.Repositories.User;
using CircularWeb.filters;
using NLog;
using Circular.Services.Community;
using DocumentFormat.OpenXml.Office.Word;
using Nancy.Diagnostics.Modules;

namespace CircularWeb.Controllers
{
    [Authorize]
    public class SafetyController : Controller
    {
        private readonly ISafetyService _SafetyService;
        private readonly IMessageService _MessageService;
		public SafetyModel safetyModel = new SafetyModel();
		private readonly IGlobal _global;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly IConfiguration _config;
		private readonly IMapper _mapper;
		private readonly IHelper _helper;
        private readonly ICommunityService _communityService;
        private string OIDCUrl;


        public SafetyController(ISafetyService safetyService, IMapper mapper,IConfiguration configuration, IHelper helper, ICommunityService communityService
            , IWebHostEnvironment webHostEnvironment , IHttpContextAccessor httpContextAccessor, IMessageService messageService, ICustomerRepository customerRepository)
        {
            _SafetyService = safetyService;
            _MessageService = messageService;
			_mapper = mapper;
			_config = configuration;
			_webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _global = new Global(_httpContextAccessor, _config,customerRepository);
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
			_communityService = communityService;
            if (string.IsNullOrEmpty(configuration["OIDCUrl"]))
                throw new ArgumentNullException("configuration : OIDCUrl is not defined in App Setting");
            OIDCUrl = Convert.ToString(configuration["OIDCUrl"]);
        }


   
		[HttpGet]
        [ActionLog("Safety", "{0} opened safety.")]
        public async Task<ActionResult> Safety()
        {
            CurrentUser currentUser = _global.GetCurrentUser();
            long CommunityId = currentUser.PrimaryCommunityId;
            long Loggedinuser = currentUser.Id;

            safetyModel.lstvehicles = await _SafetyService.GetVehiclesAsync(CommunityId);
			safetyModel.lstDisplaycode = await _SafetyService.GetPriceDisplayAsync(CommunityId);
			safetyModel.lstTicketsale = await _SafetyService.GetTicketsaleitemAsync(CommunityId);
			safetyModel.lstCommunityClasses = await _SafetyService.GetaClassNameAsync(CommunityId);  
			safetyModel.lstcommclassteachers = await _SafetyService.GetTeacherNameAsync(CommunityId);
            safetyModel.currencyModel.CurrencyCode = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.OtherPhone).ToString();
			safetyModel.getCommunityGuidanceWellnessOptions =  (_SafetyService.GetWellnessCounsellingByID(currentUser.PrimaryCommunityId)).Result.wellnessOptions;
            safetyModel.CommunityLogo = currentUser.CustomerInfo.PrimaryCommunity.CommunityLogo;
            safetyModel.CommunityFeatures = _communityService.Features(CommunityId,Loggedinuser).Result.ToList();
            safetyModel.IsOwner = currentUser.CustomerInfo.PrimaryCommunity.OwnerCustomerId == Loggedinuser ? true : false;
            safetyModel.SubscriptionStatus = currentUser.SubscriptionStatus;
            if (!safetyModel.IsFeatureAvailable("S-002"))
                throw new ArgumentNullException("Unauthroized : You dont have permission to access this functionality.");
            else
                return View(safetyModel);



           
        }


		#region "Safety - Vehicle "

		[HttpPost]
        [ActionLog("Safety", "{0} added a vehicle.")]
        public async Task<ActionResult> AddVehicles([FromBody] VehiclesDTO obj)
        {
			obj.CommunityId = _global.currentUser.PrimaryCommunityId;
			Vehicles vehicles = _mapper.Map<Vehicles>(obj);
            var result1 = await _SafetyService.VehiclesAsync(vehicles);
            obj = _mapper.Map<VehiclesDTO>(vehicles);
            if (result1 > 0)
                return Json(new { success = true, message = "Vehicle saved successfully!" });
            else
                return Json(new { success = false, message = "Oops. There is some error while saving vehicle. Please try again!" });

        }

		[HttpPost]
		[ActionLog("Safety", "{0} deleted a vehicle.")]
		public async Task<ActionResult> DeleteVehicles([FromBody] DeleteVecDTO obj)
		{
			long deletevehicles = _SafetyService.deletevehicleAsync(obj.id);
			if (deletevehicles > 0)
				return Json(new { success = true, message = "Vehicles deleted successfully!" });
			else
				return Json(new { success = false, message = "Oops. We faced an issue while deleting vehicle. Please try again!" });
		}

		[ActionLog("Safety", "{0} fetched vehicle QR code.")]
		public async Task<IActionResult> QRCodeItems(DeleteVecDTO vec)
		{
			Vehicles vehicle = await _SafetyService.GetQRCodeAsync(vec.id);
			return Json(new { success = true, data = vehicle });
		}

		#endregion

		#region "Safety - Price"

		//price display code
		[HttpPost]
        [ActionLog("Safety", "{0} saved display price QR code.")]
        public async Task<ActionResult> AddPriceDisplayCode([FromBody] CommunityTransportPassDTO obj)
        {
		
				obj.OrgId = _global.currentUser.PrimaryCommunityId;
				CommunityTransportPass PriceDisplay = _mapper.Map<CommunityTransportPass>(obj);
				var result2 = await _SafetyService.PostPriceDisplayAsync(PriceDisplay);
				obj = _mapper.Map<CommunityTransportPassDTO>(PriceDisplay);
			  safetyModel.currencyModel.CurrencyCode = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.OtherPhone).ToString();
			if (result2 > 0)
					return Json(new { success = true, message = "Price display code saved successfully!" });
				else
					return Json(new { success = false, message = "Opps..something went wrong" });
        }

        [ActionLog("Safety", "{0} fetched active price QR codes.")]
        public async Task<IActionResult> QRCodePriceItems(DeleteTransDTO trans)
        {
            CommunityTransportPass lstQRcodeprice = await _SafetyService.GetQRCodePriceAsync(trans.id);
            return Json(new { success = true, data = lstQRcodeprice });
        }      
        [HttpPost]

        [ActionLog("Safety", "{0} deleted display price QR code.")]
        public async Task<ActionResult> DeletePriceDisplayCode([FromBody] DeleteTransDTO obj)
        {
            long deleteprice = _SafetyService.DeletePriceDisplayCodeAsync(obj.id);
            if (deleteprice > 0)
                return Json(new { success = true, message = "Display price QR code deleted successfully!" });
            else
                return Json(new { success = false, message = "Oops. Something went wrong!" });
        }

        [ActionLog("Safety", "{0} fetched price per km.")]
        public async Task<IActionResult> GetPricePerKm(priceperkmrequestDTO obj)
		{
			var lstpriceperkm = await _SafetyService.GetPricePerKmAsync(_global.currentUser.PrimaryCommunityId);
			return Json(new { success = true, Data = lstpriceperkm });
		}

		[HttpPost]
        [ActionLog("Safety", "{0} updated price per km.")]
        public async Task<ActionResult> UpdatePriceperkm(priceperkmrequestDTO obj)
		{
			long ppkm = _SafetyService.UpdatePriceperkmAsync(_global.currentUser.PrimaryCommunityId, obj.PricePerKm);
			if (ppkm > 0)
				return Json(new { success = true, message = "" });
			else
				return Json(new { success = false, message = "" });
		}

		#endregion

		#region "Safety - Ticket"
		[HttpPost]
        [ActionLog("Safety", "{0} created new ticket.")]
		public async Task<ActionResult> AddNewTicket(TicketsDTO obj)
		{
			var logger = LogManager.GetLogger("database");
			long Loggedinuser = _global.currentUser.Id;
			obj.CommunityId = _global.currentUser.PrimaryCommunityId;
			logger.Info(obj.EndDate.ToString());

			TicketDays newticketcreated = _mapper.Map<TicketDays>(obj);
			newticketcreated.CreatedBy = Loggedinuser;
			newticketcreated.ModifiedBy = Loggedinuser;
			try
			{
				logger.Info("Before Add new Ticket");
				newticketcreated.FillDefaultValues();
				string cmd = "Exec [dbo].[Usp_Safety_AddTicket] "
				+ newticketcreated.CommunityId + ",'" + newticketcreated.TicketName + "','" + newticketcreated.TicketPrice + "','"
				+ newticketcreated.TicketCount + "','" + newticketcreated.StartDate.ToString("MM-dd-yyyy HH:mm:ss")
				+ "','" + newticketcreated.EndDate.ToString("MM-dd-yyyy HH:mm:ss") + "','" + newticketcreated.TicketTime.ToString("MM-dd-yyyy HH:mm:ss") + "'," + newticketcreated.CreatedBy;
				logger.Info(cmd);
				var result = await _SafetyService.AddNewTicketAsync(newticketcreated);
				safetyModel.currencyModel.CurrencyCode = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.OtherPhone).ToString();
				obj = _mapper.Map<TicketsDTO>(newticketcreated);
				if (result > 0)
					return Json(new { success = true, message = "Ticket added successfully!" });
				else
					return Json(new { success = false, message = "Oops. something went wrong!" });
			}
			catch (Exception ex)
			{
				logger.Info(ex.Message);
			}
			return Json(new { success = false, message = "" });

		}

		[HttpPost]
        [ActionLog("Safety", "{0} deleted ticket.")]
        public async Task<ActionResult> DeleteTicket(long Id)
		{
			var deletetickets = await _SafetyService.DeleteTicketAsync(Id);
			if (deletetickets > 0)
				return Json(new { success = true, message = "Deleted Successfully!" });
			else
				return Json(new { success = false, message = "Not Successfully!" });
		}

        [ActionLog("Safety", "{0} fetched tickets list.")]
        public async Task<IActionResult> ViewTicket(long Id)
		{
			var lstViewTicket = await _SafetyService.GetViewTicketAsync(Id);
			safetyModel.currencyModel.CurrencyCode = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.OtherPhone).ToString();
			return Json(new { success = true, Data = lstViewTicket });
		}

        [ActionLog("Safety", "{0} fetched sold tickets.")]
        public async Task<IActionResult> TicketSold(long Id)
		{
			var lstTicketsold = await _SafetyService.GetTicketSoldList(Id);
			safetyModel.currencyModel.CurrencyCode = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.OtherPhone).ToString();
			return Json(new { success = true, Data = lstTicketsold });
		}

        [ActionLog("Safety", "{0} fetched travel ticket details.")]
        public async Task<IActionResult> Ticketsaleitem(TicketsDTO obj)
		{
			long Community = _global.currentUser.PrimaryCommunityId;
			long Loggedinuser = _global.currentUser.Id;
			obj.CommunityId = Community;
			var lstTicketsale = await _SafetyService.GetTicketsaleitemAsync(obj.CommunityId ?? 0);
			return Json(new { success = true, Data = lstTicketsale });
		}

		#endregion

		#region Safety - Attendance QR 
		[ActionLog("Safety", "{0} fetched Attendance QR code.")]
		public async Task<IActionResult> AttendanceQRCode(DeleteVecDTO vec)
		{
			QR attendance = _SafetyService.GetAttendanceQRCode(_global.currentUser.PrimaryCommunityId);
			return Json(new { success = true, data = attendance });
		}
		#endregion

		#region Safety -Travell Status
		[ActionLog("Safety", "{0} fetched travel status details.")]
		public async Task<IActionResult> travelstatusItems(modalforstatusDTO obj)
		{
			long Community = _global.currentUser.PrimaryCommunityId;
			obj.CommunityId = Community;
			var lstTravellerStatus = await _SafetyService.GetTravellerStatusAsync(obj.CommunityId, obj.VehicleId, obj.TravelDate);
			return Json(new { success = true, Data = lstTravellerStatus });
		}

		#endregion

		#region Safety - SickNotes Submission
		[ActionLog("Safety", "{0} fetched sick note details.")]
		public async Task<IActionResult> SickNoteItems(NotesDetailsDTO obj)
		{
			obj.communityid = _global.currentUser.PrimaryCommunityId;
			var lstSickNotes = await _SafetyService.GetSickNotesAsync(0, obj.communityid, obj.classid, 
				obj.Fromdatesick, obj.Teacherid);
			return Json(new { success = true, Data = lstSickNotes });
		}

		#endregion

		#region Safety- Attendance 
		[ActionLog("Safety", "{0} fetched attendance registry items.")]
		public async Task<IActionResult> AttendanceRegistryItems(AttendanceRegistryRequestDTO obj)
		{
			var logger = LogManager.GetLogger("database");

			logger.Info(obj.STARTDATE.ToString());

			long Community = _global.currentUser.PrimaryCommunityId;
			obj.Communityid = Community;
			var lstAttendanceRegistry = await _SafetyService.GetAttendanceRegistryAsync(obj);
			if(lstAttendanceRegistry != null)
			logger.Info(lstAttendanceRegistry.Count);
			else
				logger.Info("Null");


			return Json(new { success = true, Data = lstAttendanceRegistry });
		}

		#endregion

		#region User Management
		[ActionLog("Safety", "{0} fetched user contact/name.")]
		public async Task<ActionResult> GetUserContactList(string Search)
		{
			long communityId = _global.GetCurrentUser().PrimaryCommunityId;
			var userContactList = await _MessageService.GetUserContactListAsync(communityId, Search);
			var userContact = (userContactList?.Select(u => _mapper.Map<UserContactList>(u)).ToList());
			return Json(userContact);
		}

		[ActionLog("Safety", "{0} activated scanner check.")]
		public async Task<ActionResult> ActivateScannerSafety(string Mobileno, int PermissionId)
		{
            long communityId = _global.GetCurrentUser().PrimaryCommunityId;
            var result = await _SafetyService.AttendanceSafety(Mobileno, PermissionId, communityId);
			if (result.Count() > 0)
				return Json(new { success = true, message = "Your item is now LIVE in your community" });
			else
				return Json(new { success = false, message = "Oops! Something went wrong. Please try again!" });

		}


		[HttpPost]
		[ActionLog("Safety", "{0} deleted specific user management.")]
		public async Task<ActionResult> DeleteUserSafety(long Id)
		{
			var deleteuser = await _SafetyService.DeleteUserManagementSafety(Id);
			if (deleteuser > 0)
				return Json(new { success = true, message = "Deleted Successfully!" });
			else
				return Json(new { success = false, message = "Not Successfully!" });
		}


		[ActionLog("Safety", "{0} fetched transport scanner tick.")]
		public async Task<ActionResult> UserManagementSafety(long PermissionId)
		{
            long CommunityId = _global.GetCurrentUser().PrimaryCommunityId;
            var result = await _SafetyService.GetManageUserSafety(PermissionId, CommunityId);
			if (result != null)
				return Json(new { success = true, message = "Your item is now LIVE in your community", data = result });
			else
				return Json(new { success = false, message = "Oops! Something went wrong. Please try again!" });
		}



		[ActionLog("Safety", "{0} updated safety scanner check or uncheck.")]
		public async Task<IActionResult> UpdateAttendanceScanner(long Id, long IsScannerActive, int PermissionId)
		{
			var result = await _SafetyService.UpdateAttendanceScannerAsycn(Id, Convert.ToBoolean(IsScannerActive), PermissionId);
			if (result > 0)
				return Json(new { success = true, message = "Item Update Successfully" });
			else
				return Json(new { success = false, message = "Item Not Deleted Successfully" });
		}


		#endregion



		//QR Attendance

		[ActionLog("Safety", "{0} created and saved QR code attendence.")]
        public async Task<IActionResult> QRCodeAttendence(QRAttendanceScannerDTO obj)
		{

			long Community = _global.currentUser.PrimaryCommunityId;
			QRAttendanceScannerDTO generateQRCode = new QRAttendanceScannerDTO();
			GeneratedBarcode barcode = QRCodeWriter.CreateQrCode(Community.ToString());
			barcode.SetMargins(10);
			barcode.ChangeBarCodeColor(Color.Black);
			string path = Path.Combine(_webHostEnvironment.WebRootPath, "GenerateAttendanceQR");
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			string QrCode = ""; //  GetUniqueFileName(Community.ToString());
			string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "GenerateAttendanceQR/" + QrCode + ".png");
			barcode.SaveAsPng(filePath);
			string fileName = Path.GetFileName(filePath);
			string imageUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/GenerateAttendanceQR/" + fileName;
			ViewBag.QrCodeUri = imageUrl;
			obj.AttendanceScannerImage = imageUrl;


			long result2 = _SafetyService.SaveQRCodeAttendence(obj.AttendanceScannerImage, Community);
			if (result2 > 0)
				return Json(new { success = true, message = "Price Save Successfully!" });
			else
				return Json(new { success = false, message = "Price  not Save Successfully!" });
		}

        [ActionLog("Safety", "{0} showed QR code.")]
        public async Task<IActionResult> ShowQRCode(Communities obj)
		{
			List<Communities> QR = await _SafetyService.ShowQRCodeAsync(obj.Id);
			return Json(new { success = true, data = QR });
		}













        #region Safety- Wellness

        [HttpGet]
        [ActionLog("Safety", "{0} fetched guidance wellness.")]
        public async Task<ActionResult> GetGuidance(CommunityGuidanceWellnessOptionsDTO communityWellnessButtonsDTO)
        {
            var result = (await _SafetyService.GetGuidance(communityWellnessButtonsDTO.Id));
            return Json(new { data = result, success = true });
        }

        [HttpPost]
        [ActionLog("Safety", "{0} updated guidance wellness.")]
        public async Task<ActionResult> UpdateGuidancee(CommunityGuidanceWellnessOptionsDTO communityWellnessButtonsDTO)
        {
			try
			{
				CommunityGuidanceWellnessOptions council = _mapper.Map<CommunityGuidanceWellnessOptions>(communityWellnessButtonsDTO);
                var result = await _SafetyService.UpdateGuidance(council);
				if (result > 0)
					return Json(new { success = true, message = "Your item is now LIVE in your community", data = result });
				else
					return Json(new { success = false, message = " Data Not Saved Successfully!" });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = "Oops! something went wrong" });
			}
		}


        [HttpGet]
        [ActionLog("Safety", "{0} fetched wellness and counselling.")]
        public async Task<IActionResult> GetWellnessCounsellingByID()
        {
                var result = (await _SafetyService.GetWellnessCounsellingByID(_global.currentUser.PrimaryCommunityId));
                return Json(new { data = result, success = true});
		}

        [HttpPost]
        [ActionLog("Safety", "{0} updated wellness and counselling.")]
        public async Task<ActionResult> UpdateWellnessCounselling(CommunityGuidanceWellnessDTO communityGuidanceWellnessDTO)
        {
			try
			{
				long result = 0;
                if (communityGuidanceWellnessDTO != null)
				{
					communityGuidanceWellnessDTO.CommunityId = _global.GetCurrentUser().PrimaryCommunityId;
					if (communityGuidanceWellnessDTO.Image != null)
					{
						communityGuidanceWellnessDTO.CoverImage = _helper.SaveFile(communityGuidanceWellnessDTO.Image, _global.UploadFolderPath, this.Request);
					}

					CommunityGuidanceWellness communityGuidanceWellness = _mapper.Map<CommunityGuidanceWellness>(communityGuidanceWellnessDTO);
					if (communityGuidanceWellnessDTO.Id > 0)
						result = await _SafetyService.UpdateWellnessCounselling(communityGuidanceWellness);
					else
					{
                        if (string.IsNullOrEmpty(communityGuidanceWellness.CoverImage))
                            return Json(new { success = false, message = "Oops. Something went wrong. Please check the data and try again!" });

                        result = await _SafetyService.WellnessCounselling(communityGuidanceWellness);
					}
                }

                if (result > 0)
					return Json(new { success = true, message = "Data Update Save Successfully!" });
				else
					return Json(new { success = false, message = "Oops. Something went wrong. Please check the data and try again!" });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = "Oops! something went wrong" });
			}
		}


        #endregion


    }
} 
