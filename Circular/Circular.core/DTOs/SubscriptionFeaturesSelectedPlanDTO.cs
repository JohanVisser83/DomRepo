
using Circular.Core.Entity;

namespace Circular.Core.DTOs
{
    public class SubscriptionFeaturesSelectedPlanDTO
    {
        public long SubscriptionTierId { get; set; }

        public long CustomerId { get; set; }    
        public string PlanType { get; set; }

        public string Plan { get; set; }
        public decimal Price { get; set; }

        
       

    }

    //public class SelectedFeaturesDTO
    //{

    //    public long VistiorId { get; set; }
    //    public long SubscriptionTierId { get; set; }

    //    public long FeatureId { get; set; }

    //    //public string FeatureCode { get; set; }

    //    public string MemberRange { get; set; }

    //    public string PlanType { get; set; }

    //    public decimal Price { get; set; }

   
    //    public string selectedFeatures { get; set; }

       
    //}
}

