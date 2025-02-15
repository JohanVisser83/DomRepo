using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Data.Repositories.User;
using Circular.Services.Community;
using Circular.Services.Setting;
using Circular.Services.Storefront;
using Circular.Services.User;
using CircularWeb.Business;
using CircularWeb.filters;
using CircularWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using Stripe;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;


namespace CircularWeb.Controllers
{
    [Authorize]
    public class SettingController : Controller
    {
        private readonly ISettingService _SettingService;
        private IServiceProvider _provider;
        private string OIDCUrl;
        Customers customer;
        private readonly ICustomerService _customerService;
        private readonly ICommunityService _communityService;
        public SettingModel settingModel = new SettingModel();
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStorefrontServices _storefrontServices;
        private readonly IMapper _mapper;
        private readonly IGlobal _global;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CurrentUser currentUser;
        private readonly IConfiguration _config;

        private const string WEBHOOK_SECRET = "whsec_b4899167ab9178c683ba8db33251a5de2cd4f799b603531b723c6ef07f332821";


        public SettingController(ISettingService settingService, IMapper mapper, IWebHostEnvironment webHostEnvironment, ICommunityService communityService, IStorefrontServices storefrontServices
            , IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ICustomerService customerService, IServiceProvider provider, ICustomerRepository customerRepository)
        {
            _SettingService = settingService;
            if (string.IsNullOrEmpty(configuration["OIDCUrl"]))
                throw new ArgumentNullException("configuration : OIDCUrl is not defined in App Setting");
            OIDCUrl = Convert.ToString(configuration["OIDCUrl"]);
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _config = configuration;
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _global = new Global(_httpContextAccessor, _config,customerRepository);
            _customerService = customerService;
            _communityService = communityService;
            _storefrontServices = storefrontServices ?? throw new ArgumentNullException(nameof(storefrontServices));

        }


        //user permission dropdown
        [ActionLog("Settings", "{0} opened settings.")]
        public async Task<ActionResult> Setting()
        {
            CurrentUser currentUser = _global.GetCurrentUser();
            long Loggedinuser = currentUser.Id;
            long community = currentUser.PrimaryCommunityId;
            settingModel.CommunityFeatures = _communityService.Features(community, Loggedinuser).Result.ToList();
            settingModel.CommunityLogo = currentUser.CustomerInfo.PrimaryCommunity.CommunityLogo;
            settingModel.lstRole = await _SettingService.GetRoles();
            settingModel.lstAccesssControl = await _SettingService.GetAccessControlAsync(community);
            settingModel.lstfeature = await _SettingService.GetFeatureAccess();
            settingModel.lstHouses = await _SettingService.GetHouseList(community);
            settingModel.IsOwner = currentUser.CustomerInfo.PrimaryCommunity.OwnerCustomerId == Loggedinuser ? true : false;

            List<CustomerDetails> CommunityMembers = _communityService.GetCommunityMemberDetails(currentUser.PrimaryCommunityId
                    , 0, currentUser.Id, 0, 0).Result.ToList();
            settingModel.Memberslist = CommunityMembers; ;
            settingModel.Members = CommunityMembers.Where(cm => cm.UsertypeId == 104);
            settingModel.lstCommunityCurrentPlan = await _SettingService.GetCommunitySubscriptionPlan(community);
            settingModel.lstCommunitySubsTransactionlist = await _SettingService.GetCommunitySubsTransactionList(community);
            settingModel.StoreData = await _storefrontServices.DropDownListStore(community);
            settingModel.currency = _config["Currency"];
            settingModel.SubscriptionStatus = currentUser.SubscriptionStatus;
            if (!settingModel.IsOwner)
                throw new ArgumentNullException("Unauthroized : You dont have permission to access this functionality.");
            else
                return View(settingModel);
        }



