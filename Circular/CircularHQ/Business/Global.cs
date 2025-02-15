using Circular.Core.Entity;
using Circular.Data.Repositories.User;
using System.Security.Claims;

namespace CircularHQ.Business
{
    public class Global : IGlobal
    {
        private IHttpContextAccessor httpContextAccessor;

        public CurrentUser currentUser { get; set; }

        public string UploadFolderPath { get; set; }
        public Global(IHttpContextAccessor _httpContextAccessor, IConfiguration _config, ICustomerRepository _customerRepository)
        {
            currentUser = new CurrentUser();
            currentUser.FirstName = _httpContextAccessor?.HttpContext?.User?
                .FindFirst(x => x.Type == System.Security.Claims.ClaimTypes.Name)?.Value ?? "";
            currentUser.LastName = _httpContextAccessor?.HttpContext?.User?
                .FindFirst(x => x.Type == System.Security.Claims.ClaimTypes.Surname)?.Value ?? "";
            currentUser.MobileNumber = _httpContextAccessor?.HttpContext?.User?
                .FindFirst(x => x.Type == System.Security.Claims.ClaimTypes.MobilePhone)?.Value ?? "";
            currentUser.PrimaryCommunityName = _httpContextAccessor?.HttpContext?.User?
                .FindFirst(x => x.Type == System.Security.Claims.ClaimTypes.Locality)?.Value ?? "";
            currentUser.Name = currentUser.FirstName + " " + currentUser.LastName;
            currentUser.usename = currentUser.MobileNumber;
            currentUser.CountryCode = _httpContextAccessor?.HttpContext?.User?
                .FindFirst(x => x.Type == System.Security.Claims.ClaimTypes.Country)?.Value ?? "";
            currentUser.CurrentTimeZone = _httpContextAccessor?.HttpContext?.User?
                .FindFirst(x => x.Type == System.Security.Claims.ClaimTypes.Thumbprint)?.Value ?? "";
            long Id = 0;
            long CommunityId = 0;
            long.TryParse(_httpContextAccessor?.HttpContext?.User?.FindFirst(x => x.Type == System.Security.Claims.ClaimTypes.PrimaryGroupSid)?.Value ?? "0", out CommunityId);
            long.TryParse(_httpContextAccessor?.HttpContext?.User?.FindFirst(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0", out Id);


            currentUser.Currency = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.OtherPhone).ToString();
            currentUser.PrimaryCommunityId = CommunityId;
            currentUser.Id = Id;
            currentUser.CustomerInfo = _customerRepository.getCommunityMemberDetailsByIdHQ(currentUser.Id);
            UploadFolderPath = "/" + _config["FileUpload:FileUploadPath"].ToString();
            currentUser.AccessToken = System.Security.Claims.ClaimTypes.SerialNumber.ToString();
        }

        public Global(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public CurrentUser GetCurrentUser()
        {
            return currentUser;
        }
    }

    public class CurrentUser
    {
        public CurrentUser()
        {
        }

        public long Id { get; set; } = 0;

        public string? usename { get; set; } = "";

        public string MobileNumber { get; set; } = "";

        public string Name { get; set; } = "";

        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";

        public string CountryCode { get; set; } = "";
        public string CurrentTimeZone { get; set; } = "";

        public string PrimaryCommunityName { get; set; } = "";
        public long PrimaryCommunityId { get; set; } = 0;

        public Customers CustomerInfo { get; set; }

        public string Currency { get; set; } = "";

        public string AccessToken { get; set; } = "";


    }
    public static class GetTimeZone
    {
        public static string GetTimeZoneDisplayName()
        {
            string standardName = "";
            TimeZoneInfo localTimeZone;
            localTimeZone = TimeZoneInfo.Local;
            standardName = localTimeZone.StandardName;
            return standardName;
        }
    }



   
}
