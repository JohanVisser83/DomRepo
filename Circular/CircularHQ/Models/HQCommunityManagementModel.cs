using Circular.Core.Entity;

namespace CircularHQ.Models
{
    public class HQCommunityManagementModel : BaseModel
    {
        public HQCommunityManagementModel() 
        {
            this.Communities = new List<HQCommunitiesList>();
            this.CommunitiesCategories = new List<CommunityCategory>();
            this.CommunitiesEditDetails = new Communities();
            this.HalfBakedCommunity = new List<CommunitySignUp>();
            this.ddlCommunities = new List<Communities>();
            this.Groups = new List<Groups>();
            this.HQAllMemberDetails = new List<HQAllMemberDetails>();
            this.hQCommunityTransactionDetails = new List<HQCommunityTransactionDetails>();
            this.lstCountryName = new List<Country>();
        }

        public IEnumerable<HQCommunitiesList> Communities { get; set; }

        public IEnumerable<CommunityCategory> CommunitiesCategories { get; set;}

        public Communities CommunitiesEditDetails { get; set; }

        public Communities HalfBakedCommunityDetails { get; set; }  

        public IEnumerable<CommunitySignUp> HalfBakedCommunity { get; set; }

        public IEnumerable<Communities> ddlCommunities { get; set; }

        public IEnumerable<CustomerDetails> Member { get; set; }

        public IEnumerable<Groups> Groups { get; set; }

        public IEnumerable<HQAllMemberDetails> HQAllMemberDetails { get; set; }

        public IEnumerable<HQCommunityTransactionDetails> hQCommunityTransactionDetails { get; set; }

        public IEnumerable<Country> lstCountryName { get; set; }

    }
}
