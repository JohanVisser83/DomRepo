using Circular.Core.Entity;

namespace CircularWeb.Models
{
    public class SafetyModel : BaseModel
    {
        public SafetyModel()
        {
            this.lstCommunityClasses = new List<CommunityClasses>();         
            this.lstvehicles = new List<Vehicles>();          
            this.lstDisplaycode = new List<CommunityTransportPass>();
            this.lstTicketsale = new List<TicketDays>();
			this.lstcommclassteachers = new List<StaffClasses>();
            this.lstWellness = new List<CommunityGuidanceWellness>();
            this.currencyModel = new CurrencyModel();
            this.getCommunityGuidanceWellnessOptions  = new List<CommunityGuidanceWellnessOptions>();   
        }

        public IEnumerable<CommunityClasses> lstCommunityClasses { get; set; }     
        public IEnumerable<Vehicles> lstvehicles { get; set; }
        public IEnumerable<CommunityTransportPass> lstDisplaycode { get; set; }     
        public IEnumerable<TicketDays> lstTicketsale { get; set; }
		public IEnumerable<StaffClasses> lstcommclassteachers { get; set; }

        public IEnumerable<CommunityGuidanceWellness> lstWellness { get; set; }
        public CurrencyModel currencyModel { get; set; }
        public List<CommunityGuidanceWellnessOptions> getCommunityGuidanceWellnessOptions { get; set; }

    }
}
