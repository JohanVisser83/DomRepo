using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblSportFixtureTeamMember")]
public class SportsTeamMember : BaseEntity
{

    public long Id { get; set; }
    public long CommunityId { get; set; }
    public long FixtureId { get; set; }
    public long TeamId { get; set; }
    public long PositionNumber { get; set; }
    public string FullName { get; set; }
    public string PlayerType { get; set; }
    
    

    public override void ApplyKeys()
    {

    }
}
