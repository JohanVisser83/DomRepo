

namespace Circular.Core.DTOs
{
    public  class AdminFeatureDTO
    {
        public long CommunityId { get; set; }

        public long CustomerId { get; set; }
        public long FeatureId { get; set; }

        public string Code { get; set; }

        public string FeatureName { get; set; }
    }
}
