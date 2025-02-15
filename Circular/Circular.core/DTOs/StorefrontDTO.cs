using Microsoft.AspNetCore.Http;

namespace Circular.Core.DTOs
{
    public class StorefrontDTO
    {
        
        public string? StoreDisplayName { get; set; }

        public string? DisplayImage { get; set; }

        public long? CommunityId { get; set; }
        public IFormFile? Mediafile { get; set; }
        
        public bool? IsTermsAndConditionsAccepted { get; set; }
    }


    
}
