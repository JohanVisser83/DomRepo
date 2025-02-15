namespace Circular.Core.DTOs
{
    public class FeatureSubscriptionFeeDTO : BaseEntityDTO
    {
        public long CustomerId { get; set; }
        public long CommunityId { get; set; }
        public long FeatureId { get; set; }
        public string Code { get; set; }
        public decimal Price { get; set; }
    }
    public class SelectedCommunityFeaturesDTO
    {
        public long CustomerId { get; set; }
        public long CommunityId { get; set; }
        public long members { get; set; }
        public decimal monthlysubscription { get; set; }
        public decimal addons { get; set; }
        public decimal onceOff { get; set; }
        public decimal Totalmonthlysubscription { get; set; }

        public string selectedFeatures { get; set; }
    }
}
