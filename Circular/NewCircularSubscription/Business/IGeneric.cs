using Circular.Core.DTOs;
using Circular.Core.Entity;
using Microsoft.AspNetCore.Mvc;

namespace NewCircularSubscription.Business
{
    public interface IGeneric
    {
        Task<APIResponse> SendOTP(string UserName, bool loginflow);
        Task<APIResponse> SendOTPOnMobile(string UserName, bool loginflow);
        Task<HttpResponseMessage> SaveOTPAsync(string userName, string otp, bool signupFlow);
        Task<HttpResponseMessage> RegisterAsync(string userName, string password);
        Task<APIResponse> GetTokenByOtpAsync(string userName, string otp, bool signupFlow, string CountryCode);
        Task<APIResponse> verifyOTP(string userName, string otp);
    }
}
