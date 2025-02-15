using RepoDb.Attributes;

namespace Circular.Core.Entity;
[Map("mtblStatus")]
public class Status : BaseEntity
{
    public string Name { get; set; }
    public string? Desc { get; set; }

    public override void ApplyKeys()
    {

    }
}

