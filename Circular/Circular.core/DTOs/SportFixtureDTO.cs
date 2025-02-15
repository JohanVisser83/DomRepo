namespace Circular.Core.DTOs
{
    public class SportFixtureDTO 
    {

		public string FixtureTitle { get; set; }
		public DateTime Time { get; set; }
		public DateTime Date { get; set; }
		public string Location { get; set; }
		public long SportId { get; set; }
		public long SportTypeId { get; set; }
		public string Result { get; set; }

		public string HomeTeam { get; set; }
		public string AwayTeam { get; set; }
		public long HomeTeamId { get; set; }
		public long AwayTeamId { get; set; }

	}
    public class GetFixtures
    {
        public long Id { get; set; }
        public string FixtureTitle { get; set; }
        public DateTime Time { get; set; }
        
        public string Location { get; set; }
        public long SportId { get; set; }
      
        public long SportTypeId { get; set; }
        public string Result { get; set; }
        public string Activities { get; set; }

        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public long HomeTeamId { get; set; }
        public long AwayTeamId { get; set; }

    }
}
