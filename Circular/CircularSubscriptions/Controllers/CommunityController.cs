using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Core.Mapper;
using Circular.Data.Repositories.User;
using Circular.Framework.Utility;
using Circular.Services.CreateCommunity;
using Circular.Services.User;
using CircularSubscriptions.Business;
using CircularSubscriptions.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using OpenIddict.Client;
using System.Net.Http.Headers;
using Tweetinvi.Core.Events;

namespace CircularSubscriptions.Controllers
{
   
    public class CommunityController : Controller
    {
        private readonly ICreateCommunityServices _CreateCommunityServices;

        private ICustomerService _customerService;
        private readonly IMapper _mapper;       
        private readonly IHelper _helper;
        private IServiceProvider _provider;
        private string OIDCUrl;
        public string UploadFolderPath { get; set; }        
        private readonly IConfiguration _config;
      
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IGeneric _generic;


        public CreateCommunityViewModel createCommunityViewModel = new CreateCommunityViewModel();
        public CommunityController(ICreateCommunityServices createCommunityServices, IMapper mapper, IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor, IHelper helper, ICustomerRepository customerRepository, IServiceProvider provider, IGeneric generic)
        {
            _generic = generic ?? throw new ArgumentNullException(nameof(generic));
            _CreateCommunityServices = createCommunityServices;
            _mapper = mapper;
            _config = configuration;
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));

            if (string.IsNullOrEmpty(configuration["OIDCUrl"]))
                throw new ArgumentNullException("configuration : OIDCUrl is not defined in App Setting");
            OIDCUrl = Convert.ToString(configuration["OIDCUrl"]);

            _httpContextAccessor = httpContextAccessor;
            UploadFolderPath = "/" + _config["FileUpload:FileUploadPath"].ToString();
            _provider = provider;
            _generic = generic;
        }

        [HttpGet]
        [Route("Community/")]
        public async Task<IActionResult> Index()
        {
            if (TempData["AuthCode"] == null || TempData["AuthCode"] == "")
                return RedirectToAction("EmailVerification", "EmailVerification");
            createCommunityViewModel.lstCountryName = await _CreateCommunityServices.GetCountryName();
            if (TempData["AffliatedCode"] != null && TempData["AffliatedCode"] != "")
                createCommunityViewModel.AId = TempData["AffliatedCode"].ToString();
            else
                createCommunityViewModel.AId = "";
            TempData["CustomerId"] = TempData["CustomerId"];
            TempData["AuthCode"] = TempData["AuthCode"];
            TempData["UserName"] = TempData["UserName"];
          
            return View("Community",createCommunityViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOTP(OtpDTO otpDTO)
        {
            try
            {
                TempData["TempCommunityId"] = TempData["TempCommunityId"];
                TempData["CustomerId"] = TempData["CustomerId"];
                var resp = await _generic.verifyOTP(otpDTO.UserName, otpDTO.otp);
                string data = resp.ToJson();
                APIResponse objResponse = JsonConvert.DeserializeObject<APIResponse>(data);
                if (objResponse.StatusCode == 2000)
                    return RedirectToAction("Index", "Features");
                else
                    return View("Community");
            }
            catch (Exception ex)
            {
                return View("Community");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CommunitySignUpDTO communitySignUpDTO)
        {
            communitySignUpDTO.Mobile = communitySignUpDTO.CountryCode + communitySignUpDTO.Mobile;
            communitySignUpDTO.Email = TempData["UserName"].ToString();
            if (communitySignUpDTO.Logo != null)
                communitySignUpDTO.CommunityLogo = _helper.SaveFile(communitySignUpDTO.Logo, UploadFolderPath, this.Request);
            CommunitySignUp communitySignUp = _mapper.Map<CommunitySignUp>(communitySignUpDTO);
            if(TempData["CustomerId"] != null)
                communitySignUp.CustomerId = long.Parse(TempData["CustomerId"].ToString());
            await _generic.SendOTPOnMobile(communitySignUp.Mobile, communitySignUp.Signupflow ?? true);
            var result = await _CreateCommunityServices.SaveCommunitySignUpDetails(communitySignUp);

            if (result>0)
            {
                TempData["TempCommunityId"] = result.ToString();
                TempData["CustomerId"] = communitySignUp.CustomerId.ToString();
                TempData["Mobile"] = communitySignUp.Mobile.ToString();
                TempData["UserName"] =  communitySignUp.Email.ToString();
                TempData["FirstName"] = communitySignUp.FirstName.ToString();
                TempData["LastName"] = communitySignUp.LastName.ToString();
                TempData["CommunityName"] = communitySignUp.CommunityName.ToString();
                ViewBag.UserName = communitySignUp.Mobile;
                return Json(new { success = true, message = "Community Details Saved Successfully" });
            }
            else
                return Json(new { success = false, message = "Something went wrong" });

        }
        public async Task<IActionResult> ResendOTP(bool? loginflow = null)
        {
            string userMobile = TempData["Mobile"].ToString();
            TempData["Mobile"] = userMobile;
            TempData["TempCommunityId"] = TempData["TempCommunityId"];
            TempData["CustomerId"] = TempData["CustomerId"];
            APIResponse objResponse = await _generic.SendOTP(userMobile, loginflow ?? true);
            if (objResponse.StatusCode == 200)
                return Json(new { success = true, message = "OTP Sent Successfully" });
            else
                return Json(new { success = true, message = "" });
        }

    }
}
