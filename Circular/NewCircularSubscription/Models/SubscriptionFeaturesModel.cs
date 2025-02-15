using Circular.Core.Entity;


namespace NewCircularSubscription.Models
{
    public class SubscriptionFeaturesModel
    {
        public SubscriptionFeaturesModel() 
        {
            this.startUpsFeatures = new List<SubscriptionTier>();
            this.essentialFeatures = new List<SubscriptionTier>();
            this.revenueMaxFeatures = new List<SubscriptionTier>();
            this.startUpsFeaturesYearly = new List<SubscriptionTier>();
            this.essentialFeaturesYearly = new List<SubscriptionTier>();
            this.revenueMaxFeaturesYearly = new List<SubscriptionTier>();
            
            this.lstCountryName = new List<Country>();
            this.orderDetails = new List<SubscriptionFeaturesSelectedPlan>();
        }

        public string FeaturePrice { get; set; }
        
        public string Plan { get; set; }
        
        public string PlanPeriod { get; set; }  

        public IEnumerable<SubscriptionTier> startUpsFeatures { get; set; }

        public IEnumerable<SubscriptionTier> essentialFeatures { get; set; }

        public IEnumerable<SubscriptionTier> revenueMaxFeatures { get; set; }

        public IEnumerable<SubscriptionTier> startUpsFeaturesYearly { get; set; }

        public IEnumerable<SubscriptionTier> essentialFeaturesYearly { get; set; }

        public IEnumerable<SubscriptionTier> revenueMaxFeaturesYearly { get; set; }

        public IEnumerable<Country> lstCountryName { get; set; }

        public SubscriptionTier subscriptionTiers { get; set; }

        public IEnumerable<SubscriptionFeaturesSelectedPlan> orderDetails { get; set; } 

        public string currency { get; set; }

        public string TotalPrice { get; set; }

    }
}


