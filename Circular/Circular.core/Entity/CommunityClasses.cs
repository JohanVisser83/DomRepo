using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("mtblCommunityClasses")]
public class CommunityClasses : MasterEntity
{
 
    public long CommunityId { get; set; }
    public override void ApplyKeys()
    {

    }
}
                 