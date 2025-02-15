using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Data.Repositories.User;
using Circular.Framework.Utility;
using Circular.Services.Community;
using Circular.Services.Finance;
using Circular.Services.Message;
using Circular.Services.User;
using CircularWeb.Business;
using CircularWeb.filters;
using CircularWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nancy.Diagnostics.Modules;
using Newtonsoft.Json;
using NuGet.Protocol;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace CircularWeb.Controllers
{
    [Authorize]
    public class FinanceController : Controller
    {
        private readonly IFinanceService _FinanceService;
        private readonly ICommunityService _communityService;
        private string OIDCUrl;
        private IServiceProvider _provider;
        Customers customer;
        private readonly ICustomerService _customerService;
        private readonly IMessageService _MessageService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CommonModel commonModel = new CommonModel();
        public FinanceModel financeModel = new FinanceModel();
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGlobal _global;
        private readonly IHelper _helper;

        public FinanceController(IFinanceService FinanceService, IMapper mapper, IWebHostEnvironment webHostEnvironment, IServiceProvider provider, ICommunityService communityService
            , IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMessageService messageService, ICustomerService customerService, ICustomerRepository customerRepository, IHelper helper)
        {
            _FinanceService = FinanceService;
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            if (string.IsNullOrEmpty(configuration["OIDCUrl"]))
                throw new ArgumentNullException("configuration : OIDCUrl is not defined in App Setting");
            OIDCUrl = Convert.ToString(configuration["OIDCUrl"]);
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _MessageService = messageService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _config = configuration;
            _httpContextAccessor = httpContextAccessor;
            _global = new Global(_httpContextAccessor, _config, customerRepository);
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _communityService = communityService ?? throw new ArgumentNullException(nameof(communityService));
        }

        [ActionLog("Finance", "{0} opened finance.")]
        public async Task<IActionResult> Finance()
        {
            CurrentUser currentUser = _global.GetCurrentUser();
            long Loggedinuser = currentUser.Id;
            long primaryId = currentUser.PrimaryCommunityId;
            financeModel.CommunityLogo = currentUser.CustomerInfo.PrimaryCommunity.CommunityLogo;
            financeModel.CommunityFeatures = _communityService.Features(currentUser.PrimaryCommunityId,Loggedinuser).Result.ToList();
            financeModel.IsOwner = currentUser.CustomerInfo.PrimaryCommunity.OwnerCustomerId == Loggedinuser ? true : false;

            financeModel.lstbankDetails = await _FinanceService.GetBankDetailsAsync(0,primaryId);
            financeModel.Ewallet = await _FinanceService.WalletBalance(currentUser.PrimaryCommunityId);
            financeModel.lstCountry = await _FinanceService.GetMasterCountryAsync();
            financeModel.currencyModel.CurrencyCode = currentUser.Currency;
            List<Settings> cslist = currentUser.CustomerInfo.PrimaryCommunity.CommunitySettings;
            financeModel.lstsettings = cslist.Where(s => s.Key == "Withdrawl_Fee").FirstOrDefault();
            financeModel.SubscriptionStatus = currentUser.SubscriptionStatus;
            if (!financeModel.IsOwner)
                throw new ArgumentNullException("Unauthroized : You dont have permission to access this functionality.");
            else
               
            return View(financeModel);
        }
        [ActionLog("Finance", "{0} fetched withdraw feature.")]
        public async Task<IActionResult> GetFeaturesName()
        {
            var result = await _FinanceService.GetWithdrawalFeature();
            if (result != null && result.Count() > 0)
                return Json(new { success = true, message = "Your item is now LIVE in your community", data = result });
            else
                return Json(new { success = false, message = "Data Not Saved Successfully!" });
        }
        [ActionLog("Finance", "{0} fetched transactions history.")]
        public async Task<IActionResult> BindTransactionHistory(DateTime StartDate, DateTime EndDate, long Count)
        {
            //  long CustomerId = 32508;  //_httpContextAccessor.HttpContext.Items(Claim.)
            long communityId = _global.GetCurrentUser().PrimaryCommunityId;
            var result = await _FinanceService.GetCommunityTransactions(StartDate, EndDate, Count, communityId);
            if (result != null)
                return Json(new { success = true, message = "Your item is now LIVE in your community", data = result });
            else
                return Json(new { success = false, message = "Data Not Saved Successfully!" });
        }


        [HttpPost]
        [ActionLog("Finance", "{0} saved bank account.")]
        public async Task<ActionResult<int>> AddBankAccount(CustomerBankAccountsDTO customerBankAccountsDTO)
        {
            try
            {
                customerBankAccountsDTO.CommunityId = _global.GetCurrentUser().PrimaryCommunityId;
                //  customerBankAccountsDTO.CustomerId = Convert.ToInt64(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier));
                CustomerBankAccounts messagelist = _mapper.Map<CustomerBankAccounts>(customerBankAccountsDTO);
                var result = await _FinanceService.SaveAsync(messagelist);
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
        [ActionLog("Finance", "{0} fetched banks.")]
        public async Task<ActionResult> GetMasterBank(string Code)
        {
            try
            {
                var result = await _FinanceService.GetMasterBank(Code);
                if (result != null && result.Count() > 0)
                    return Json(new { success = true, message = "Your item is now LIVE in your community", data = result });
                else
                    return Json(new { success = false, message = "Data Not Saved Successfully!" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Oops! something went wrong" });
            }
        }

        [ActionLog("Finance", "{0} fetched contact list.")]
        public async Task<ActionResult> GetUserContactList(string Search)
        {
            long communityId = _global.GetCurrentUser().PrimaryCommunityId;
            var userContactList = await _MessageService.GetUserContactListAsync(communityId, Search);
            var userContact = (userContactList?.Select(u => _mapper.Map<UserContactList>(u)).ToList());
            return Json(userContact);
        }

        [ActionLog("Finance", "{0} pay by mobile.")]
        public async Task<ActionResult<int>> PayByMobile(TransactionRequestDTO transactionsDTO)
        {
            try
            {
                transactionsDTO.CommunityId = _global.GetCurrentUser().PrimaryCommunityId;
                transactionsDTO.TransactionTypeId = (long)TransactionTypeEnum.Transfer;
                transactionsDTO.Currency = _global.GetCurrentUser().Currency;
                TransactionRequest transaction = _mapper.Map<TransactionRequest>(transactionsDTO);
                Customers customers = _customerService.getcustomerbyId(_global.GetCurrentUser().Id, true);
                transaction.TransactionTypeId = (long)TransactionTypeEnum.Transfer;
                transaction.TransactionStatusId = (long)TransactionStatusEnum.Success;
                transaction.Currency = _global.GetCurrentUser().Currency;
                var result = await _FinanceService.NewPayment(transaction, customers);
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

        [ActionLog("Finance", "{0} reqested for payment.")]
        public async Task<ActionResult<int>> RequestPayment(TransactionRequestDTO transactionsDTO)
        {
            try
            {
                transactionsDTO.TransactionFrom = _global.GetCurrentUser().Id;
                transactionsDTO.CommunityId = _global.GetCurrentUser().PrimaryCommunityId;
                transactionsDTO.TransactionTypeId = (long)TransactionTypeEnum.Requested;
                transactionsDTO.Currency = _global.GetCurrentUser().Currency;
                TransactionRequest transaction = _mapper.Map<TransactionRequest>(transactionsDTO);
                Customers customers = _customerService.getcustomerbyId(_global.GetCurrentUser().Id, true);

                transaction.TransactionStatusId = (long)TransactionStatusEnum.Success;
                transaction.Currency = _global.GetCurrentUser().Currency;


                var result = await _FinanceService.NewPayment(transaction, customers);
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

        [ActionLog("Finance", "{0} saved process withdrawal.")]
        public async Task<ActionResult<int>> Withdrawal(CustomerWithdrawalRequestDTO customerWithdrawalRequestDTO)
        {
            try
            {
                CurrentUser cu = _global.GetCurrentUser();
                List<Settings> cslist = cu.CustomerInfo.PrimaryCommunity.CommunitySettings;
                Settings cs = cslist.Where(s => s.Key == "Withdrawl_Fee").FirstOrDefault();
                decimal withdrawalpercentage = decimal.Parse(cs.Value??"0");
                decimal? withdrawalFixedCharge = cs.FixedValue;
                customerWithdrawalRequestDTO.CommunityId = cu.PrimaryCommunityId;
                customerWithdrawalRequestDTO.CustomerId = cu.Id;
                customerWithdrawalRequestDTO.WithDrawalRequestStatusId = (long)WithdrawalStatusEnum.Pending;
                CustomerWithdrawalRequest transactions = _mapper.Map<CustomerWithdrawalRequest>(customerWithdrawalRequestDTO);
                transactions.currency = cu.Currency;

                transactions.ServiceFee = (transactions.Amount * withdrawalpercentage) / 100;
                transactions.ServiceFee = transactions.ServiceFee + withdrawalFixedCharge;

                var result = await _FinanceService.MakeWithdrawal(transactions);
                if (result > 0)
                    return Json(new { success = true, message = "You have successfully posted a withdrawal" });
                else
                    return Json(new { success = false, message = "Data Not Saved Successfully!" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Oops! something went wrong" });
            }
        }

        [ActionLog("Finance", "{0} fetched events name.")]
        public async Task<ActionResult> GetEventName(string EventName,long CommunityId)
        {
            try
            {
                CommunityId = _global.GetCurrentUser().PrimaryCommunityId;
                var result = await _FinanceService.GetEventName(EventName, CommunityId);
                if (result != null && result.Count() > 0)
                    return Json(new { success = true, message = "Your item is now LIVE in your community", data = result });
                else
                    return Json(new { success = false, message = "Data Not Saved Successfully!" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Oops! something went wrong" });
            }
        }

        [ActionLog("Finance", "{0} fetched available balance.")]
        public async Task<ActionResult> GetAvailabelBalance(string EventName, long Id)
        {
            try
            {
                var result = await _FinanceService.GetAvailabelBalance(EventName, Id);
                if (result != null && result.Count() > 0)
                    return Json(new { success = true, message = "Your item is now LIVE in your community", data = result });
                else
                    return Json(new { success = false, message = "Data Not Saved Successfully!" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Oops! something went wrong" });
            }
        }

        [ActionLog("Finance", "{0} saved process a refund.")]
        public async Task<ActionResult<long>> ProcessRefund(TransactionRequestDTO transactionsDTO)
        {
            try
            {

                transactionsDTO.CommunityId = _global.GetCurrentUser().PrimaryCommunityId;
                string UserId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                transactionsDTO.TransactionTypeId = (long)TransactionTypeEnum.Refund;
                transactionsDTO.Currency = _global.GetCurrentUser().Currency;
                transactionsDTO.TransactionFrom = Convert.ToInt64(UserId);
                TransactionRequest transaction = _mapper.Map<TransactionRequest>(transactionsDTO);
                var result = await _FinanceService.RefundAsync(transaction);
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


        public async Task<ActionResult> VerifyPassword(string? otp, bool isLoginFlow = false)
        {
            try
            {
                var UserName = _global.GetCurrentUser().MobileNumber;
                var Isloginotp = await GetTokenByOtpAsync(UserName, otp, true, "");
                if (Isloginotp)
                    return Json(new { success = true, message = "OTP Verified Successfully" });
                else
                    return Json(new { success = false, message = "Invaild OTP" });


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Invaild OTP" });
            }
        }
        async Task<bool> GetTokenByOtpAsync(string userName, string otp, bool signupFlow, string CountryCode)
        {
            bool objResponse = false;
            var OIDCClient = _provider.GetRequiredService<OpenIddictClientService>();
            var clientDetails = await OIDCClient.GetClientRegistrationAsync(new Uri(OIDCUrl));
            if (clientDetails != null)
            {
                var client = _provider.GetRequiredService<HttpClient>();
                using var request = new HttpRequestMessage(HttpMethod.Post, OIDCUrl + "connect/otp");
                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>{
                    {"client_id", clientDetails.ClientId},
                    {"client_secret", clientDetails.ClientSecret},
                    {"grant_type","password" },
                    {"username", userName },
                    {"password", otp }
                });
                using var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    OpenIDLoginResponse obj = JsonConvert.DeserializeObject<OpenIDLoginResponse>(json);

                    using var clientIdentityResponse = _provider.GetRequiredService<HttpClient>();
                    using var requestIdentityResponse = new HttpRequestMessage(HttpMethod.Get, OIDCUrl + "api/Identity");
                    requestIdentityResponse.Headers.Authorization = new AuthenticationHeaderValue("Bearer", obj.access_token);
                    using var identityResponse = await clientIdentityResponse.SendAsync(requestIdentityResponse);
                    if (identityResponse.IsSuccessStatusCode)
                    {
                        string identityjson = await identityResponse.Content.ReadAsStringAsync();
                        OpenIDIdentityResponse objIdentityResponse = JsonConvert.DeserializeObject<OpenIDIdentityResponse>(identityjson);
                        obj.User_Code = objIdentityResponse.value;
                        // Circular specific logic
                        customer = await GetCustomer(userName, new Guid(obj.User_Code), CountryCode);
                        objResponse = true;
                    }
                    return objResponse;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        private async Task<Customers> GetCustomer(string username, Guid usercode, string CountryCode)
        {
            return _customerService.getcustomerByUserId(usercode, true);
        }

        [NonAction]
        async Task<OpenIddictResponse> GetTokenAsync(string userName, string password, bool isEncrypted)
        {
            if (isEncrypted)
                password = password.ToLower();

            var service = _provider.GetRequiredService<OpenIddictClientService>();
            Dictionary<string, OpenIddictParameter>? parameters = new Dictionary<string, OpenIddictParameter>();
            parameters.Add("is_encrypted", new OpenIddictParameter(isEncrypted));
            var (response, _) = await service.AuthenticateWithPasswordAsync(new Uri(OIDCUrl), userName, password, parameters: parameters);
            return response;

        }

        [ActionLog("Finance", "{0} fetched refund details.")]
        public async Task<ActionResult> GetRefundDetails(long CommunityId)
        {
            CommunityId = _global.GetCurrentUser().PrimaryCommunityId;
            var result = await _FinanceService.GetRefundDetails(CommunityId);
            if (result != null && result.Count() > 0)
                return Json(new { success = true, message = "Data fetched successfully!", data = result });
            else
                return Json(new { success = false, message = "No Data Found!", data = "" });
        }

        [ActionLog("Finance", "{0} fetched pending withdrawal.")]
        public async Task<ActionResult> PendingWithdrawal()
        {
            try
            {
                long CommunityId = Convert.ToInt64(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.PrimaryGroupSid));
                long customerId = Convert.ToInt64(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _FinanceService.PendingWithdrawal(CommunityId, customerId);
                if (result != null)
                    return Json(new { success = true, message = "Your item is now LIVE in your community", data = result });
                else
                    return Json(new { success = false, message = "Data Not Saved Successfully!" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Oops! something went wrong" });
            }
        }

        [ActionLog("Finance", "{0} fetched bank details.")]
        public async Task<ActionResult> ViewBankDetails(long Id)
        {
            try
            {
                long CommunityId = Convert.ToInt64(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.PrimaryGroupSid));

                var result = await _FinanceService.GetBankDetailsAsync(Id,CommunityId);
                if (result != null && result.Count() > 0)
                    return Json(new { success = true, message = "Your item is now LIVE in your community", data = result });
                else
                    return Json(new { success = false, message = "Data Not Saved Successfully!" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Oops! something went wrong" });
            }
        }
        [ActionLog("Finance", "{0} fetched selected pending withdrawal.")]
        public async Task<ActionResult> pendingWithdrawalView(long Id)
        {
            try
            {
                var result = await _FinanceService.PendingWithdrawalView((int)TransactionTypeEnum.Withdrawal, Id);
                if (result != null)
                    return Json(new { success = true, message = "Your item is now LIVE in your community", data = result });
                else
                    return Json(new { success = false, message = "Data Not Saved Successfully!" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Oops! something went wrong" });
            }
        }

        [ActionLog("Finance", "{0} paided withdrawal.")]

        public async Task<ActionResult> PaidWithdrawal(long transactionStatusId, long transactionTypeId)
        {
            try
            {
                //    long CommunityId = Convert.ToInt64(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.PrimaryGroupSid));
                //    long customerId = Convert.ToInt64(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _FinanceService.PaidWithdrawal(transactionStatusId, transactionTypeId);
                if (result != null)
                    return Json(new { success = true, message = "Your item is now LIVE in your community", data = result });
                else
                    return Json(new { success = false, message = "Data Not Saved Successfully!" });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Oops! something went wrong" });
            }
        }

        public async Task<ActionResult> GetWithdrawalBank(long BankId)
        {
            CurrentUser cu = _global.GetCurrentUser();
            long communityId = cu.PrimaryCommunityId;
            var result = await _FinanceService.BankListWithdrawal(BankId, communityId);
            if (result.Count() > 0 && result != null)
                return Json(new { success = true, message = "You have successfully posted a withdrawal", data = result });
            else
                return Json(new { success = false, message = "withdrawal Not posted Successfully!" });
        }
        public async Task<ActionResult> SendOtpForWithdrawal()
        {
            var lstOTP = _helper.GenerateRandomNumber(int.Parse(_config["OTP:Length"]), _config["OTP:MasterOTP"],
               bool.Parse(_config["OTP:IsMasterOTPEnabled"]), bool.Parse(_config["OTP:IsAlphaNumeric"]));
            var userName = _global.GetCurrentUser().MobileNumber;
            var response = await SaveOTPAsync(userName, lstOTP.ToString(), true);
            string data = response.ToJson();
            APIResponse objResponse = JsonConvert.DeserializeObject<APIResponse>(data);
            if (objResponse.StatusCode == 200)
            {
                var sentWithdrawalOTP = await _customerService.SendWithdrawalOTPMail(userName, lstOTP.ToString());
                if (sentWithdrawalOTP == "True")
                    return Json(new { success = true, message = "Otp sent Successfully" });
                else
                    return Json(new { success = false, message = "Something went wrong" });
            }
            else
            {
                return Json(new { success = false, message = "Something went wrong" });
            }
        }
        [NonAction]
        public async Task<ActionResult> SaveOTPAsync(string userName, string otp, bool signupFlow)
        {
            var OIDCClient = _provider.GetRequiredService<OpenIddictClientService>();
            var clientDetails = await OIDCClient.GetClientRegistrationAsync(new Uri(OIDCUrl));
            if (clientDetails != null)
            {
                using var client = _provider.GetRequiredService<HttpClient>();
                using var request = new HttpRequestMessage(HttpMethod.Post, OIDCUrl + "connect/save-otp");
                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>{
                    {"client_id", clientDetails.ClientId},
                    {"client_secret", clientDetails.ClientSecret},
                    {"username", userName},
                    {"otp", otp}
                });

                var response = await client.SendAsync(request);
                var result = "";
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    return Ok(result);
                }
                return BadRequest();
            }
            else
            {
                return Unauthorized();
            }
        }
        public async Task<IActionResult> DeleteBankAccount(long Id)
        {
            var result = await _FinanceService.DeleteBankAccount(Id);
            if (result > 0)
                return Json(new { success = true, message = "Bank deleted successfully" });
            else
                return Json(new { success = false, message = "Item Not Deleted Successfully" });
        }
        public async Task<ActionResult> GetUserBankListAsync(search search)
        {
            // long communityId = _global.GetCurrentUser().PrimaryCommunityId;
            var userbankList = await _FinanceService.GetUserBankListAsync("", search.Search);
            //  var result = (userbankList?.Select(u => _mapper.Map<Banks>(u)).ToList());
            return Json(userbankList);
        }

        public async Task<ActionResult> WalletBalance(long communityId)
        {
            var result = await _FinanceService.WalletBalance(communityId);
            if (result != null)
                return Json(new { success = true, message = "", data = result });
            else
                return Json(new { success = false, message = "" });
        }
    }
}
