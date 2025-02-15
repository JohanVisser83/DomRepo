using Circular.Core.Entity;
namespace Circular.Core.DTOs
{
    public class CustomerCommunityDTO
    {
        public CustomerCommunityDTO()
        {
            if (CommunitySettings == null)
                CommunitySettings = new List<SettingsDTO>();
            if (SponsorInformation == null)
                SponsorInformation = new SponsorInformationDTO();
            if (OtherDetails == null)
                OtherDetails = new CommunityDTO();
            if (ExternalLink == null)
                ExternalLink = new List<CommunityExternalLinks>();
            if (Paymentgateways == null)
                Paymentgateways = new List<Gateways>();

    }
        public long CustomerId { get; set; }
        public long? CommunityId { get; set; }
        public string? CommunityName { get; set; }
        public string? CommunityCode { get; set; } 
        public string? CommunityLogo { get; set; }
        public bool IsPrimary { get; set; }
        public string? Website { get; set; }
        public string? PrimaryEmail { get; set; }
        public string? PrimaryContact { get; set; }
        public string? PrimaryMobileNo { get; set; }
        public string? currencyCode { get; set; }
        public string? CompleteAddress { get; set; }
        public string? About { get; set; }
        public string? WorkingHours { get; set; }
        public string? Youtube { get; set; }
        public string? LinkedIn { get; set; }
      //  public string? Linkedin { get; set; }

        public string? TikTok { get; set; }
        public string? Instagram { get; set; }
        public string? Facebook { get; set; }
        public string? Twitter { get; set; }
        public string? coverimage { get; set; }
        public string? Title { get; set; }
        public string? TitleName { get; set; }
        public CommunityDTO OtherDetails;

        public string CurrencyToken { get; set; }
        public bool AttendanceScan { get; set; } = false;
        public bool DriverScan { get; set; } = false;
        public bool TicketScan { get; set; } = false;

        public bool IsPaymentRestricted { get; set; } = true;
        public string DashboardBanner { get; set; }
        public bool IsExternalLink { get; set; }
        public string? MemberCount { get; set; }
        public long? PostCount { get; set; }

        public string? SubscriptionType { get; set; }
        public string? AccessType { get; set; }
        public string? CoummunityUrl { get; set; }
        public decimal? Price { get; set; }

        public SponsorInformationDTO SponsorInformation { get; set; }
        public List<SettingsDTO> CommunitySettings { get; set; }

        public string PublicType { get; set; }
        public string JoinButton { get; set; }

        public string CommunityAddress { get; set; }


        public long Id { get; set; }

        public string OrgLogo { get; set; }


     
        public string? MembershipType { get; set; }
        public long MembershipTypeId { get; set; }
       

        public bool IsPeerTransferRestricted { get; set; } = true;
        public long OwnerCustomerId { get; set; }

        public List<CommunityExternalLinks> ExternalLink { get; set; }
        public List<Gateways> Paymentgateways { get; set; }

        public int IsMyCommunity { get; set; }
        public string PaymentGatewayReturnURL { get; set; } = "";

        

    }
    public class CustomerCommunityRequestDTO
    {
        public long CustomerId { get; set; }
        public long? CommunityId { get; set; }
        public string? CommunityName { get; set; }
        public string? CommunityCode { get; set; }
        public string? CommunityLogo { get; set; }
        public string? DashboardBanner { get; set; }
        public bool IsPrimary { get; set; }

    }
}
