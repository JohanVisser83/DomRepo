using Microsoft.AspNetCore.Http;

namespace Circular.Core.DTOs
{
    public class CommunityDTO : BaseEntityDTO
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
		public string? AccessCode { get; set; }
		public string? AttendanceScannerImage { get; set; }
		public string? Youtube { get; set; }
		public string? LinkedIn { get; set; }
		public string? TikTok { get; set; }
		public string? Instagram { get; set; }
		public string? Facebook { get; set; }
		public string? Twitter { get; set; }
		public long? OwnerCustomerId { get; set; }
		public long Reportcount { get; set; }
        public string? CurrencyToken { get; set; }
        public IFormFile? DasboardBannerImg { get; set; }
        public string? DashboardBanner { get; set; }
        public bool AttendanceScan { get; set; } = false;
        public bool DriverScan { get; set; } = false;
        public bool TicketScan { get; set; } = false;
        public bool IsExternalLink { get; set; }
		public string? PaymentGatewayReturnURL { get; set; } = "";

		public string AffiliateCode { get; set; }

        public string completeAddress
        {
            get; set;
        }
    }
}

public class CommunityAccessDTO
{
	public long  CommunityId { get; set; }	
	public string Fullname { get; set; }

	public string Mobile { get; set; }

    public string? AccessCode { get; set; }

	public long CustomerId { get; set; }
	
	public string Email { get; set; }

	public long StatusId { get; set; }	

	public string Price { get; set; }	

	public string CountryCode { get; set; }	


}

public class JoinCommunityDTO
{
    
    public long CommunityId { get; set; }
    public long CustomerId { get; set; }

	public long StatusId { get; set; }	
	public string AccessType { get; set; }
   
}


public class CommunityUserDTO
{
	public string Mobile { get; set; }
	public string CountryCode { get; set; }
    public string Otp { get; set; }
    public long MembershipType { get; set; }
    public long CustomerId { get; set; }
    public long AccessType { get; set; }

    public decimal? MembershipAmount { get; set; }


}
