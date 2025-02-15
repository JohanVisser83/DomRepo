using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblFriends")]

public class Friend : BaseEntity
{
    public long? FromId { get; set; }
    public long? ToId { get; set; }
    public int? FriendRequestStatusId { get; set; }


    public override void ApplyKeys()
    {

    }
}