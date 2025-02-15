using RepoDb.Attributes;

namespace Circular.Core.Entity;
[Map("tblSubscriptionDetails")]
public class SubscriptionDetails : BaseEntity
{
    public long CustomerId { get; set; }

    public string StripeSubscriptionId { get; set; }

    public long CommunityMembershipId { get; set; }

    public string SubscriptionStatus { get; set; } 
    
    public long TransactionId { get; set; } 

    public DateTime CurrentPeriodStart { get; set; }    

    public DateTime CurrentPeriodEnd { get; set; }  

    public string stripeCustomerId { get; set; }  
    
    public string Email { get; set; }   

    public override void ApplyKeys()
    {

    }

}

public class ActualExpiredSubscriptions
{
    public ActualExpiredSubscriptions()
    {
    }

    public string Email { get; set; }
    public string SubscriptionId { get; set; }  
    public long CustomerId { get; set; }

    public long CommunityMembershipId { get; set ; }
    public bool IsOwnerSubscription { get; set; }

}

public class ExpiredSubscriptionsNotifications
{
    public ExpiredSubscriptionsNotifications()
    {
        actualExpiredSubscriptions =  new List<ActualExpiredSubscriptions>();
        subscriptionDetails = new List<SubscriptionDetails>();
    }
   public List<ActualExpiredSubscriptions> actualExpiredSubscriptions {  get; set; }
   public List<SubscriptionDetails> subscriptionDetails { get; set; }

}
