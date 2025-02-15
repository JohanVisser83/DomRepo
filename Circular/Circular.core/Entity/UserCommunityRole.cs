using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblUserCommunityRole")]

public class UserCommunityRole : BaseEntity
{
    public long? CustomerId { get; set; }
    public long? CommunityRoleId { get; set; }
    public long? CommunityId { get; set; }
    public bool? IsOrganizer { get; set; }


    public override void ApplyKeys()
    {

    }
}
