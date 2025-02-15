
namespace Circular.Core.DTOs
{
    public class SportsTeamMemberDTO
    {
		public long Id { get; set; }
		public long CommunityId { get; set; }
		public long FixtureId { get; set; }
		public long TeamId { get; set; }
		public long PositionNumber { get; set; }
		public string FullName { get; set; }
		public string PlayerType { get; set; }

	}
}
