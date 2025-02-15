using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Framework.ShortMessages;
using Circular.Framework.Utility;

using Circular.Services.User;
using Newtonsoft.Json;
using OpenIddict.Client;
using System.Net.Http.Headers;
using static System.Net.WebRequestMethods;

namespace Exceeder_xe_Community.Business
{
    public class Generic : IGeneric
    {
        private readonly IBulkSMS _sms;
        private readonly IHelper _helper;
        private ICustomerService _customerService;
        private readonly IMapper _mapper;
        private IServiceProvider _provider;
        private readonly IConfiguration _configuration;
        private string OIDCUrl;
        Customers customer;

        public Generic(IMapper mapper, IHttpContextAccessor _httpContextAccessor, IConfiguration configuration, IBulkSMS bulkSMS,
            ICustomerService customerService,IHelper helper, IServiceProvider provider)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _sms = bulkSMS ?? throw new ArgumentNullException(nameof(bulkSMS));

            if (string.IsNullOrEmpty(configuration["OIDCUrl"]))
                throw new ArgumentNullException("configuration : OIDCUrl is not defined in App Setting");
            OIDCUrl = Convert.ToString(configuration["OIDCUrl"]);
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(_configuration));
        }


        public async Task<APIResponse> SendOTPs(string UserName, bool loginflow)
        {
            var lstOTP = _helper.GenerateRandomNumber(int.Parse(_configuration["OTP:Length"]), _configuration["OTP:MasterOTP"],
                bool.Parse(_configuration["OTP:IsMasterOTPEnabled"]), bool.Parse(_configuration["OTP:IsAlphaNumeric"]));
            var response = await SaveOTPAsync(UserName, lstOTP.ToString(), loginflow);

            ShortMessageDetails sms = new ShortMessageDetails() { Mobile_number = UserName, Message = _configuration["OTP:Message"].Replace("<otp>", lstOTP) };
            ShortMessageProviderDetails shortMessageProvider = new ShortMessageProviderDetails() { UserName = _configuration["BulkSMS:username"], Password = _configuration["BulkSMS:password"] };
            string data = JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            APIResponse objResponse = JsonConvert.DeserializeObject<APIResponse>(data);
            if (objResponse.StatusCode == 200)
            {
                if (!bool.Parse(_configuration["OTP:IsMasterOTPEnabled"]))
                    _sms.BulkSms(sms, shortMessageProvider);
            }
            
            return objResponse;
        }


        public async Task<HttpResponseMessage> SaveOTPAsync(string userName, string otp, bool signupFlow)
        {
            if (signupFlow && _configuration["RegisterIfNotExist"] == "1")
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
                return response;
            }
            else
            {
                return (new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized));
            }
        }

        public async Task<HttpResponseMessage> RegisterAsync(string userName, string password)
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
                return response;
            }
            else
                return (new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized));
        }


        public async Task<APIResponse> GetTokenByOtpAsync(string userName, string otp, bool signupFlow, string CountryCode)
        {
            APIResponse objResponse = new APIResponse();
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

                        Customers customer = await SaveCustomer(userName, new Guid(obj.User_Code), CountryCode);
                        if (customer != null)
                        {
                            CustomersDTO customersDTO = _mapper.Map<CustomersDTO>(customer);
                            objResponse.Data = new CustomerResponse()
                            {
                                AccessToken = obj.access_token,
                                Customer = customersDTO
                            };
                        }
                        else
                        {
                            objResponse.Data = null;
                            objResponse.StatusCode = (int)APIResponseCode.Existing_Owner;
                            return objResponse;
                        }
                    }
                    return objResponse;
                }
                else
                {
                    objResponse.StatusCode = (int)APIResponseCode.Failure;
                    return objResponse;
                }
            }
            else
            {
                objResponse.StatusCode = (int)APIResponseCode.Failure;
                return objResponse;
            }

        }


        private async Task<Customers> SaveCustomer(string username, Guid usercode, string CountryCode)
        {
            Customers customer;
            customer = new Customers();
            customer = _customerService.getcustomerByUserId(usercode, true);
            if (customer is null || customer.Id <= 0)
            {
                bool IsEmail = username.Contains("@");

                customer = new Customers()
                {
                    CountryCode = CountryCode,
                    Mobile = IsEmail == true ? "" : username,
                    PrimaryEmail = IsEmail == true ? username : "",
                    UserId = (Guid)usercode
                };
                var id = await _customerService.Save(customer, true);
                customer.Id = id;
                return customer;
            }
            else
            {
                if (_customerService.CheckIfExistingOwnerId(customer.Id))
                    return null;
                else
                    return customer;
            }

        }
    }
}
