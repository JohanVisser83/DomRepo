using Circular.Core.Entity;

namespace NewCircularSubscription.Models
{
    public class CommunitySignUpModel
    {
        public CommunitySignUpModel()
        {
            this.CommunityAccessTypes = new List<CommunityAccessType>();
            this.SubscriptionTypes = new List<SubscriptionType>();
            this.lstCountryName = new List<Country>();
        }

        public IEnumerable<CommunityAccessType> CommunityAccessTypes { get; set; }
        
        public IEnumerable<SubscriptionType> SubscriptionTypes { get; set; }

        public IEnumerable<Country> lstCountryName { get; set; }
    }
}
