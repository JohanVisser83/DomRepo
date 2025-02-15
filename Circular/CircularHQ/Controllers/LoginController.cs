using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Framework.Utility;
using Circular.Services.Master;
using Circular.Services.User;
using CircularHQ.filters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using OpenIddict.Client;
using System.Net.Http.Headers;
using System.Security.Claims;
using TimeZoneConverter;

namespace CircularHQ.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHelper _helper;
        private readonly IMapper _mapper;
        private IServiceProvider _provider;
        private readonly IMasterService _masterService;
        private readonly IConfiguration _configuration;
        private string OIDCUrl;
        Customers customer;

        //private readonly IGlobal _global;
        private readonly ICustomerService _customerService;

        public LoginController(IMapper mapper, IServiceProvider provider, IConfiguration configuration, ICustomerService customerService, IMasterService masterService,
            IHelper helper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            if (string.IsNullOrEmpty(configuration["OIDCUrl"]))
                throw new ArgumentNullException("configuration : OIDCUrl is not defined in App Setting");
            OIDCUrl = Convert.ToString(configuration["OIDCUrl"]);
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(_configuration));
            _masterService = masterService ?? throw new ArgumentNullException(nameof(masterService));
        }

        [ActionLog("Login", "{0} opened login.")]
        public IActionResult Login(string? returnUrl = null)
        {
            TempData["ReturnUrl"] = returnUrl;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [ActionLog("Login", "{0} fetched otp.")]
        public IActionResult Otp(string? returnUrl = null)
        {
            return View();

        }


        // CircularHQ login using OTP
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionLog("Login", "{0} logged in using otp.")]
        public async Task<ActionResult<int>> Login(CommunityOtpDTO loginDTO, string? returnUrl = null)
        {
            TempData["ReturnUrl"] = returnUrl;
            CustomerDetails userName = await _customerService.CheckValidHQAdminEmail(loginDTO.UserName);
            if (userName.CustomerTypeId == 101)
            {
                //Generate OTP
                var lstOTP = _helper.GenerateRandomNumber(int.Parse(_configuration["OTP:Length"]), _configuration["OTP:MasterOTP"],
                bool.Parse(_configuration["OTP:IsMasterOTPEnabled"]), bool.Parse(_configuration["OTP:IsAlphaNumeric"]));
                var response = await SaveOTPAsync(loginDTO.UserName, lstOTP.ToString(), loginDTO.loginflow ?? true);
                string data = response.ToJson();
                APIResponse objResponse = JsonConvert.DeserializeObject<APIResponse>(data);

                if (objResponse.StatusCode == 200)
                {
                    var sentOTPMail = await _customerService.SendOTPMail(loginDTO.UserName, lstOTP.ToString());
                    TempData["UserName"] = loginDTO.UserName;
                    return RedirectToAction("Otp", "Login");
                }

                ViewBag.ErrorMessage = "Invalid Email. Please try again";
                ModelState.AddModelError("UserName", "Invalid");
                return View("Login");

            }
            else
            {
                ViewBag.ErrorMessage = "Invalid Email. Please try again";
                ModelState.AddModelError("UserName", "Invalid");
                return View("Login");
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



        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionLog("Login", "{0} logged in successfully.")]
        public async Task<IActionResult> otp(CommunityOtpDTO otpDTO, string? returnUrl = null)
        {
            if (TempData["ReturnUrl"] != null && returnUrl == null)
                returnUrl = TempData["ReturnUrl"].ToString();
            try
            {

                if (!ModelState.IsValid)
                {
                    if (ModelState.ContainsKey("otp"))
                        ModelState["otp"].Errors.Clear();
                    if (ModelState.ContainsKey("UserName"))
                        ModelState["UserName"].Errors.Clear();
                }
                //    return View(otpDTO);
                var Isloginotp = await GetTokenByOtpAsync(otpDTO.UserName, otpDTO.otp, true, "");
                if (Isloginotp)
                {
                    var identity = new ClaimsIdentity("Cookies");
                    var community = customer.CustomerCommunities.Find(cc => cc.IsPrimary == true);
                    identity.AddClaim(new Claim(ClaimTypes.MobilePhone, otpDTO.UserName ?? ""));
                    identity.AddClaim(new Claim(ClaimTypes.Locality, community.CommunityName ?? ""));

                    identity.AddClaim(new Claim(ClaimTypes.Name, customer.CustomerDetails.FirstName ?? ""));
                    identity.AddClaim(new Claim(ClaimTypes.Surname, customer.CustomerDetails.LastName ?? ""));

                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.Country, customer.CountryCode.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.PrimaryGroupSid, community.CommunityId.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.OtherPhone, community.currencyCode.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.SerialNumber, TempData["AccessToken"].ToString()));
                    string windowsTimeZone = TZConvert.IanaToWindows(otpDTO.timezoneName);
                    identity.AddClaim(new Claim(ClaimTypes.Thumbprint, windowsTimeZone));
                    var claimsPrincipal = new ClaimsPrincipal(identity);
                    // Set current principal
                    Thread.CurrentPrincipal = claimsPrincipal;


                    await HttpContext.SignInAsync("Cookies",
                        claimsPrincipal);

                    var save = await _masterService.SaveTimeZone(customer.Id, windowsTimeZone);

                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    TempData["UserName"] = otpDTO.UserName;
                    TempData["ReturnUrl"] = returnUrl;
                    ViewBag.ErrorMessage = "Invalid OTP. Please try again.";
                    ModelState.AddModelError("otp", "Invalid OTP");
                    return View("Otp");
                }
            }
            catch (Exception ex)
            {
                TempData["UserName"] = otpDTO.UserName;
                TempData["ReturnUrl"] = returnUrl;
                ViewBag.ErrorMessage = "Oops.Something went wrong";
                ModelState.AddModelError("otp", "Invalid OTP");
                return View("Otp");
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
                    TempData["AccessToken"] = obj.access_token;
                    using var clientIdentityResponse = _provider.GetRequiredService<HttpClient>();
                    using var requestIdentityResponse = new HttpRequestMessage(HttpMethod.Get, OIDCUrl + "api/Identity");
                    requestIdentityResponse.Headers.Authorization = new AuthenticationHeaderValue("Bearer", obj.access_token);
                    using var identityResponse = await clientIdentityResponse.SendAsync(requestIdentityResponse);
                    if (identityResponse.IsSuccessStatusCode)
                    {
                        string identityjson = await identityResponse.Content.ReadAsStringAsync();
                        OpenIDIdentityResponse objIdentityResponse = JsonConvert.DeserializeObject<OpenIDIdentityResponse>(identityjson);
                        obj.User_Code = objIdentityResponse.value;
                        // CircularHQ specific logic
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










        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction(nameof(DashboardController.Dashboard), "Dashboard");
        }


        [HttpGet]
        [ActionLog("Login", "{0} logged out.")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync("Cookies");
            return RedirectToAction(nameof(LoginController.Login), "Login");
        }

    }
}