        //sicknotes alert tab
        [ActionLog("Settings", "{0} fetched sick note.")]
        public async Task<IActionResult> GetSickNotes(long CommunityId)
        {
            var communityId = _global.currentUser.PrimaryCommunityId;
            var getrecord = await _SettingService.GetSickNotesAlert(communityId);
            return Json(new { success = true, Data = getrecord });
        }
        [HttpPost]
        [ActionLog("Settings", "{0} updated sick note.")]
        public async Task<ActionResult> UpdateSickNotes(CommunitiesAlertDTO obj)
        {
            long Id = _global.currentUser.PrimaryCommunityId;
            long varsicknotealert = _SettingService.UpdateSickNotesAlert(Id, obj.SickNoteRecipientName, obj.SickNotePhoneNumber, obj.SickNoteMailBox, obj.SickNoteEmail);
            if (varsicknotealert > 0)
                return Json(new { success = true, message = "Data Updated Successfully!" });
            else
                return Json(new { success = false, message = "Oops! something went wrong" });
        }

        //Report IT alert tab
        [ActionLog("Settings", "{0} fetched report it.")]
        public async Task<IActionResult> GetReportIt(long CommunityId)
        {
            var communityId = _global.currentUser.PrimaryCommunityId;
            var Result = await _SettingService.GetReportItAlert(communityId);
            return Json(new { success = true, Data = Result });
        }
        [HttpPost]
        [ActionLog("Settings", "{0} updated report it.")]
        public async Task<ActionResult> UpdateReportIt(CommunitiesReportDTO obj)
        {
            long Id = _global.currentUser.PrimaryCommunityId;
            long Result = _SettingService.UpdateReportItAlert(Id, obj.RecipientNameReport, obj.EmailAddressReport, obj.PhoneNumberReport, obj.Reportcount);
            if (Result > 0)
                return Json(new { success = true, message = "Data Updated Successfully!" });
            else
                return Json(new { success = false, message = "Oops! something went wrong" });
        }


        //Add A New portal user
        [HttpPost]
        [ActionLog("Settings", "{0} saved new portal user.")]
        public async Task<IActionResult> SaveNewPortalUser([FromBody] AddPermissionDTO obj)
        {
            try
            {
                long Loggedinuser = _global.currentUser.Id;
                long Communityid = _global.currentUser.PrimaryCommunityId;

                obj.Communityid = Communityid;

                var result = await _SettingService.SaveSaveNewPortalUserAsync(obj);

                if (result != null)
                {
                    foreach (var item in obj.CustomerStores)
                    {
                        item.CustomerId = obj.CustomerId;
                    }

                    foreach (var checkedFeature in obj.adminFeatures)
                    {
                        checkedFeature.CustomerId = obj.CustomerId;
                        checkedFeature.CommunityId = Communityid;
                    }

                    if (obj.uncheckedadminFeatures != null)
                    {
                        foreach (var uncheckedFeature in obj.uncheckedadminFeatures)
                        {
                            uncheckedFeature.CustomerId = obj.CustomerId;
                            uncheckedFeature.CommunityId = Communityid;
                        }
                    }
                    List<CustomerStoreFrontAccess> customerStore = _mapper.Map<List<CustomerStoreFrontAccess>>(obj.CustomerStores);
                    List<AdminFeature> checkedFeatures = _mapper.Map<List<AdminFeature>>(obj.adminFeatures);
                    List<AdminFeature> uncheckedFeatures = _mapper.Map<List<AdminFeature>>(obj.uncheckedadminFeatures);

                    // Save the data
                    long storeresult = await _SettingService.SaveCustomerStoreFrontAccess(customerStore);

                    long checkedFeaturesResult = await _SettingService.SaveAdminFeatureAccess(checkedFeatures, uncheckedFeatures);

                    return Json(new { success = true, message = "Your item is now LIVE in your community", data = storeresult });
                }
                else
                {
                    return Json(new { success = false, message = "Oops! something went wrong" });
                }
            }
            catch (Exception ex)
            {
                // Log exception if needed
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }

        }

