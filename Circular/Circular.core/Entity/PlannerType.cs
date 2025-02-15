using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblPlannerType")]

public class PlannerType : BaseEntity
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }


    public override void ApplyKeys()
    {

    }
}
