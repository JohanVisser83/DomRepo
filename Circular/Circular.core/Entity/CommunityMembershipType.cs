using RepoDb.Attributes;

namespace Circular.Core.Entity;
[Map("tblCommunityMembershipType")]

    public class CommunityMembershipType :BaseEntity
    {
    public long  CommunityId { get; set; }
    public long  MembershipTypeId { get; set; } 
    public override void ApplyKeys()
    {

    }
}

