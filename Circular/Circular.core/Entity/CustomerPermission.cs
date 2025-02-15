using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblUserPermission")]
public class CustomerPermission : BaseEntity 
{
    public long UserId { get; set; }
    public long PermissionId { get; set; }
    public int DeniedOrGranted { get; set; }
    public long CommunityId { get; set; }


    public override void ApplyKeys()
    {

    }
}
