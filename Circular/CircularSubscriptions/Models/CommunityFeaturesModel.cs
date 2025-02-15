using Circular.Core.Entity;



namespace CircularSubscriptions.Models
{
    public class CommunityFeaturesModel
    {
        public CommunityFeaturesModel()
        {
            this.AllSubscriptionfeature = new List<SubscriptionFeatures>();
            this.lstfeature = new List<SubscriptionFeatures>();
            this.lstSupportfeature = new List<SubscriptionFeatures>();
            this.lstHelpReqfeature = new List<SubscriptionFeatures>();
            this.lstFeatures = new List<FeatureSubscriptionsFee>();

        }


        public IEnumerable<SubscriptionFeatures> AllSubscriptionfeature { get; set; }

        public IEnumerable<SubscriptionFeatures> lstfeature { get; set; }

        public IEnumerable<SubscriptionFeatures> lstSupportfeature { get; set; }

        public IEnumerable<SubscriptionFeatures> lstHelpReqfeature { get; set; }

        public IEnumerable<FeatureSubscriptionsFee> lstFeatures { get; set; }
        
        public string redirectContactSales {get;set;}
        public string ActualCost { get; set; }
        public string PayButtonText { get; set; }


    }


}
