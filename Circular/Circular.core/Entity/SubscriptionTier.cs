using RepoDb.Attributes;

namespace Circular.Core.Entity;
[Map("mtblsubscriptionTier")]

    public class SubscriptionTier : MasterEntity
    {
        public string MemberRange { get; set; } 
        public string PlanType { get; set; }

       public string AdditionalFees { get; set; }
         public string StripePriceId { get; set; }
    public override void ApplyKeys()
        {

        }
}

