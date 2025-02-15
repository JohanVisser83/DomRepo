using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblCommunityDrivers")]

public class CommunityDrivers : BaseEntity
{
    public long OrgId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? ContactNumber { get; set; }


    public override void ApplyKeys()
    {

    }
}
