using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblFlexiPass")]

public class FlexiPass : BaseEntity
{
    public string? OneTimepass { get; set; }
    public decimal? Amount { get; set; }



    public override void ApplyKeys()
    {

    }
}

