using Circular.Core.Entity;
namespace CircularWeb.Models
{
    public class SportsModel : BaseModel
    {
        public SportsModel() 
        {
            this.lstSports = new List<SportsType>();
            this.lstSportsSpotlightDetails = new List<SportFixture>();
            this.lstSportsTeamList = new List<SportsTeams>();
            this.lstactivity = new List<SportsType>();
            this.lstUpcomingRecord = new List<Sports>();
            this.lstdata = new List<Sports>();
            this.lstsportsupcoming = new SportsListResponse();
        }

        public IEnumerable<SportsType> lstSports { get; set; }
        public IEnumerable<SportFixture> lstSportsSpotlightDetails { get; set; }
        public IEnumerable<SportsTeams> lstSportsTeamList { get; set; }
        public IEnumerable<SportsType> lstactivity { get; set; }
        public IEnumerable<Sports> lstUpcomingRecord { get; set; }
        public IEnumerable<Sports> lstdata { get; set; }
        public SportsListResponse lstsportsupcoming { get; set; }


    }
}
