using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Circular.Core.Entity;
using Circular.Services.Account;
using CircularWeb.Models;
using Circular.Core.DTOs;
using Circular.Framework.Utility;
using AutoMapper;
using CircularWeb.Business;
using Circular.Services.Community;
using Circular.Services.User;
using Circular.Core.Mapper;
using Circular.Services.Message;
using Circular.Data.Repositories.User;
using CircularWeb.filters;
using System;
using Org.BouncyCastle.Asn1.Cms;

namespace CircularWeb.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        private readonly IAccountServices _accountServices;
        private readonly ICommunityService _CommunityService;
        private readonly ICustomerService _CustomerServives;
        private readonly IMessageService _MessageService;
        private readonly IMapper _Mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        public CommonModel commonModel = new CommonModel();
        public AccountModel accountModel = new AccountModel();
        private readonly IConfiguration _config;
        private readonly IHelper _helper;
        private readonly IGlobal _global;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CurrentUser currentUser;
        private string OIDCUrl;

        public AccountController(IAccountServices accountServices,IMapper mapper,IWebHostEnvironment webHostEnvironment,IConfiguration configuration, 
            ICommunityService communityService,IHelper helper, IHttpContextAccessor httpContextAccessor, IMessageService messageService, ICustomerRepository customerRepository)
        {
            _accountServices = accountServices;
            _CommunityService = communityService ?? throw new ArgumentNullException(nameof(communityService));
            _Mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _config = configuration;
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _MessageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            _global = new Global(_httpContextAccessor, _config,customerRepository);
            if (string.IsNullOrEmpty(configuration["OIDCUrl"]))
                throw new ArgumentNullException("configuration : OIDCUrl is not defined in App Setting");
            OIDCUrl = Convert.ToString(configuration["OIDCUrl"]);

        }

        [ActionLog("Account", "{0} opened account.")]
        public async Task<IActionResult> Account()
        {
            CurrentUser currentUser = _global.GetCurrentUser();
            long Loggedinuser = currentUser.Id;
            accountModel.CommunityLogo = currentUser.CustomerInfo.PrimaryCommunity.CommunityLogo;
            accountModel.CommunityFeatures = _CommunityService.Features(currentUser.PrimaryCommunityId,Loggedinuser).Result.ToList();
            //
            accountModel.IsOwner = currentUser.CustomerInfo.PrimaryCommunity.OwnerCustomerId == Loggedinuser ? true : false;

            long currentUserId = currentUser.Id;
            accountModel.Organizers = await _CommunityService.GetCommunityOrganizers(currentUser.PrimaryCommunityId);
            accountModel.Groups = await _CommunityService.GetCommunityGroups(currentUser.PrimaryCommunityId);
            accountModel.ActiveAccount = await _accountServices.GetActiveAccount(currentUser.PrimaryCommunityId);
            accountModel.ClosedAccount = await _accountServices.GetClosedAccount(currentUser.PrimaryCommunityId);
            accountModel.currencyModel.CurrencyCode = currentUser.Currency;
            accountModel.SubscriptionStatus = currentUser.SubscriptionStatus;
            if (!accountModel.IsFeatureAvailable("AC-006"))
                throw new ArgumentNullException("Unauthroized : You dont have permission to access this functionality.");
            else
            return View(accountModel);
        }

        //Delete Active Account
        [ActionLog("Account", "{0} deleted active account.")]
        public async Task<IActionResult> DeleteActiveAccountItems(long Id)
        {
            var result = await _accountServices.DeleteActiveAccountitem(Id);

            if (result > 0)
                return Json(new { success = true, message = "Item Deleted Successfully" });
            else
                return Json(new { success = false, message = "Oops. Something went wrong while deleting account. Please try again" });
        }
        [ActionLog("Account", "{0} closed accounts.")]
        public async Task<IActionResult> ClosedAccount(long Id)
        {
            var result = await _accountServices.ClosedAccount(Id);

            if (result > 0)
                return Json(new { success = true, message = "Account closed successfully" });
            else
                return Json(new { success = false, message = "Oops..! Something went wrong" });
        }
        [ActionLog("Account", "{0} active account details.")]
        public async Task<ActionResult> ActiveAccountCollectionData(long CollectionId)
        {

            var activeCollectionData = _accountServices.GetCollectionData(CollectionId);

            if (activeCollectionData != null)
                return Json(new { success = true, message = "Data fetched successfully!", data = activeCollectionData });
            else
                return Json(new { success = false, message = "No Data Found!", data = "" });
        }
        [ActionLog("Account", "{0} closed account details.")]
        public async Task<ActionResult> ClosedAccountCollectionData(long CollectionId)
        {
            var closedCollectionData = _accountServices.GetCollectionData(CollectionId);
            if (closedCollectionData != null)
            {
                return Json(new { success = true, message = "Data fetched successfully!", data = closedCollectionData });
            }
            else
                return Json(new { success = false, message = "No Data Found!", data = "" });
        }
        [ActionLog("Account", "{0} fetched user contact details.")]
        public async Task<ActionResult> GetUserContactList(string Search)
        {
            long communityId = _global.GetCurrentUser().PrimaryCommunityId;
            var userContactList = await _MessageService.GetUserContactListAsync(communityId, Search);
            var result = (userContactList?.Select(u => _Mapper.Map<UserContactList>(u)).ToList());
            //return Json({ userContact, data = userContact});
            return Json(result);
        }
        [ActionLog("Account", "{0} fetched account details.")]
        public async Task<IActionResult> GetViewAccountDetails(long Id)
        {
            var result = await _accountServices.GetViewAccountDetails(Id);
            if (result != null &&  result.Count() > 0)
                return Json(new { success = true, message = "Data fetched successfully!", data = result });
            else
                return Json(new { success = false, message = "No Data Found!", data = "" });
        }

        [HttpPost]
        //Add a New Account 
        [ActionLog("Account", "{0} saved account.")]
        public async Task<IActionResult> SaveAccount(CollectionAggregate data)
        {
            try
            {
                 
                var communityId = _global.currentUser.PrimaryCommunityId;
                var communityName =  _global.currentUser.PrimaryCommunityName;
                var UserId = _global.currentUser.Id;
                if (data.Mediafile != null)
                    data.AccountMedia = _helper.SaveFile(data.Mediafile, _global.UploadFolderPath, this.Request);

                  if (data.Type == "All")
                  {
                      data.GroupId = 0;
                      data.Individual = 0;
                    
                  }
                   else if (data.Type == "Individual")
                    {
                        data.GroupId = 0;
                       
                    }
                    else
                    {
                        data.Individual= 0;
                    }

                if (data.Scheduledeliverydate == null || data.Scheduledeliverydate.ToString("yyyy-MM-dd") == "0001-01-01")
                 {
                    data.Scheduledeliverydate = DateTime.Now.Date;
                    data.Scheduleddeliverytime = DateTime.Now.TimeOfDay;
                }
                if (data.Scheduleddeliverytime == null)
                {
                    data.Scheduleddeliverytime = DateTime.Now.TimeOfDay;
                }

                data.CustomerId = UserId;
                data.CommunityId = communityId;
                data.IsClosed = false;
                data.CommunityName = communityName;
               

                //DateTime dtExpire = new DateTime(data.EventEndDate.Year, data.EventEndDate.Month, data.EventEndDate.Day, data.EventEndTime.Hours, data.EventEndTime.Minutes, 0);
                if (data.EventEndDate < data.ExpirydateCollection)
                {
                    return Json(new { success = false, message = "Expiry date & time can not be after Event end date and time!" });
                }
                
                  
                   CollectionAggregate account = _Mapper.Map<CollectionAggregate>(data);
                   var result = await _accountServices.AddAccountDetails(account);
               

                if (result > 0)
                    return Json(new { success = true, message = "Account added successfully" });
                else
                    return Json(new { success = false, message = "Account not added successfully!" });


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Oops Something Wents Wrong!" }); ;
            }
        }

    }

   
}

