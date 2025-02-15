

namespace Circular.Core.DTOs
{
    public class AdvertisementMediaDTO : BaseEntityDTO
    {
        public long AdvertisementId { get; set; }

        public string Media { get; set; }

        public string MediaType { get; set; }
    }
}
