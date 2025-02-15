
using Circular.Core.Entity;

namespace NewCircularSubscription.Models
{
    public class CommunityMembershipModel
    {
        public CommunityMembershipModel()
        {
            this.CommunityMembership = new List<CommunityDetails>();
        }



        public IEnumerable<CommunityDetails> CommunityMembership { get; set; }

        public string currency { get; set; }

        public string CommunityURL { get; set; }    

        public string LearnmorecircularURL { get; set; }
        public string CommunityPortalURL { get; set; }

        public string DiscoverCommunityAboutusURL { get; set; } 
    }
}
