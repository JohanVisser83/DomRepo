

namespace Circular.Core.DTOs
{
    public  class SubscriptionDetailsDTO
    {
        public long CustomerId { get; set; }

        public string StripeSubscriptionId { get; set; }

        public long CommunityMembershipId { get; set; }

        public string SubscriptionStatus { get; set; }

        public long TransactionId { get; set; }

        public DateTime CurrentPeriodStart { get; set; }

        public DateTime CurrentPeriodEnd { get; set; }
    }
}