        //Access control
        [HttpPost]
        [ActionLog("Settings", "{0} deleted access control users.")]
        public async Task<ActionResult> DeleteAcessControl(long Id)
        {
            var record = await _SettingService.DeleteAcessControlAsync(Id);
            if (record > 0)
                return Json(new { success = true, message = "Deleted successfully." });
            else
                return Json(new { success = false, message = "Not Deleted Successfully!" });
        }
        [ActionLog("Settings", "{0} fetched access control details.")]
        public async Task<IActionResult> GetAccessControl(long longgedinuser)
        {
            long community = _global.currentUser.PrimaryCommunityId;

            var lstAccesssControl = await _SettingService.GetAccessControlAsync(community);
            return Json(new { success = true, Data = lstAccesssControl });
        }






        //Audit
        [ActionLog("Settings", "{0} fetched audit by date.")]
        public async Task<IActionResult> AuditByDate(AuditdetailDTO obj)
        {
            var communityId = _global.currentUser.PrimaryCommunityId;

            var lstaudit = await _SettingService.GetAuditByDateAsync(communityId, obj.STARTDATE);
            return Json(new { success = true, Data = lstaudit });
        }

        //security check		

        public async Task<bool> verifypasswordCode(string? password, bool isLoginFlow = false)
        {
            try
            {
                string? userName = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.MobilePhone);
                bool objResponse = false;
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                    return objResponse;

                var resp = await GetTokenCode(userName, password, false);
                if (resp != null && resp.AccessToken != "")
                {
                    objResponse = true;

                    if (!isLoginFlow)
                    {
                        using var client = _provider.GetRequiredService<HttpClient>();
                        using var request = new HttpRequestMessage(HttpMethod.Get, OIDCUrl + "api/Identity");
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", resp.AccessToken);

                        using var response = await client.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                        {
                            string json = await response.Content.ReadAsStringAsync();
                            OpenIDIdentityResponse obj = JsonConvert.DeserializeObject<OpenIDIdentityResponse>(json);
                            resp.UserCode = obj.value;
                            //Circular Specific logic

                            customer = _customerService.getcustomerByUserId(new Guid(resp.UserCode), true);
                            customer.Passcode = "";
                            customer.Password = "BizBrolly@$1234";
                        }
                    }
                }


                return objResponse;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [NonAction]
        async Task<OpenIddictResponse> GetTokenCode(string userName, string password, bool isEncrypted)
        {
            if (isEncrypted)
                password = password.ToLower();

            var service = _provider.GetRequiredService<OpenIddictClientService>();
            Dictionary<string, OpenIddictParameter>? parameters = new Dictionary<string, OpenIddictParameter>();
            parameters.Add("is_encrypted", new OpenIddictParameter(isEncrypted));
            var (response, _) = await service.AuthenticateWithPasswordAsync(new Uri(OIDCUrl), userName, password, parameters: parameters);
            return response;

        }
        [ActionLog("Settings", "{0} fetched specific portal user details.")]
        public async Task<IActionResult> ViewPortalUser(long Id, long customersId)
        {

            var lstViewPortalUser = await _SettingService.GetViewPortalUser(Id);
             var result = await _SettingService.GetAccessStoreName(customersId);
             var adminFeatures = await _SettingService.GetSelectedFeatures(customersId);
            return Json(new { success = true, data = new { lstViewPortalUser, result , adminFeatures } });
        }



        public async Task<IActionResult> RemoveSelectedStore(long Id, long StoreId, long CustomerId)
        {
            try
            {
                var result = await _SettingService.RemoveSelectedStore(Id, StoreId, CustomerId);
                if (result > 0)
                    return Json(new { success = true, message = "Store Deleted Successfully!" });
                else
                    return Json(new { success = false, message = "Store Deleted not Successfully!" });
            }
            catch (Exception ex)
            {
                return null;
            }
            

        }




        [HttpPost]
        [ActionLog("Settings", "{0} updated specific portal user details.")]
        public async Task<ActionResult> UpdatePortalUser([FromBody] AddPermissionDTO obj)
        {
            try
            {
                long Loggedinuser = _global.currentUser.Id;
                long Communityid = _global.currentUser.PrimaryCommunityId;
                obj.Communityid = Communityid;

                var result = await _SettingService.SaveSaveNewPortalUserAsync(obj);

                if (result != null)
                {
                    foreach (var item in obj.CustomerStores)
                    {
                        item.CustomerId = obj.CustomerId;
                    }

                    foreach (var checkedFeature in obj.adminFeatures)
                    {
                        if (obj.adminFeatures.Any())
                        {
                            checkedFeature.CustomerId = obj.CustomerId;
                            checkedFeature.CommunityId = Communityid;
                        }
                    }

                 

                    List<CustomerStoreFrontAccess> customerStore = _mapper.Map<List<CustomerStoreFrontAccess>>(obj.CustomerStores);
                    List<AdminFeature> checkedFeatures = _mapper.Map<List<AdminFeature>>(obj.adminFeatures);
                    List<AdminFeature> uncheckedFeatures = _mapper.Map<List<AdminFeature>>(obj.uncheckedadminFeatures);

                    // Save the data
                    long storeresult = await _SettingService.SaveCustomerStoreFrontAccess(customerStore);

                    long checkedFeaturesResult = await _SettingService.SaveAdminFeatureAccess(checkedFeatures, uncheckedFeatures);

                    return Json(new { success = true, message = "Your item is now LIVE in your community", data = storeresult });
                }
                else
                {
                    return Json(new { success = false, message = "Oops! something went wrong" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        //public async Task<ActionResult> UpdatePortalUser([FromBody] AddPermissionDTO obj)
        //{
        //    try
        //    {
        //        long Loggedinuser = _global.currentUser.Id;
        //        long Communityid = _global.currentUser.PrimaryCommunityId;

        //        obj.Communityid = Communityid;

        //        //var result = await _SettingService.SaveSaveNewPortalUserAsync(Loggedinuser, obj.customerFirstName, obj.customeremail, obj.customerLastName, obj.customermobile, obj.customerpassword, obj.CommunityRoleId, 0, Loggedinuser, obj.AccessNumber, Communityid);

        //        var result = await _SettingService.SaveSaveNewPortalUserAsync(obj);


        //        if (result != null)
        //        {
        //            foreach (var item in obj.CustomerStores)
        //            {
        //                item.CustomerId = obj.CustomerId;
        //            }
        //            foreach (var items in obj.adminFeatures)
        //            {
        //                items.CustomerId = obj.CustomerId;
        //                items.CommunityId = Communityid;
        //            }
        //            List<CustomerStoreFrontAccess> customerStore = _mapper.Map<List<CustomerStoreFrontAccess>>(obj.CustomerStores);
        //            List<AdminFeature> features = _mapper.Map<List<AdminFeature>>(obj.adminFeatures);
        //            long storeresult = await _SettingService.SaveCustomerStoreFrontAccess(customerStore);
        //            long featuresresults = await _SettingService.SaveAdminFeatureAccess(features);

        //            return Json(new { success = true, message = "Your item is now LIVE in your community", data = storeresult });
        //        }
        //        else
        //            return Json(new { success = false, message = "Oops! something went wrong" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}


        //Feature Access
        [ActionLog("Settings", "{0} fetched feature access.")]
		public async Task<IActionResult> FeatureAccess()
        {

            var lstfeature = await _SettingService.GetFeatureAccess();
            return Json(new { success = true, Data = lstfeature });
        }


        //Password Reset
        [HttpGet]
        public async Task<ActionResult> PasswordReset(string Email)
        {           
            var lstpassword = await _SettingService.GetPasswordReset(Email);
            return Json(new { success = true, Data = lstpassword });
        }

        public async Task<ActionResult> PasswordSetting()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(CustomerPasswordChangeRequest obj)
        {
            long Loggedinuser = _global.currentUser.Id;
            long Result = _SettingService.SaveResetPasswordAsync(Loggedinuser,(long)obj.PasswordActivationCode,(bool)obj.IsVerified);
            if (Result > 0)
                return Json(new { success = true, message = "Data Updated Successfully!" });
            else
                return Json(new { success = false, message = "Data Not Updated Successfully!" });
        }
        [HttpPost]
        [ActionLog("Setting", "{0} Add a houses.")]
        public async Task<ActionResult> SaveHouse([FromBody] HouseDTO obj)
        {
            obj.CommunityId = _global.currentUser.PrimaryCommunityId;
            obj.Code = _global.currentUser.PrimaryCommunityId.ToString();
            House houses = _mapper.Map<House>(obj);
            var result1 = await _SettingService.SaveHouse(houses);
            if (result1 > 0)
                return Json(new { success = true, message = "Houses saved successfully!" });
            else
                return Json(new { success = false, message = "Oops. There is some error while saving vehicle. Please try again!" });
        }

        [ActionLog("Setting", "{0} Get House Name.")]
        public async Task<IActionResult> GetHouse(HouseDTO obj)
        {

            var lstcommHouse = await _SettingService.GetHouse(obj.Id);
            return Json(new { success = true, Data = lstcommHouse });
        }

        [HttpPost]
        [ActionLog("Setting", "{0} delete Community House.")]
        public async Task<ActionResult> DeleteHouse(long Id)
        {
            var delete = await _SettingService.DeleteHouse(Id);
            if (delete > 0)
                return Json(new { success = true, message = "Deleted Successfully!" });
            else
                return Json(new { success = false, message = "Not Successfully!" });
        }

        [HttpPost]
        [ActionLog("Setting", "{0} updated Community House.")]
        public async Task<ActionResult> UpdateHouse(HouseDTO obj)
        {
            long result = _SettingService.UpdateHouse(obj.Id, obj.Name);
            if (result != null)
                return Json(new { success = true, message = "Data Updated Successfully!" });

            else
                return Json(new { success = false, message = "Data Not Updated Successfully!" });
        }


        public async Task<IActionResult> CancelCommunitySubscription()
        {
            try
            {
             
                StripeConfiguration.ApiKey = _config["SecretKey"];
                long CustomerId = _global.currentUser.Id;
                long CommunityId = _global.currentUser.PrimaryCommunityId;
                IEnumerable<SubscriptionDetails> details =  await _SettingService.GetStripeCustomerSubscriptionId(CustomerId, CommunityId);
                foreach (var subscription in details)
                {
                    try
                    {
                        //var service = new SubscriptionService();
                        //service.Cancel(subscription.StripeSubscriptionId);


                        //var options = new SubscriptionUpdateOptions
                        //{
                        //    PauseCollection = new SubscriptionPauseCollectionOptions { Behavior = "void" },
                        //};
                        //var service1 = new SubscriptionService();
                        //service1.Update(subscription.StripeSubscriptionId, options);


                        var options = new SubscriptionUpdateOptions
                        {
                            //PauseCollection = new SubscriptionPauseCollectionOptions { Behavior = "void" },
                            PauseCollection = new SubscriptionPauseCollectionOptions { Behavior = "mark_uncollectible", ResumesAt = DateTime.UtcNow}

                        };
                        var service1 = new SubscriptionService();
                        service1.Update(subscription.StripeSubscriptionId, options);



                    }
                    catch(Exception ex)
                    {

                    }
                    
                }
                 return Json(new { success = true, message = "Subscription canceled successfully." });
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        [HttpPost("/webhook")]

        public async Task<ActionResult> WebhookHandler()
        {
            try
            {
                var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], WEBHOOK_SECRET);
                if(stripeEvent.Type == Events.CustomerSubscriptionDeleted) 
                {
                
                }
                if(stripeEvent.Type == Events.InvoicePaymentFailed)
                {

                }
                else
                {
                
                }
               return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}


