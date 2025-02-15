using Circular.Core.DTOs;

namespace Exceeder_xe_Community.Business
{
    public interface IGeneric
    {

        Task<APIResponse> SendOTPs(string UserName, bool loginflow);
        Task<HttpResponseMessage> SaveOTPAsync(string userName, string otp, bool signupFlow);
        Task<HttpResponseMessage> RegisterAsync(string userName, string password);
        Task<APIResponse> GetTokenByOtpAsync(string userName, string otp, bool signupFlow, string CountryCode);
       
    }
}
