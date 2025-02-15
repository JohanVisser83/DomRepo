using Microsoft.AspNetCore.Http;

namespace Circular.Core.DTOs
{
    public class AdvertisementDTOs : BaseEntityDTO
    {
        public long? CommunityId { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string CoverAdvertisementMedia { get; set; }
        public string CoverMediaThumbnail { get; set; }
        public string Url { get; set; }

        public IFormFile Mediafile { get; set; }
    }
}
