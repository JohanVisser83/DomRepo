using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblSubscription")]

public class Subscription : BaseEntity
{

    public string? Name { get; set; }


    public override void ApplyKeys()
    {

    }
}
