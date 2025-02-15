using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblCustomerCommunity")]
public class CustomerCommunity : BaseEntity
{
    public CustomerCommunity()
    {
        if (CommunitySettings == null)
            CommunitySettings = new List<Settings>();
        if (SponsorInformation == null)
            SponsorInformation = new SponsorInformation();
        //if (OtherDetails == null)
        //    OtherDetails = new Communities();
        if (Paymentgateways == null)
            Paymentgateways = new List<Gateways>();
        if (ExternalLink == null)
            ExternalLink = new List<CommunityExternalLinks>();
        
    }
    public long CustomerId { get; set; }
    public long? CommunityId { get; set; }
    public string? CommunityName { get; set; }
    public string? CommunityLogo { get; set; }
    public string? CommunityCode { get; set; }
    public bool? IsPrimary { get; set; }
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
   // public string? Linkedin { get { return LinkedIn; } }
    public string? TikTok { get; set; }
    public string? Instagram { get; set; }
    public string? Facebook { get; set; }
    public string? Twitter { get; set; }
    public string? coverimage { get; set; }
    public string? Title { get; set; }
    public string? TitleName { get; set; }
    public string? DashboardBanner { get; set; }
    public string? MembershipType { get; set; }
    public long MembershipTypeId { get; set; }
    public long SubscriptionTierId { get; set; }
    public long AccessTypeId { get; set; }
    public bool IsExternalLink { get; set; }
    public string? MemberCount { get; set; }
    public long? PostCount { get; set; }

    public string? SubscriptionType { get; set; }
    public string? AccessType { get; set; }
    public string? CoummunityUrl { get; set; }
    public decimal? Price { get; set; }


    //   public Communities OtherDetails { get; set; }

    public SponsorInformation SponsorInformation { get; set; }

    public List<Settings> CommunitySettings { get; set; }

    public List<Gateways> Paymentgateways { get; set; }

    public string? CurrencyToken { get; set; }
    public bool AttendanceScan { get; set; } = false;
    public bool DriverScan { get; set; } = false;
    public bool TicketScan { get; set; } = false;

    public bool IsAdmin { get; set; } = false;
    public bool IsPaymentRestricted { get; set; } = true;
    public bool IsPeerTransferRestricted { get; set; } = true;
    public long OwnerCustomerId { get; set; }

    public List<CommunityExternalLinks> ExternalLink { get; set; }

    public string PublicType { get; set; }
    public string JoinButton { get; set; }

    public string CommunityAddress { get; set; }


    public long Id { get; set; }

    public string OrgLogo { get; set; }
    public int IsMyCommunity { get; set; }
    public string DiscoveryBaseURL { get; set; }
    public string PaymentGatewayReturnURL { get; set; } = "";
    

    public override void ApplyKeys()
    {

    }
}
