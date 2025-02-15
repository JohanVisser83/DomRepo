using Circular.Core.Entity;
namespace Circular.Core.DTOs
{
    public class CustomersDTO : BaseEntityDTO
    {
        public CustomersDTO()
        {
            this.CustomerCommunities = new List<CustomerCommunityDTO?>();
            this.CustomerGroups = new List<CustomerGroupsDTO?>();
            this.BankAccounts = new List<CustomerBankAccountsDTO>();
        }
        public string? CountryCode { get; set; }
        public string Mobile { get; set; }
        public string? PrimaryEmail { get; set; }
        public string? Passcode { get; set; }
        public string? CustomerCode { get; set; }
        public bool? IsExternalSignUp { get; set; }
        public bool? IsBlocked { get; set; }
        public bool IsPasswordSet { get; set; } = true;
        public bool IsPasscodeSet { get; set; } = true;

		public string? SignUpStatus { get; set; }
        public long UnreadNotifications { get; set; } = 0;
        public decimal WalletBalance { get; set; } = 0.00m;

        public long UnreadMessages { get; set; } = 0;
        public QRDTO QRCode { get; set; }

        public string Environment { get; set; }


        public List<CustomerCommunityDTO?> CustomerCommunities { get; set; }
        public CustomerDetailsDTO? CustomerDetails { get; set; }
        public List<CustomerGroupsDTO?> CustomerGroups { get; set; }
        public List<CustomerBankAccountsDTO> BankAccounts { get; set; }
		public List<AppVersions> AppVersions { get; set; }

		public AdminConfigurationsDTO AdminConfigurations { get; set; }


        public void setSignUpStatus()
        {
            if (CustomerDetails?.UsertypeId is null)
            {
                SignUpStatus = Enum.GetName(typeof(SignUpStatusCode), 101);
            }
                
           else if (CustomerDetails?.FirstName is null && CustomerDetails?.LastName is null && CustomerDetails?.DOB is null && CustomerDetails?.Email is null)
            {
                SignUpStatus = Enum.GetName(typeof(SignUpStatusCode), 102);
            }

            else if (!IsPasswordSet)
            {
                SignUpStatus = Enum.GetName(typeof(SignUpStatusCode), 103);
            }
            else if (!IsPasscodeSet)
            {
                SignUpStatus = Enum.GetName(typeof(SignUpStatusCode), 104);
            }
            else
            {
                SignUpStatus = Enum.GetName(typeof(SignUpStatusCode), 105);
            }
            Passcode = "";
            return;
        }
    }
    public class CustomerRequestDTO
    {
        public long Id { get; set; }
        public string Mobile { get; set; }
        public int IdOrMobile { get; set; }
    }
    public class CustomerIdRequestDTO
    {
        public long CustomerId { get; set; }
    }
    public class CustomerResponse
    {
        public string AccessToken { get; set; }
        public CustomersDTO Customer { get; set; }

    }
}
