using RepoDb.Attributes;

namespace Circular.Core.Entity;
[Map("tblSubscriptionSelectedFeatures")]
    public class SubscriptionFeaturesSelectedPlan : BaseEntity
    {
     
      public long SubscriptionTierId { get; set; }

      public long CustomerId { get; set; }
      public string PlanType { get; set; }

      public string Plan { get; set; }  

      public decimal Price { get; set; }
    public string StripePriceId { get; set; }

    public int TrialDays { get; set; }

    public override void ApplyKeys()
       {

       }

    }



public class SelectedFeatures
{

    public long VistiorId { get; set; }
    public long SubscriptionTierId { get; set; }

    public long FeatureId { get; set; }

    //public string FeatureCode { get; set; }

    public string MemberRange { get; set; }

    public string PlanType { get; set; }

    public decimal Price { get; set; }

    public string selectedFeatures { get; set; }

    //public List<SubscriptionFeaturesSelectedPlan> featureSubscriptionsplan
    //{
    //    get
    //    {
    //        List<SubscriptionFeaturesSelectedPlan> features = new List<SubscriptionFeaturesSelectedPlan>();
    //        if (!string.IsNullOrEmpty(selectedFeatures))
    //        {
    //            string[] values = new string[] { };
    //            values = selectedFeatures.Split(',');

    //            foreach (string value in values)
    //            {
    //                string[] str = new string[] { };
    //                str = value.Split('^');
    //                SubscriptionFeaturesSelectedPlan pm = new SubscriptionFeaturesSelectedPlan();
                    
                    
    //                pm.FeatureCode = str[2];
    //                pm.FeatureId = long.Parse(str[1]);
    //                pm.Price = decimal.Parse(str[0]);

                

    //                features.Add(pm);
    //            }
    //        }
    //        return features;
    //    }
    //}
}

