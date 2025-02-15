using Microsoft.AspNetCore.Http;
using RepoDb.Attributes;

namespace Circular.Core.Entity;
[Map("tblSubscriptionCommunityInfo")]
public class SubscriptionCommunityInfo : BaseEntity
    {
      
        public string CommunityName { get; set; }

        public IFormFile? CommunityLogoImg { get; set; }
        public string CommunityLogo { get; set; }

        public IFormFile? CommunityBannerImg { get; set; }
        public string CommunityBanner { get; set; }
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

    public override void ApplyKeys()
        {

        }
    }

