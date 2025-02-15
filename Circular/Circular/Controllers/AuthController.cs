using AutoMapper;
using Circular.Core.Entity;
using Circular.Core.DTOs;
using Circular.Filters;
using Circular.Framework.Logger;
using Circular.Services.User;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Http.Headers;
using MailKit.Search;
using static System.Net.Mime.MediaTypeNames;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Circular.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private IServiceProvider _provider;
        private readonly IConfiguration _configuration;
        private readonly ICustomerService _customerService;
        string OIDCUrl;

        public AuthController(IMapper mapper, ILoggerManager logger, IServiceProvider provider, IConfiguration configuration,ICustomerService customerService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            if (string.IsNullOrEmpty(configuration["OIDCUrl"]))
            {
                throw new ArgumentNullException("configuration : OIDCUrl is not defined in App Setting");
            }
            OIDCUrl = Convert.ToString(configuration["OIDCUrl"]);
        }

        [HttpPost]
        [Route("register")]
        [SwaggerOperation(Summary = "Reviewed")]
        public async Task<ActionResult> register([FromBody] registerDTO registerDTO)
        {
            try
            {
                var resp = await RegisterAsync(registerDTO.username,registerDTO.password);
                APIResponse clsResponse = new APIResponse();
                clsResponse.StatusCode = (int)APIResponseCode.Success;
                clsResponse.Data = resp;
                return Ok(clsResponse);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("verifyuser")]
        
        [SwaggerOperation(Summary = "Reviewed")]
        public async Task<ActionResult> verifyUser([FromBody] verifyUserDTO verifyUserDTO)
        {
                return await VerifyUserAsync(verifyUserDTO.username);
        }
        [HttpPost]
        [Route("login/password")]
        [SwaggerOperation(Summary = "Reviewed")]
        public async Task<ActionResult<OpenIddictResponse>> login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var resp = await GetTokenAsync(loginDTO.username, loginDTO.password, loginDTO.isEncrypted, loginDTO.TimeZone);

                using var client = _provider.GetRequiredService<HttpClient>();
                using var request = new HttpRequestMessage(HttpMethod.Get, OIDCUrl + "api/Identity");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", resp.AccessToken);

                APIResponse objResponse = new APIResponse();

                using var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    OpenIDIdentityResponse obj = JsonConvert.DeserializeObject<OpenIDIdentityResponse>(json);
                    resp.UserCode = obj.value;
                    //Circular Specific logic

                    Customers customer = _customerService.getcustomerByUserId(new Guid(resp.UserCode), true);
                    if (customer == null || customer.IsBlocked == true)
                    {
                        objResponse.StatusCode = (int)APIResponseCode.Failure;
                        objResponse.Data = null;
                    }
                    else
                    {
                        customer.Passcode = "";
                        objResponse.StatusCode = (int)APIResponseCode.Success;
                        objResponse.Data = new
                        {
                            Authentication = resp,
                            customers = customer
                        };
                    }
                }
                return Ok(objResponse);
                
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
        }


        [HttpPost]
        [Route("login/otp")]
        [SwaggerOperation(Summary = "Reviewed")]
        public async Task<IActionResult> login_otp(login_otpDTO Login_OtpDTO)
        {
            try
            {
                var resp = await GetTokenByOtpAsync(Login_OtpDTO.username, Login_OtpDTO.password, Login_OtpDTO.SignUpFlow,Login_OtpDTO.CountryCode);
                return resp;
            }
            catch(Exception ex)
            {
                return Unauthorized();
            }
        }
        [HttpPost]
        [Route("logout")]
        [SwaggerOperation(Summary = "Reviewed")]
        public async Task<ActionResult<HttpResponseMessage>> logout([FromBody] LogOut token)
        {
            try
            {
                var resp = await logoutAsync(token.token);
                APIResponse clsResponse = new APIResponse();
                clsResponse.StatusCode = (int)APIResponseCode.Success;
                clsResponse.Data = resp;
                return Ok(clsResponse);
            }
            catch(Exception ex)
            {
                return Unauthorized();
            }
           
        }

        [HttpPost]
        [Route("refreshtoken")]
        [SwaggerOperation(Summary = "Reviewed")]
        public async Task<ActionResult<OpenIddictResponse>> refreshToken(RefreshTokenDTO refreshTokenDTO)
        {
            try
            {
                var resp = await GetTokenByRefreshTokenAsync(refreshTokenDTO.refreshToken);
                APIResponse objResponse = new APIResponse();
                objResponse.StatusCode = (int)APIResponseCode.Success;
                objResponse.Data = resp;
                return Ok(objResponse);
            }
            catch(Exception ex)
            {
                return Unauthorized();
            }
        }


        [HttpPost]
        [Route("getuserid")]
        [SwaggerOperation(Summary = "Reviewed")]
        public async Task<ActionResult<HttpResponseMessage>> getUserId([FromBody] LogOut token)
        {
            try
            {
                var resp = await GetUserIdAsync(token.token);
                APIResponse clsResponse = new APIResponse();
                clsResponse.StatusCode = (int)APIResponseCode.Success;
                clsResponse.Data = resp;
                return Ok(clsResponse);
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }

        }

        [HttpPost]
        [Route("change-password")]
        [AuthorizeOIDC]
        [SwaggerOperation(Summary = "Reviewed")]
		[ActionLog("Master", "{UserName} Requested change Password")]
		public async Task<ActionResult<HttpResponseMessage>> changePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            return await changeCustomerPassword( changePasswordDTO.newPassword,  changePasswordDTO.confirmPassword, changePasswordDTO.AcessToken);
        }

        [HttpPost]
        [Route("VerifyOTP")]
        [SwaggerOperation(Summary = "Reviewed")]
        public async Task<IActionResult> VerifyOTP([FromBody] OtpDTO otpDTO)
        {
            try
            {
                var resp = await verifyOTP(otpDTO.UserName,otpDTO.otp);
                return resp;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPost]
        [Route("VerifyPassword")]
        [SwaggerOperation(Summary = "Reviewed")]
        public async Task<ActionResult<OpenIddictResponse>> VerifyPassword([FromBody] LoginDTO loginDTO)
        {
            APIResponse objResponse = new APIResponse();
            try
            {
                var resp = await GetTokenAsync(loginDTO.username, loginDTO.password, loginDTO.isEncrypted);
                if (resp != null && resp.AccessToken != "") 
                    objResponse.StatusCode = (int)APIResponseCode.Success;
            }
            catch (Exception ex)
            {
                objResponse.StatusCode = (int)APIResponseCode.Password_Does_Not_Match;
            }
            return Ok(objResponse);
        }




        #region "Private Functions"


        [NonAction]
        async Task<ActionResult> VerifyUserAsync(string userName)
        {
            var OIDCClient = _provider.GetRequiredService<OpenIddictClientService>();
            var clientDetails = await OIDCClient.GetClientRegistrationAsync(new Uri(OIDCUrl));
            if (clientDetails != null)
            {
                using var client = _provider.GetRequiredService<HttpClient>();
                using var request = new HttpRequestMessage(HttpMethod.Post, OIDCUrl + "connect/verifyuser");
                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>{
                    {"client_id", clientDetails.ClientId},
                    {"client_secret", clientDetails.ClientSecret},
                    {"username", userName}
                });

                var response = await client.SendAsync(request);
                var result = "";
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    APIResponse objResponse = JsonConvert.DeserializeObject<APIResponse>(result);
                    if (objResponse != null)
                    {
                        //Circular specific logic
                        if(objResponse.StatusCode == (int)APIResponseCode.Record_Found) {
                            string Usercode = ((JObject)objResponse.Data)["id"].ToString();
                            Customers customer = _customerService.getcustomerByUserId(new Guid(Usercode),true);
                            if (customer != null)
                            {
                                CustomersDTO customersDTO = _mapper.Map<CustomersDTO>(customer);
                                customersDTO.setSignUpStatus();
                                objResponse.Data = customersDTO;
                            }
                            else
                            {
                                objResponse.StatusCode = (int)APIResponseCode.No_Record_Found;
                                objResponse.Data = null;

							}
						}
                        //
                        return Ok(objResponse);
                    }
                    return BadRequest();
                }
                return BadRequest();
            }
            else
            {
                return Unauthorized();
            }
        }

        [NonAction]
        async Task<OpenIddictResponse> GetTokenAsync(string userName, string password, bool isEncrypted, string TimeZone = "UTC")
        {
            if(isEncrypted)          
                password = password.ToLower();
           
            var service = _provider.GetRequiredService<OpenIddictClientService>();
            Dictionary<string, OpenIddictParameter>? parameters = new Dictionary<string, OpenIddictParameter>();
            parameters.Add("is_encrypted", new OpenIddictParameter(isEncrypted));
            parameters.Add("timezone", new OpenIddictParameter(TimeZone));

            var (response, _) = await service.AuthenticateWithPasswordAsync(new Uri(OIDCUrl), userName, password, parameters: parameters);
            return response; 
        }

        [NonAction]
        async Task<ActionResult> GetTokenByOtpAsync(string userName, string otp, bool signupFlow, string CountryCode, string TimeZone = "UTC")
        {
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
                    {"password", otp },
                    {"timezone", TimeZone }
            });
                using var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    APIResponse objResponse = new APIResponse();
                    objResponse.StatusCode = (int)APIResponseCode.Success;
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
                        objResponse.Data = obj;
                        // Circular specific logic
                        
                        Customers customer = await SaveCustomer(userName,new Guid (obj.User_Code),CountryCode);
                        CustomersDTO customersDTO = _mapper.Map<CustomersDTO>(customer);

                        customersDTO.setSignUpStatus();

                        objResponse.Data = new
                        {
                            Authentication = obj,
                            customer = customersDTO
                        };
                    }
                    return Ok(objResponse);
                }
                else
                {
                    return Unauthorized();
                }                
            }
            else
            {
                return Unauthorized();
            }

        }

        [NonAction]
        async Task<OpenIddictResponse> GetTokenByRefreshTokenAsync(string refreshToken)
        {
            var service = _provider.GetRequiredService<OpenIddictClientService>();
            var (response, _) = await service.AuthenticateWithRefreshTokenAsync(new Uri(OIDCUrl), refreshToken);
            return response;
        }

        [NonAction]
        async Task<ActionResult> logoutAsync(string token)
        {
            using var client = _provider.GetRequiredService<HttpClient>();
            using var request = new HttpRequestMessage(HttpMethod.Post, OIDCUrl + "connect/logout");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                APIResponse objResponse = new APIResponse();
                objResponse.StatusCode = (int)APIResponseCode.Success;
                OpenIDLoginResponse obj = JsonConvert.DeserializeObject<OpenIDLoginResponse>(json);
                objResponse.Data = obj;
                return Ok(objResponse);
            }
            return Unauthorized();
        }

        [NonAction]
        async Task<ActionResult> RegisterAsync(string userName, string password)
        {
            var OIDCClient = _provider.GetRequiredService<OpenIddictClientService>();
            var clientDetails = await OIDCClient.GetClientRegistrationAsync(new Uri(OIDCUrl));
            if (!string.IsNullOrEmpty(clientDetails.ClientId) && !string.IsNullOrEmpty(clientDetails.ClientSecret))
            {
                using var client = _provider.GetRequiredService<HttpClient>();
                using var request = new HttpRequestMessage(HttpMethod.Post, OIDCUrl + "connect/create-user");
                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>{
                    {"client_id", clientDetails.ClientId},
                    {"client_secret", clientDetails.ClientSecret },
                    { "username", userName },
                    { "password", password }
                });
                using var response = await client.SendAsync(request);
                var result = "";
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    return Ok(result);
                }
                return BadRequest();
            }
            else
                return Unauthorized();

        }


        [NonAction]
        async Task<ActionResult> GetUserIdAsync(string token)
        {
            using var clientIdentityResponse = _provider.GetRequiredService<HttpClient>();
            using var requestIdentityResponse = new HttpRequestMessage(HttpMethod.Get, OIDCUrl + "api/Identity");
            requestIdentityResponse.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var identityResponse = await clientIdentityResponse.SendAsync(requestIdentityResponse);
            if (identityResponse.IsSuccessStatusCode)
            {
                string identityjson = await identityResponse.Content.ReadAsStringAsync();
                OpenIDIdentityResponse objIdentityResponse = JsonConvert.DeserializeObject<OpenIDIdentityResponse>(identityjson);
                APIResponse objResponse = new APIResponse();
                objResponse.StatusCode = (int)APIResponseCode.Success;
                objResponse.Data = new { User_code= objIdentityResponse .value};
                return Ok(objResponse);
            }
            return Unauthorized();

        }

        [NonAction]
        async Task<ActionResult> changeCustomerPassword(string newPassword, string confirmPassword, string AcessToken)
        {
            try
            {
                var User_code = "";
                using var clientIdentityResponse = _provider.GetRequiredService<HttpClient>();
                using var requestIdentityResponse = new HttpRequestMessage(HttpMethod.Get, OIDCUrl + "api/Identity");
                requestIdentityResponse.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AcessToken);
                using var identityResponse = await clientIdentityResponse.SendAsync(requestIdentityResponse);
                if (identityResponse.IsSuccessStatusCode)
                {
                    string identityjson = await identityResponse.Content.ReadAsStringAsync();
                    OpenIDIdentityResponse objIdentityResponse = JsonConvert.DeserializeObject<OpenIDIdentityResponse>(identityjson);
                    User_code = objIdentityResponse.value;
                }

                using var clientRequest = _provider.GetRequiredService<HttpClient>();
                using var request = new HttpRequestMessage(HttpMethod.Post, OIDCUrl + "api/Identity/changePassword");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AcessToken);
                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>{
                        {"newPassword", newPassword},
                        {"confirmPassword", confirmPassword},
                        {"User_code", User_code}
                    });

                using var response = await clientRequest.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    APIResponse oResponse = JsonConvert.DeserializeObject<APIResponse>(json);
                    return Ok(oResponse);
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


       

        [NonAction]
       public async Task<ActionResult> SaveOTPAsync(string userName, string otp, bool signupFlow)
        {
            if(signupFlow && _configuration["RegisterIfNotExist"] == "1")
                 await RegisterAsync(userName, "circularpassword".ToLower().Reverse().ToString());

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

        private async Task<Customers> SaveCustomer(string username, Guid usercode,string CountryCode)
        {
            Customers customer;
            customer = new Customers();
            customer = _customerService.getcustomerByUserId(usercode,true);
            if (customer is null || customer.Id <= 0)
            {
                bool IsEmail = username.Contains("@");
                
                customer = new Customers() {CountryCode = CountryCode,Mobile = IsEmail==true?"":username,
                                           PrimaryEmail=IsEmail==true?username:"",UserId = (Guid)usercode};
                var id = await _customerService.Save(customer,true);
                customer = _customerService.UpdateUserType(id,104,id);
                customer.Id = id;
            }
            return customer;
        }


        [NonAction]
        async Task<ActionResult> verifyOTP(string userName, string otp,string TimeZone = "UTC")
        {
            try
            {
                var OIDCClient = _provider.GetRequiredService<OpenIddictClientService>();
                var clientDetails = await OIDCClient.GetClientRegistrationAsync(new Uri(OIDCUrl));
                if (!string.IsNullOrEmpty(clientDetails.ClientId) && !string.IsNullOrEmpty(clientDetails.ClientSecret))
                {
                    using var client = _provider.GetRequiredService<HttpClient>();
                    using var request = new HttpRequestMessage(HttpMethod.Post, OIDCUrl + "connect/verify-otp");
                    request.Content = new FormUrlEncodedContent(new Dictionary<string, string>{
                    {"client_id", clientDetails.ClientId},
                    {"client_secret", clientDetails.ClientSecret },
                    {"grant_type","password" },
                    { "username", userName },
                    { "password", otp },
                    { "timezone", TimeZone }
                });
                    using var response = await client.SendAsync(request);
                    string result = "";
                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadAsStringAsync();
                        OpenIDLoginResponse objResponse = JsonConvert.DeserializeObject<OpenIDLoginResponse>(result);
                        APIResponse apiResponse = new APIResponse();
                        apiResponse.StatusCode = (int)APIResponseCode.Success;
                        apiResponse.Data = new { access_token= objResponse.access_token };
                        return Ok(apiResponse);
                    }
                    return Unauthorized();
                }
                else
                    return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }


}
