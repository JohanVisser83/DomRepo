using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblLikedFeeds")]
public class LikedFeeds : BaseEntity
{
    public int Feedid { get; set; }
    public long Userid { get; set; }


    public override void ApplyKeys()
    {

    }
}
