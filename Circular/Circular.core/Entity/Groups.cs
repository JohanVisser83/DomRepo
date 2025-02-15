using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblGroups")]

public class Groups : BaseEntity
{
    public new long Id { get; set; }
    public long? OrgId { get; set; }
    public string? GroupName { get; set; }
    public string? GroupDesc { get; set; }
    public long? CommunityID { get; set; }
    public bool? IsAddedByHQ { get; set; }

    public string MemberCount { get; set; }
    public override void ApplyKeys()
    {

    }
}
