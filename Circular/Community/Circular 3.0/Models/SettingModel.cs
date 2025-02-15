using Circular.Core.Entity;

namespace CircularWeb.Models
{
    public class SettingModel : BaseModel
    {
		public SettingModel()
		{
            this.StoreData = new List<CustomerStore>();
            this.lstRole = new List<communityroles>();
            this.lstAccesssControl = new List<EventOrgBind>();
			this.lstfeature = new List<Features>();
            this.lstSubscriptionsTransactions = new List<TransactionSubscription>();
            this.lstbuySubscriptionFeatures = new List<SubscriptionFeatures>();
            this.lstFeaturesPrice = new List<CommunitySignUp>();
            this.Members = new List<CustomerDetails>();
            this.Memberslist = new List<CustomerDetails>();
            this.lstHouses = new List<House>();
            this.lstCommunityCurrentPlan = new List<CurrentCommunityPlan>();
            this.lstCommunitySubsTransactionlist = new List<CommunityMemberTransaction>();
            this.StoreDataFrontAccess = new List<CustomerStoreFrontAccess>();
            this.SubscriptionDetails = new List<SubscriptionDetails>();
            this.CustomerPaymentGetwayDetails = new List<CustomerPaymentGatewayDetails>();
        }
        public IEnumerable<communityroles> lstRole { get; set; }
        public IEnumerable<EventOrgBind> lstAccesssControl { get; set; }
        public IEnumerable<Features> lstfeature { get; set; }
        public List<TransactionSubscription> lstSubscriptionsTransactions { get; set; }
        public IEnumerable<SubscriptionFeatures> lstbuySubscriptionFeatures { get; set; }
        public IEnumerable<CommunitySignUp> lstFeaturesPrice { get; set; }
        public IEnumerable<CustomerDetails> Members { get; set; }
        public IEnumerable<CustomerDetails> Memberslist { get; set; }    
        public IEnumerable<House> lstHouses { get; set; }

        public IEnumerable<CurrentCommunityPlan> lstCommunityCurrentPlan { get; set; } 
        
        public IEnumerable<CommunityMemberTransaction> lstCommunitySubsTransactionlist { get; set; }

        public string currency { get; set; }

        public IEnumerable<CustomerStore> StoreData { get; set; }

        public IEnumerable<CustomerStoreFrontAccess> StoreDataFrontAccess { get; set;}
        
        public IEnumerable<SubscriptionDetails> SubscriptionDetails { get; set; }   
        public IEnumerable<CustomerPaymentGatewayDetails> CustomerPaymentGetwayDetails { get; set; }   

    }
}
