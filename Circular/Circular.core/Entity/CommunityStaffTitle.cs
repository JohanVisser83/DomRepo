using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblCommunityStaffTitle")]

public class CommunityStaffTitle : BaseEntity
{
    public string? Name { get; set; }
    public string? Desc { get; set; }
    public string? EmailId { get; set; }


    public override void ApplyKeys()
    {

    }
}
