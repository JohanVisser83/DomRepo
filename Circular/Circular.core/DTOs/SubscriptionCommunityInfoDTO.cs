

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Circular.Core.DTOs
{
    public class SubscriptionCommunityInfoDTO
    {
        public string CommunityName { get; set; }
        public IFormFile? CommunityLogoImg { get; set; }
        public string? CommunityLogo { get; set; }
       
        public string? CommunityBanner { get; set; }
        public IFormFile? CommunityBannerImg { get; set; }

       
        public string About { get; set; }
        public string WebsiteUrl { get; set; }
        public string PhysicalAddress { get; set; }
        public string CommunityContactNo { get; set; }
        public string Country { get; set; }
        public long CountryId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyToken { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public DateTime DOB { get; set; }

        public long CommunityBillingId { get; set; }    


    }
}
public class CommunitySetUpInfoDTO
{
    

    public IFormFile? OrgLogo { get; set; }
    
    public string? CommunityLogo { get; set; }

    public string? DashboardBanner { get; set; }
    public IFormFile? CommunityDashboardBanner { get; set; }

    public string CommunityName { get; set; }

    public long MembershipType { get; set; }  

    public decimal? MembershipAmount { get; set; }

    public long AccessType { get; set; }  

    public string About { get; set; }   

    public string Website { get; set; }

    public string physicalAddress { get; set; } 

    public string PlanType { get; set; }

    public string Country { get; set; }
    public long CountryId { get; set; }
    public string CurrencyCode { get; set; }
    public string CurrencyToken { get; set; }

    public string Plan { get;set; }

    public string CountryCode { get; set; } 
    public string Mobile { get; set; } 

    public long CustomerId { get; set; }    
}