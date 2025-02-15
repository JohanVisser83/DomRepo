namespace Circular.Core.DTOs
{
    public class LoginDTO
    {
        public string username { get; set; }
        public string password { get; set; }
        public bool isEncrypted { get; set; } = false;
        public string TimeZone { get; set; } = "UTC";

    }

    public class LoginNameDTO
    {
        public string? UserName { get; set; }
        public bool SignupFlow { get; set; }
    }
    public class login_otpDTO
    {
        public string username { get; set; }
        public string password { get; set; }
        public bool SignUpFlow { get; set; }
        public string CountryCode { get; set; }
        public string TimeZone { get; set; } = "UTC";

    }

    public class RefreshTokenDTO
    {
        public string refreshToken { get; set; }
    }

    public class ChangePasswordDTO
    {
        public string newPassword { get; set; }
        public string confirmPassword { get; set; }
        public string AcessToken { get; set; }
    }

    public class OtpDTO
    {
        public string UserName { get; set; }

        public string otp { get; set; }
        public string TimeZone { get; set; } = "UTC";

    }


    public class CommunityOtpDTO
    {
        public string UserName { get; set; }

        public string Name { get; set; }    
        public long? CustomerId { get; set; }
        public string? otp { get; set; }

        public bool? loginflow { get; set; }

        public string? timezoneName { get; set; }   

        public long CustomerTypeId { get; set; }    
        

    }

    

    
   
    

}
