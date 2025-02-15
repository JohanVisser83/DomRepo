using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblFoodCategory")]

public class FoodCategory : BaseEntity
{
    public string? Menu { get; set; }
    public string? Icon { get; set; }


    public override void ApplyKeys()
    {

    }
}