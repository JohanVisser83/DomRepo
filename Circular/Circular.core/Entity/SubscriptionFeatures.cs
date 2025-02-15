using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblCommunitySubscription")]

public class SubscriptionFeatures : MasterEntity
{

    public decimal Price { get; set; }

    public string? FeatureCode { get; set; }
     public bool IsFree { get; set; }    

    public string? AdditionalCost { get; set; } 

    public long CommunityFeature { get; set; }

   



    public override void ApplyKeys()
    {

    }

}


public class CurrentCommunityPlan
{
    public string PlanName { get; set; }
    public string Price { get; set; }
    public string AdditionalFees { get; set; }
    public string Features { get; set; }

    public string PlanType { get; set; }    
    public string SubscriptionStatus { get; set; }

}


public class CommunityMemberTransaction
{
    public long TrasactionId { get; set; }  

    public long TransactionFromID { get; set; }

    public long TransactionToID { get; set;}

    public string TransactionFromName { get; set; }

    public string TransactionDateFormat { get; set; }

    public string TransactionCode { get; set; }

    public string Amount { get; set; }

    public string Description { get; set; }

    public string TransactionType { get; set; }


    public string PlanName { get; set; }
}