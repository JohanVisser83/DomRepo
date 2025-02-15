

namespace Circular.Core.DTOs
{
    public class ManageFixtureDTO
    {
       
        public string FixtureTitle { get; set; }
        public DateTime Time { get; set; }    
        public string Location { get; set; }
        public long SportId { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public long SportTypeId { get; set; }

    }
}
