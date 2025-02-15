using Circular.Core.Entity;

namespace NewCircularSubscription.Models
{
    public class CommunityDetailsModel
    {
        public CommunityDetailsModel()
        {
            this.lstCommunitydetails = new List<CommunityDetails>();
            this.lstCountryName = new List<Country>();
        }

        public IEnumerable<CommunityDetails> lstCommunitydetails { get; set; }
        public IEnumerable<Country> lstCountryName { get; set; }
        public string currency { get; set; }
    }
}
