using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblCommunitySubscriptionStatus")]

public class CommunitySubscriptionStatus : BaseEntity
{
    public string Name { get; set; }


    public override void ApplyKeys()
    {

    }
}