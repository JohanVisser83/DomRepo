using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblCommunityRestrictedFeatures")]

public class CommunityRestrictedFeatures : BaseEntity
{
    public long? CommunityId { get; set; }
    public long? FeatureId { get; set; }

    public override void ApplyKeys()
    {

    }
}