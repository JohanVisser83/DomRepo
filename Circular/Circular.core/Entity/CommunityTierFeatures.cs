using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("mtblCommunityTierFeatureMapping")]
    public  class CommunityTierFeatures: MasterEntity
    {

        public decimal Price { get; set; }
        public string? AdditionalText { get; set; }
        public string? FeatureCode { get; set; }
        public long SubscriptionTierId { get; set; }    
        public override void ApplyKeys()
        {

        }
    }





