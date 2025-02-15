using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Framework.Utility;
using Circular.Services.User;
using Microsoft.AspNetCore.Mvc;
using NewCircularSubscription.Business;
using Newtonsoft.Json;
using NuGet.Protocol;

namespace NewCircularSubscription.Controllers
{
    public class CreateCommunityController : Controller
    {
        private readonly IHelper _helper;
        private ICustomerService _customerService;
        private readonly IMapper _mapper;
        private IServiceProvider _provider;
        private readonly IConfiguration _configuration;
        private string OIDCUrl;
        Customers customer;
        private IGeneric _generic;
        public CreateCommunityController(IMapper mapper, IServiceProvider provider
            , IConfiguration configuration, ICustomerService customerService, IHelper helper, IGeneric generic) 
        {
            _generic = generic ?? throw new ArgumentNullException(nameof(generic));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            if (string.IsNullOrEmpty(configuration["OIDCUrl"]))
                throw new ArgumentNullException("configuration : OIDCUrl is not defined in App Setting");
            OIDCUrl = Convert.ToString(configuration["OIDCUrl"]);
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(_configuration));

        }

        [Route("CreateCommunity/")]
        public async Task<IActionResult> Index() 
        {
            ViewBag.LearnMoreCircularURl = _configuration["LearnMoreCircularURl"];
            ViewBag.CommunityPortalURl = _configuration["CommunityPortalURL"];
            ViewBag.TermsOfUse = _configuration["TermsOfUse"];
            ViewBag.PrivacyPolicy = _configuration["PrivacyPolicy"];

            ViewBag.LinkedIn = _configuration["LinkedIn"];
            ViewBag.WhatsApp = _configuration["WhatsApp"];
            ViewBag.Contactus = _configuration["Contactus"];

            return View("CreateCommunity");
        }


        [HttpPost]
        public async Task<ActionResult<int>> EmailVerification(CommunityOtpDTO loginNameDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(loginNameDTO);
                APIResponse objResponse = await _generic.SendOTP(loginNameDTO.UserName, loginNameDTO.loginflow ?? true);
                if (objResponse.StatusCode == 200)
                {
                    TempData["Name"] = loginNameDTO.Name;
                    TempData["UserName"] = loginNameDTO.UserName;
                    TempData.Keep();
                    return Json(objResponse);
                }
                ViewBag.ErrorMessage = "Something went wrong. Please try again";
                ModelState.AddModelError("", "Invalid");
                return View("CreateCommunity");

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Something went wrong. Please try again";
                ModelState.AddModelError("", "Invalid");
                return View("CreateCommunity");
            }

        }


        [HttpPost]
      
        public async Task<IActionResult> VerifyOTP(CommunityOtpDTO objdata, string? returnUrl = null)
        {
            if (TempData["ReturnUrl"] != null && returnUrl == null)
                returnUrl = TempData["ReturnUrl"].ToString();
            string userEmail = TempData["UserName"].ToString();
            TempData["Name"] = TempData["Name"];
            TempData.Keep();
            var resp = await _generic.GetTokenByOtpAsync(userEmail, objdata.otp, true, "");
            string data = resp.ToJson();
            APIResponse objResponse = JsonConvert.DeserializeObject<APIResponse>(data);
            if (objResponse.StatusCode == 2000)
            {
                CustomerResponse customerResponse = JsonConvert.DeserializeObject<CustomerResponse>(objResponse.Data.ToJson());
                TempData["CustomerId"] = customerResponse.Customer.Id.ToString();
                TempData["AuthCode"] = customerResponse.AccessToken;
               
                return Json(new { success = true, redirectUrl = Url.Action("Features", "Feature"), data = objResponse });
            }
            else if (objResponse.StatusCode == 2998)
            {
                ViewBag.ErrorMessage = "You are already administrator of an existing community.";
                return Json(new { success = false, message = "You are already administrator of an existing community." });
            }
            else
                return Json(new { success = false, message = "Invalid OTP." });
        }
    }
}
