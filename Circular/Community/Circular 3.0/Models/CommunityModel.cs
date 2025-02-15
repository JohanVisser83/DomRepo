using Circular.Core.Entity;
using System.Security.Policy;

namespace CircularWeb.Models
{
	public class CommunityModel : BaseModel
    {
		public CommunityModel()
		{
			this.Communities = new List<Communities>();
			this.CommunityStaff = new List<CommunityTeamProfile>();
			this.Students = new List<CustomerDetails>();
			this.customerSubscriptionStatuses = new List<CustomerSubscriptionStatus>();
			this.lstSubscribstionStatus = new List<CustomerDetails>();
			this.CustomerMembershipPaymentStatus = new List<CustomerMembershipPaymentStatus>();
            this.GetBusinessIndex = new List<CustomerBusinessIndex>();
			this.JobPostingList = new List<Jobs>();
			this.GetCustomGroups = new List<Groups>();
            this.lstFundraisers = new List<Fundraiser>();
            this.MembershipType = new List<MembershipType>();
            this.AccessType = new List<CommunityAccessType>();
            this.lsttypeoffundraiser = new List<FundraiserType>();
			this.linkedMembers = new List<LinkedMembers>();
            this.currencyModel = new CurrencyModel();
			this.ActiveOTPDetails = new List<ActiveOTPDetails>();
			this.requestlist = new List<CommunityAccessRequests>();
			this.RequestStatus = new List<CommunityAccessRequests>();
        }				

		public IEnumerable<Communities> Communities { get; set; }
		public IEnumerable<CommunityTeamProfile> CommunityStaff { get; set; }
		public IEnumerable<CustomerDetails> Students { get; set; }
        public IEnumerable<CustomerDetails> Parents { get; set; }
        public IEnumerable<CommunityAccessRequests> RequestStatus { get; set; }
        public IEnumerable<CustomerDetails> Alumni { get; set; }
        public IEnumerable<CustomerDetails> Members { get; set; }
        public IEnumerable<MembershipType> MembershipType { get; set; }
        public IEnumerable<CommunityAccessType> AccessType { get; set; }

        public IEnumerable<CustomerBusinessIndex> GetBusinessIndex { get; set; }
		public IEnumerable<Jobs> JobPostingList { get; set; }
		public IEnumerable <CustomerSubscriptionStatus> customerSubscriptionStatuses { get; set; }
		public IEnumerable <CustomerMembershipPaymentStatus> CustomerMembershipPaymentStatus { get; set; }
        public IEnumerable <CustomerDetails> lstSubscribstionStatus { get; set; }
		public IEnumerable<Groups> GetCustomGroups { get; set; }
        public IEnumerable<Fundraiser> lstFundraisers { get; set; }
        public IEnumerable<CustomerDetails> Organizers { get; set; }
        public IEnumerable<FundraiserType> lsttypeoffundraiser { get; set; }

		public IEnumerable<LinkedMembers> linkedMembers { get; set; }
        public CurrencyModel currencyModel { get; set; }

       // public int InactiveCount { get; set; }
        public IEnumerable<CustomerDetails> InactiveCount { get; set; }
		public List<ActiveOTPDetails> ActiveOTPDetails { get; set; }
        public IEnumerable<CommunityAccessRequests> requestlist { get; set; }

        public string exploreURL { get; set; }


    }
}
