using Circular.Core.Entity;
using Circular.Core.DTOs;
using Circular.Services.User;
using Circular.Framework.Utility;
using OpenIddict.Client;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using NuGet.Protocol;
using CircularSubscriptions.Business;

namespace CircularSubscriptions.Controllers
{
    public class EmailVerificationController : Controller
    {

        private readonly IHelper _helper;
        private ICustomerService _customerService;
        private readonly IMapper _mapper;
        private IServiceProvider _provider;
        private readonly IConfiguration _configuration;
        private string OIDCUrl;
        Customers customer;
        private IGeneric _generic;

        public EmailVerificationController(IMapper mapper, IServiceProvider provider
            , IConfiguration configuration, ICustomerService customerService, IHelper helper, IGeneric generic
            )
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

        public IActionResult EmailVerification(string? returnUrl = null)
        {
            if (!String.IsNullOrEmpty(HttpContext.Request.Query["AId"]))
                TempData["AffliatedCode"] = HttpContext.Request.Query["AId"].ToString().Substring(0,6);
             return View();
        }

        [HttpPost]
        public async Task<ActionResult<int>> EmailVerification(CommunityOtpDTO loginNameDTO)
        {
            try
            {
                //if (!ModelState.IsValid)
                //    return View(loginNameDTO);
                APIResponse objResponse = await _generic.SendOTP(loginNameDTO.UserName, loginNameDTO.loginflow ?? true);
                if (objResponse.StatusCode == 200)
                {
                    TempData["UserName"] = loginNameDTO.UserName;
                    TempData.Keep();
                    return Json(objResponse);
                }
                ViewBag.ErrorMessage = "Something went wrong. Please try again";
                ModelState.AddModelError("", "Invalid");
                return View("EmailVerification");
               
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Something went wrong. Please try again";
                ModelState.AddModelError("", "Invalid");
                return View("EmailVerification");
            }

        }

        [HttpPost]
        [Route("Verify/")]
        public async Task<IActionResult> login_otp(CommunityOtpDTO objdata, string? returnUrl = null)
        {
            if (TempData["ReturnUrl"] != null && returnUrl == null)
                returnUrl = TempData["ReturnUrl"].ToString(); 
            string userEmail = TempData["UserName"].ToString();
           
            TempData.Keep();
            var resp =  await _generic.GetTokenByOtpAsync(userEmail, objdata.otp, true, "");
            string data = resp.ToJson();
            APIResponse objResponse = JsonConvert.DeserializeObject<APIResponse>(data);
            if (objResponse.StatusCode == 2000)
            {
                CustomerResponse customerResponse = JsonConvert.DeserializeObject<CustomerResponse>(objResponse.Data.ToJson());
                TempData["CustomerId"] = customerResponse.Customer.Id.ToString();
                TempData["AuthCode"] = customerResponse.AccessToken;
                return RedirectToAction("Index", "Community");
            }
            else if(objResponse.StatusCode == 2998)
            {
                
                    ViewBag.ErrorMessage = "You are already administrator of an existing community.";
                    ModelState.AddModelError("", "Invalid");
                    return View("EmailVerification");
               
            }
            else 
            {
                ViewBag.ErrorMessage = "Something went wrong. Please try again";
                ModelState.AddModelError("", "Invalid");
                return View("EmailVerification");
            }
                
        }

        public async Task<IActionResult> ResendOTP(bool? loginflow = null)
        {
            string userEmail = TempData["UserName"].ToString();
            TempData["UserName"] = userEmail;
            if (TempData["ReturnUrl"] != null)
            {
                string returnUrl = TempData["ReturnUrl"].ToString();
                TempData["ReturnUrl"] = returnUrl;
            }
            APIResponse objResponse = await _generic.SendOTP(userEmail,loginflow ?? true);
            if (objResponse.StatusCode == 200)
                return Json(new { success = true, message = "OTP Sent Successfully" });
            else
                return Json(new { success = false, message = "" });
        }

    }
}
