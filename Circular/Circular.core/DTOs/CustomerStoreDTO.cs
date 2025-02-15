using Microsoft.AspNetCore.Http;

namespace Circular.Core.DTOs
{
    public class CustomerStoreDTO
    {
        public string? StoreDisplayName { get; set; }
        public long Id { get; set; }
    
        public string? DisplayImage { get; set; }
      
        public string? SalesSupportNumber { get; set; }
        public IFormFile? Mediafile { get; set; }
        public long? CommunityId { get; set; }
      

    }
}
