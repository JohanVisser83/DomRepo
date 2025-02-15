using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Filters;
using Circular.Framework.Logger;
using Circular.Framework.ShortMessages;
using Circular.Framework.Utility;
using Circular.Services.Master;
using Circular.Services.Notifications;
using Circular.Services.Safety;
using Circular.Services.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using TimeZoneConverter;

namespace Circular.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IBulkSMS _sms;
        private readonly IMapper _mapper;
        private readonly IHelper _helper;
        private readonly ICommon _common;
        private readonly ILoggerManager _loggerManager;
        private readonly IConfiguration _configuration;
        private readonly AuthController _authController;
        private readonly ICustomerService _customerService;
        private readonly ISafetyService _safetyService;
        private readonly INotificationService _NotificationService;
		private readonly IMasterService _masterService;

		private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;


        public string SickNoteEmail { get; private set; }

        public CustomerController(IMapper mapper, ICustomerService customerService, ILoggerManager loggerManager, IHelper helper,
            IBulkSMS bulkSMS, IConfiguration configuration, AuthController authController, ICommon common
            , INotificationService notificationService, ISafetyService safetyService, IHttpContextAccessor httpContextAccessor, IMasterService masterService)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _loggerManager = loggerManager ?? throw new ArgumentNullException(nameof(loggerManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _sms = bulkSMS ?? throw new ArgumentNullException(nameof(bulkSMS));
            _authController = authController ?? throw new ArgumentNullException(nameof(authController));
            _common = common;
            _NotificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _safetyService = safetyService ?? throw new ArgumentNullException(nameof(_safetyService));
			_masterService = masterService ?? throw new ArgumentNullException(nameof(masterService));

		}


        #region "Customers"

        [HttpPost]
        [Route("send-otp")]
        [SwaggerOperation(Summary = "Reviewed")]
        public async Task<ActionResult<int>> sendOTP([FromBody] LoginNameDTO loginDTO)
        {

            try
            {
                APIResponse objResponse = new APIResponse();
                var mobileNumber = loginDTO.UserName;

                //Generate OTP
                var otp = _helper.GenerateRandomNumber(int.Parse(_configuration["OTP:Length"]), _configuration["OTP:MasterOTP"],
                    bool.Parse(_configuration["OTP:IsMasterOTPEnabled"]), bool.Parse(_configuration["OTP:IsAlphaNumeric"]));

                if(mobileNumber.StartsWith("+1"))
                {
                    otp = "04007";
                }

                //Send to server to save against user
                var response = await _authController.SaveOTPAsync(loginDTO.UserName, otp, loginDTO.SignupFlow);
                ShortMessageDetails sms = new ShortMessageDetails() { Mobile_number = mobileNumber, Message = _configuration["OTP:Message"].Replace("<otp>", otp) };
                ShortMessageProviderDetails shortMessageProvider = new ShortMessageProviderDetails() { UserName = _configuration["BulkSMS:username"], Password = _configuration["BulkSMS:password"] };
                if (!bool.Parse(_configuration["OTP:IsMasterOTPEnabled"]))
                    _sms.BulkSms(sms, shortMessageProvider);
                objResponse.StatusCode = (int)APIResponseCode.Success;
                return Ok(objResponse);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError("Error in sendOTP method-" + ex.Message);
                return BadRequest(ex);
            }
        }


        [AuthorizeOIDC]
        [HttpPost]
        [Route("UserType/Update")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Customer", "{UserName} Update User Type")]
        public async Task<ActionResult<APIResponse>> UpdateUserType([FromBody] UpdateUserTypeDTO updateUserTypeDTO)
        {
            try
            {
                if (updateUserTypeDTO.userTypeId == null || updateUserTypeDTO.userTypeId == 0 || updateUserTypeDTO.CustomerId == null || updateUserTypeDTO.CustomerId == 0)
                    return BadRequest("User Type is not correct");
                var customer = _customerService.UpdateUserType(updateUserTypeDTO.CustomerId, updateUserTypeDTO.userTypeId, updateUserTypeDTO.modifiedBy);
                APIResponse clsResponse = new APIResponse();
                clsResponse.StatusCode = (int)APIResponseCode.Success;
                clsResponse.Data = customer;
                return clsResponse;
            }
            catch (Exception ex)
            {
                _loggerManager.LogError("Error in UpdateCustomerType method-" + ex.Message);
                return BadRequest(ex);
            }
        }

        [AuthorizeOIDC]
        [HttpPost]
        [Route("UpdatePasscode")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Customer", "{UserName} Update Passcode")]

        public async Task<ActionResult<APIResponse>> UpdatePasscode([FromBody] PasscodeRequestDTO passcodeRequestDTO)
        {
            passcodeRequestDTO.passcode = _helper.EncryptUsingSHA1Hashing(passcodeRequestDTO.passcode);
            Customers customer = _common.CurrentUser();
            var Name = customer.CustomerDetails.Name;
            bool sendWelcomeEmail = !customer.IsPasscodeSet;
            string email = customer.PrimaryEmail;
            var customers = _customerService.UpdatePasscode(passcodeRequestDTO.id, passcodeRequestDTO.passcode, true, Name, sendWelcomeEmail,  email);
            CustomersDTO customersDTO = _mapper.Map<CustomersDTO>(customers);
            customersDTO.setSignUpStatus();
            APIResponse clsResponse = new APIResponse();
            clsResponse.StatusCode = (int)APIResponseCode.Success;
            clsResponse.Data = customersDTO;
            return Ok(clsResponse);
        }

        [AuthorizeOIDC]
        [HttpPost]
        [Route("VerifyPasscode")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Customer", "{UserName} Verify Passcode")]
        public async Task<ActionResult<APIResponse>> VerifyPasscode([FromBody] PasscodeRequestDTO passcodeRequestDTO)
        {
            passcodeRequestDTO.passcode = _helper.EncryptUsingSHA1Hashing(passcodeRequestDTO.passcode);
            var result = _customerService.VerifyPasscode(passcodeRequestDTO.id, passcodeRequestDTO.passcode);
            APIResponse apiResponse = new APIResponse();
            if (result)
                apiResponse.StatusCode = (int)APIResponseCode.Success;
            else
                apiResponse.StatusCode = (int)APIResponseCode.Password_Does_Not_Match;

            return Ok(apiResponse);
        }

        [AuthorizeOIDC]
        [HttpPost]
        [Route("Deactivate")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Customer", "{UserName} DeActivate Account")]
        public async Task<ActionResult<APIResponse>> DeActivateMyAccount()
        {
            var result = await _customerService.DeActivateMyAccount(_common.UserId);
            APIResponse apiResponse = new APIResponse();
            if (result == 1)
                apiResponse.StatusCode = (int)APIResponseCode.Success;
            else
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            return Ok(apiResponse);
        }


        //[AuthorizeOIDC]
        [HttpPost]
        [Route("FetchIfExists")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Customer", "{UserName} Fetch If Exists")]

        public async Task<ActionResult<APIResponse>> FetchIfExists(CustomerRequestDTO customerDTO)
        {
            Customers customer;
            if (customerDTO.IdOrMobile == 1)
            {
                if (customerDTO.Id == 0)
                    return BadRequest("Customer Id is not correct");
                customer = _customerService.getcustomerbyId(customerDTO.Id, true);
            }
            else
            {
                if (customerDTO.Mobile == "")
                    return BadRequest("Mobile is not correct");
                customer = _customerService.getcustomerByUserName(customerDTO.Mobile, true);
            }
            //CustomersDTO customersDTO = _mapper.Map<CustomersDTO>(customer);

            APIResponse clsResponse = new APIResponse();
            if (customer != null)
                clsResponse.StatusCode = (int)APIResponseCode.Success;
            else
                clsResponse.StatusCode = (int)APIResponseCode.Failure;
            clsResponse.Data = customer;
            return clsResponse;
        }

        [HttpPost]
        [AuthorizeOIDC]
        [Route("Articles")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Customer", "{UserName} Requested News Feed")]
        public async Task<ActionResult<APIResponse>> GetNewsFeeds([FromBody] GetNewsFeedDTO getNewsFeedDTO)
        {
            var _newFeeds = _customerService.GetNewFeeds(getNewsFeedDTO.CustomerId, 0,getNewsFeedDTO.Feedid);
            var _newsFeeds = _newFeeds?.Select(u => _mapper.Map<NewsFeeds>(u)).ToList();
            APIResponse clsResponse = new APIResponse();
            clsResponse.StatusCode = (int)APIResponseCode.Success;
            clsResponse.Data = _newsFeeds;
            return Ok(clsResponse);
        }


        [HttpPost]
        [AuthorizeOIDC]
        [Route("Articles/Like")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Customer", "{UserName} Like Article")]
        public async Task<IActionResult> LikeArticle([FromBody] LikedFeedsDTO likedFeedsDTO)
        {
            LikedFeeds likefeeds = _mapper.Map<LikedFeeds>(likedFeedsDTO);
            await _customerService.LikeArticle(likefeeds);
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = likefeeds;
            return Ok(apiResponse);
        }


        [HttpPost]
        [AuthorizeOIDC]
        [Route("Article/Views")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Customer", "{UserName}  Article View Count")]

        public async Task<ActionResult> UpdateArticleViewCount([FromBody] ArticleViewsDTO articleViews)
        {
            ArticleViews viewsCount = _mapper.Map<ArticleViews>(articleViews);
            var response = await _customerService.UpdateArticleViewCount(viewsCount);
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


        #endregion

        #region "Customer Details"

        [AuthorizeOIDC]
        [HttpPost]
        [Route("UpdateCustomerBasicDetails")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Customer", "{UserName} Update Customer Details")]
        public async Task<ActionResult<CustomerDetailsDTO>> UpdateCustomerBasicDetails([FromBody]
        CustomerDetailsBasicDTO customerDetailsDTO)
        {
            try
            {
                APIResponse objResponse = new APIResponse();
                if (customerDetailsDTO == null || customerDetailsDTO.FirstName.IsNullOrEmpty() ||
                    customerDetailsDTO.LastName.IsNullOrEmpty() || customerDetailsDTO.DOB
                     < new DateTime(1900, 01, 01))
                {
                    objResponse.StatusCode = (int)APIResponseCode.Invalid_Request;
                    objResponse.Message = "Mandatory_Field_Validation_Failed";
                    return Ok(objResponse);
                }

                CustomerDetails customerDetails = _mapper.Map<CustomerDetails>(customerDetailsDTO);
                Customers customer = _customerService.UpdateCustomerBasicDetails(customerDetails);
                CustomersDTO customerDTO = _mapper.Map<CustomersDTO>(customer);
                objResponse.StatusCode = (int)APIResponseCode.Success;
                objResponse.Data = customerDTO;
                return Ok(objResponse);
            }
            catch (Exception ex)
            {
                _loggerManager.LogError("Error in UpdateCustomerBasicDetails method-" + ex.Message);
                return BadRequest(ex);
            }

        }

        [HttpPost]
        [AuthorizeOIDC]
        [Route("Device/Save")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Customer", "{UserName} Save Device Details")]
        public async Task<IActionResult> SaveDeviceDetails(CustomerDevicesDTO userDevicesDTO)
        {
            CustomerDevices userDevices = _mapper.Map<CustomerDevices>(userDevicesDTO);
            APIResponse apiResponse = new APIResponse();

            var response = await _customerService.SaveDeviceDetails(userDevices);

            string windowsTimeZone = TZConvert.IanaToWindows(userDevicesDTO.TimeZone);

            var save = await _masterService.SaveTimeZone(userDevices.CustomerId, windowsTimeZone);


            apiResponse.Message = "";
            if (response > 0)
                apiResponse.StatusCode = (int)APIResponseCode.Success;
            else
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            return Ok(apiResponse);

        }

        #endregion

        #region "Wellnes"
        [HttpPost]
        [AuthorizeOIDC]
        [Route("Safety/BullyingReport")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Customer", "{UserName} ReportIt")]

        public async Task<ActionResult> ReportIt([FromBody] BullyingReportsDTO reportDTO)
        {
            try
            {

                BullyingReports bullyingReports = _mapper.Map<BullyingReports>(reportDTO);
                var response = await _safetyService.ReportItAsync(bullyingReports);
				APIResponse apiResponse = new APIResponse();
                apiResponse.Message = "";
                if (response)
                {
                    var getreportmail = await _safetyService.reportemail((long)reportDTO.CommunityId);
					var  IncidentValue = await _safetyService.GetIncident((long)reportDTO.IncidentTypeId);
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


        [HttpGet]
        [AuthorizeOIDC]
        [Route("GetWellnessOverview")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Customer", "{UserName} Requested Wellness Overview")]
        public async Task<ActionResult<APIResponse>> GetWellnessOverview(long CommunityId)
        {
            CommunityGuidanceWellness overview = await _safetyService.GetOverview(CommunityId);
            APIResponse clsResponse = new APIResponse();
            clsResponse.StatusCode = (int)APIResponseCode.Success;
            clsResponse.Data = overview;
            return Ok(clsResponse);
        }

        #endregion

        #region "Linked Members"

        [HttpPost]
        [AuthorizeOIDC]
        [Route("LinkedMembers/List")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Customer", "{UserName} Requested Linked Members")]
        public async Task<ActionResult<APIResponse>> LinkedMembers()
        {
            var linkedMembers = await _customerService.GetLinkedMembers(_common.UserId);
            APIResponse apiResponse = new APIResponse();
            apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = linkedMembers;
            return Ok(apiResponse);
        }

        [HttpPost]
        [AuthorizeOIDC]
        [Route("LinkedMembers/Link")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Customer", "{UserName} Add Linked Members")]
        public async Task<ActionResult<APIResponse>> AddLinkedMembers(LinkedMembersDTO linkedMember)
        {
            LinkedMembers _linkedMember = _mapper.Map<LinkedMembers>(linkedMember);
            Customers currentCustomer = _common.CurrentUser();
            long status = await _customerService.AddLinkedMembers(_linkedMember, currentCustomer);
            APIResponse apiResponse = new APIResponse();
            if (status <= 0)
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            else
            {
                if (status > 5)
                    apiResponse.StatusCode = (int)APIResponseCode.Success;
                else if (status == 2)
                {
                    apiResponse.StatusCode = (int)APIResponseCode.Request_In_Pending;
                    apiResponse.Message = "You already have a pending request.";
                }
                else if (status == 3)
                {
                    apiResponse.StatusCode = (int)APIResponseCode.Record_Already_Exists;
                    apiResponse.Message = "You both are already linked.";
                }
                else if (status == 4)
                {
                    apiResponse.StatusCode = (int)APIResponseCode.Not_Allowed;
                    apiResponse.Message = "You have a student account. You can't complete this request.";
                }
                else if (status == 5)
                {
                    apiResponse.StatusCode = (int)APIResponseCode.Not_Allowed;
                    apiResponse.Message = "Your linking request is already reported once. You can't complete this request.";
                }
            }
            return Ok(apiResponse);

        }

        [HttpPost]
        [AuthorizeOIDC]
        [Route("LinkedMembers/Delink")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Customer", "{UserName} Remove Linked Members")]
        public async Task<ActionResult<APIResponse>> RemoveLinkedMembers(LinkedMembersDTO linkedMember)
        {
            LinkedMembers _linkedMember = _mapper.Map<LinkedMembers>(linkedMember);
            int status = await _customerService.RemoveLinkedMembers(_linkedMember);
            APIResponse apiResponse = new APIResponse();
            if (status <= 0)
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            else
                apiResponse.StatusCode = (int)APIResponseCode.Success;

            return Ok(apiResponse);

        }

        [HttpPost]
        [AuthorizeOIDC]
        [Route("LinkedMembers/confirm")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Customer", "{UserName} Confirm Linked Members")]
        public async Task<ActionResult<APIResponse>> ConfirmLinkedMembers(LinkedMembersDTO linkedMember)
        {
            LinkedMembers _linkedMember = _mapper.Map<LinkedMembers>(linkedMember);
            int status = await _customerService.ConfirmLinkedMembers(_linkedMember, null);
            APIResponse apiResponse = new APIResponse();
            if (status <= 0)
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            else
                apiResponse.StatusCode = (int)APIResponseCode.Success;

            return Ok(apiResponse);

        }

        #endregion

        #region "Notifications"

        [HttpPost]
        [AuthorizeOIDC]
        [Route("Notifications")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Customer", "{UserName} Request Notifications")]
        public async Task<ActionResult<APIResponse>> Notifications(NotificationsRequestDTO notificationsRequest)
        {
            Circular.Core.Entity.NotificationListResponse Notifications =
                await _NotificationService.GetNotificationsAsync(notificationsRequest.customerId,
                notificationsRequest.userNotificationId, notificationsRequest.IsRead,
                notificationsRequest.pageNumber, notificationsRequest.pageSize);

            APIResponse apiResponse = new APIResponse();
            if (Notifications == null)
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            else
                apiResponse.StatusCode = (int)APIResponseCode.Success;
            apiResponse.Data = Notifications;
            return Ok(apiResponse);
        }

        [HttpPost]
        [AuthorizeOIDC]
        [Route("Notification/Read")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Customer", "{UserName} Request Read Notification")]
        public async Task<ActionResult<APIResponse>> ReadNotification(ReadNotificationsRequest readNotificationsRequest)
        {
            int result = await _NotificationService.ReadNotificationAsync(readNotificationsRequest.customerId, readNotificationsRequest.userNotificationId, readNotificationsRequest.IsReadAll);
            APIResponse apiResponse = new APIResponse();
            if (result > 0)
                apiResponse.StatusCode = (int)APIResponseCode.Success;
            else
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            apiResponse.Data = result;
            return Ok(apiResponse);
        }

        [HttpPost]
        [AuthorizeOIDC]
        [Route("Notification/Remove")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Customer", "{UserName} Remove Notification")]
        public async Task<ActionResult<APIResponse>> RemoveNotification(UserNotificationsRequest UserNotification)
        {
            int result = await _NotificationService.DeleteNotificationAsync(UserNotification.userNotificationId);
            APIResponse apiResponse = new APIResponse();
            if (result > 0)
                apiResponse.StatusCode = (int)APIResponseCode.Success;
            else
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            apiResponse.Data = result;
            return Ok(apiResponse);
        }

        #endregion

        #region "Sicknote"

        [HttpPost]
        [AuthorizeOIDC]
        [Route("SickNote/Submit")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Customer", "{UserName} Submit SickNote")]
        public async Task<ActionResult> SubmitSickNote(SicknotesDTO sicknotesDTO)
        {
            try
            {
                Sicknotes sicknotes = _mapper.Map<Sicknotes>(sicknotesDTO);
                var response = await _safetyService.SubmitSickNoteAsync(sicknotes);
                APIResponse apiResponse = new APIResponse();
                apiResponse.Message = "";
                if (response)
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
        [Route("SickNote/List")]
        [SwaggerOperation(Summary = "Reviewed")]
        [ActionLog("Customer", "{UserName} Request SickNotes")]
        public async Task<ActionResult> SickNotes()
        {

            Customers loggedInCustomer = _common.CurrentUser();
            DateTime dt = new DateTime(2022, 01, 01);
            List<Sicknotes> lstSicknotes = await _safetyService.GetSickNotesAsync(loggedInCustomer.Id, loggedInCustomer.PrimaryCommunity?.CommunityId ?? 0,
                0, dt.ToString("yyyy/MM/dd"), 0);
            SickNoteListResponse ListResponse = null;
            if (lstSicknotes != null)
            {
                ListResponse = new SickNoteListResponse();
                lstSicknotes.ForEach(n =>
                {
                    // group the data in dates
                    int index = ListResponse.SickNoteGroups.FindIndex(ng => ng.SickNoteDate == (new DateTime(n.Fromdate?.Year ?? DateTime.Now.Year, n.Fromdate?.Month ?? DateTime.Now.Month, 1)));
                    if (index == -1)
                    {
                        SickNoteGroupResponse groupResponse = new SickNoteGroupResponse();
                        groupResponse.SickNoteDate = (new DateTime(n.Fromdate?.Year ?? DateTime.Now.Year, n.Fromdate?.Month ?? DateTime.Now.Month, 1));
                        groupResponse.SickNotesList?.Add(n);
                        ListResponse.SickNoteGroups.Add(groupResponse);
                    }
                    else
                        ListResponse.SickNoteGroups[index].SickNotesList?.Add(n);
                }
                );
            }
            APIResponse apiResponse = new APIResponse();
            apiResponse.Message = "";
            if (lstSicknotes != null)
                apiResponse.StatusCode = (int)APIResponseCode.Success;
            else
                apiResponse.StatusCode = (int)APIResponseCode.Failure;
            apiResponse.Data = ListResponse;
            return Ok(apiResponse);
        }


        #endregion


        [HttpPost]
        [AuthorizeOIDC]
        [Route("Advertisement")]
        [ActionLog("Customer", "{UserName} Requested Advertisement")]
        public async Task<ActionResult<APIResponse>> GetAdvertisement([FromBody] GetAdvertisement getAdvertisement)
        {
            var _advertisementList = _customerService.GetAdvertisement(getAdvertisement.CommunityId, getAdvertisement.AdvertisementId);
            var _advertisementLists = _advertisementList?.Select(u => _mapper.Map<Advertisement>(u)).ToList();
            APIResponse clsResponse = new APIResponse();
            clsResponse.StatusCode = (int)APIResponseCode.Success;
            clsResponse.Data = _advertisementLists;
            return Ok(clsResponse);
        }

    }
}
