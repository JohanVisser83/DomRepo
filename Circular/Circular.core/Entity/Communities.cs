using Microsoft.AspNetCore.Http;
using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("mtblCommunity")]
public class Communities : BaseEntity
{

	public string? OrgName { get; set; }
	public string? About { get; set; }
    public IFormFile? OrgLogoImg { get; set; }
    public string? OrgLogo { get; set; }
    public string? OrgLogoName { get; set; }
    public string? OrgAddress1 { get; set; }
    public string? OrgAddress2 { get; set; }
    public string? City { get; set; }
    public string? Zip { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }
	public long? CountryId { get; set; }
	public string? currencyCode { get; set; }
	public string? Website { get; set; }
	public string? WorkingHours { get; set; }
	public string? TitleLabel { get; set; }
	public string? TitleHolder { get; set; }
    public IFormFile? OrgCoverImage { get; set; }
    public string? coverimage { get; set; }
    public string? OrgCoverImageName { get; set; }
    public int? CompanySize { get; set; }
	public bool? IsLive { get; set; }
    public string? CoummunityUrl { get; set; }

    public string? PrimaryContact { get; set; }
	public string? PrimaryEmail { get; set; }
    public string? PrimaryMobileNo { get; set; }
    public string? SecContact { get; set; }
    public string? SecEmail { get; set; }
	public string? SecondaryMobileNo { get; set; }
	public string? Landline1 { get; set; }
    public string? Landline2 { get; set; }
    public string? BillingContactName { get; set; }
    public string? MainAccountNumber { get; set; }
    public string? AccountMobileNo { get; set; }
    public string? CouncillorName { get; set; }
    public string? CouncillorEmail { get; set; }
	public string RecipientNameReport { get; set; }
	public string EmailAddressReport { get; set; }
	public string PhoneNumberReport { get; set; }
    public string? SickNoteRecipientName { get; set; }
	public string? SickNoteEmail { get; set; }
	public string SickNotePhoneNumber { get; set; }
	public string? SickNoteMailBox { get; set; }
	public decimal? PricePerKm { get; set; }
	public decimal? Price { get; set; }

    public string? MembershipType { get; set; }

    public string? AccessCode { get; set; }
    public string? AttendanceScannerImage { get; set; }
    public string? Youtube { get; set; }
    public string? LinkedIn { get; set; }
    public string? TikTok { get; set; }
    public string? Instagram { get; set; }
    public string? Facebook { get; set; }
    public string? Twitter { get; set; }
	public long? OwnerCustomerId { get; set; }
    public string? CurrencyToken { get; set; }
    public long Reportcount { get; set; }

	public bool IsPaymentRestricted { get; set; } = false;
    public bool IsPeerTransferRestricted { get; set; } = true;
    public bool IsExternalLink { get; set; }

    public bool IsDeletedByAdmin { get; set; }

    public string? OrganizerName { get; set; }
    public string? DashboardBanner { get; set; }
    public IFormFile? DasboardBannerImg { get; set; }

    public long Reportcounter { get; set; }

    public int IsMyCommunity { get; set; }

    public string completeAddress { 
        get    
        {
            string CA = "";
            if (!string.IsNullOrWhiteSpace(OrgAddress1))
                CA = CA + OrgAddress1;
            if (!string.IsNullOrWhiteSpace(OrgAddress2))
                CA = CA +","+ OrgAddress2;
            if (!string.IsNullOrWhiteSpace(City))
                CA = CA + "," + City;
            if (!string.IsNullOrWhiteSpace(Zip))
                CA = CA + "," + Zip;
            if (!string.IsNullOrWhiteSpace(Province))
                CA = CA + "," + Province;
            if (!string.IsNullOrWhiteSpace(Country))
                CA = CA + "," + Country;
            return CA;
        }
    }


   

    public bool AttendanceScan { get; set; } = false;
    public bool DriverScan { get; set; } = false;
    public bool TicketScan { get; set; } = false;

    public override void ApplyKeys()
    {

    }
 
}


public class CommunityDetails 
{
    public long Id { get; set; }    
    public string CommunityName { get; set; }
    public string About { get; set; }

    public string OrgLogo { get; set; }

    public string MembershipType { get; set;}

    public string AccessType { get; set; }

    public string Membercount { get; set; }

    public decimal? Price { get; set; }

    public long PostCount { get; set; } 

    public string CommunityAddress { get; set; }    

    public string Website { get; set; }

    public string coverimage { get; set; }  

    public string PublicType { get; set; }
    public string JoinButton { get; set; }
    public string? CurrencyToken { get; set; }

    public int IsMyCommunity { get; set; }
    public int IsPrimary { get; set; }

    public string? PaymentGatewayReturnURL { get; set; } = "";

    public string? CoummunityUrl { get; set; }

    public string currencyCode { get; set; }
}


public class HQCommunitiesList
{
    public long Id { get; set; }
    public string AccountMobileNo { get; set; }
    public string PrimaryMobileNo { get; set; }
    public string AccessCode { get; set; }
    public string PrimaryContact { get; set; }
    public string OrgName { get; set; }
    public long memberCount { get; set; }
    public string CommunityStatus { get; set; }
}