using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblfeatures")]
public class FeatureSubscriptionsFee : BaseEntity
{
    public long CustomerId { get; set; }    
    public long CommunityId { get; set; }
    public long FeatureId { get; set; } 
    public string Code { get; set; }    
    public decimal Price { get; set; }
    public string? StripePriceId { get; set; }

    public override void ApplyKeys()
    {

    }
}

public class SelectedCommunityFeatures
{
    public long CustomerId { get; set; }
    public long CommunityId { get; set; }
    public long members { get; set; }
    public decimal monthlysubscription { get; set; }
    public decimal addons { get; set; }
    public decimal onceOff { get; set; }
    public decimal Totalmonthlysubscription { get; set; }
    public string selectedFeatures { get; set; }

    public List<FeatureSubscriptionsFee> featureSubscriptionsFees { 
        get
        {
            List<FeatureSubscriptionsFee> features = new List<FeatureSubscriptionsFee>();
            if (!string.IsNullOrEmpty(selectedFeatures))
            {
                string[] values = new string[] { };
                values = selectedFeatures.Split(',');

                foreach (string value in values)
                {
                    string[] str = new string[] { };
                    str = value.Split('^');
                    FeatureSubscriptionsFee pm = new FeatureSubscriptionsFee();
                    pm.CustomerId = CustomerId; 
                    pm.CommunityId = CommunityId;
                    pm.Code = str[2];
                    pm.FeatureId = long.Parse(str[1]);
                    pm.Price = decimal.Parse(str[0]);
                    features.Add(pm);
                }
            }
            return features;
        }
    }
}

