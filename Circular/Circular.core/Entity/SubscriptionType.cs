using RepoDb.Attributes;

namespace Circular.Core.Entity;
[Map("mtblMembershipType")]
public class SubscriptionType : BaseEntity
{
    public string Name { get; set; }
    public string? Desc { get; set; }
    public string? Code { get; set; }
    public override void ApplyKeys()
    {

    }
}

