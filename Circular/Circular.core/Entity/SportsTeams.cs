using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblSportFixtureTeams")]
    public  class SportsTeams : BaseEntity
    {
    public string TeamName { get; set; }
    public long FixtureId { get; set; }
    public bool IsHomeTeam { get; set; }


    public override void ApplyKeys()
    {

    }
}

