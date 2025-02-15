using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblPlanner")]
public class Planner : BaseEntity
{
    public long PlannerTypeId { get; set; }
    public long CommunityId { get; set; }
    public string? title { get; set; }
    public string? Description { get; set; }
    public string? IsArchived { get; set; }
    public string? Media { get; set; } 
    public string? HyperLink { get; set; }
    public decimal? Price { get; set; }
    public string PlannerTypeName { get; set; }
    public bool Isbought { get; set; } = false;


    public override void ApplyKeys()
    {

    }
}
