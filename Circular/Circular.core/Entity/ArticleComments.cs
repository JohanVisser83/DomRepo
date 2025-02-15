using RepoDb.Attributes;

namespace Circular.Core.Entity;
[Map("tblarticlecomments")]


public class ArticleComments : BaseEntity
{
    public long ArticleId { get; set; }
    public long CustomerId { get; set; }
    public long CommunityId { get; set; }
    public long? ParentCommentId { get; set; }
    public string Comment { get; set; }


    public override void ApplyKeys()
    {

    }
}
public class ArticlePostComment : BaseEntity
{
    public long ArticleId { get; set; }
    public long communityId { get; set; }
    public long CustomerId { get; set; }
    public long? ParentCommentId { get; set; }
    public string Comment { get; set; }
    public string CommentBy { get; set; }
    public string CommentByProfilePic { get; set; }
    public ArticlePostComment()
    {
        this.userReplies = new List<UserReply>();
    }
    public List<UserReply> userReplies { get; set; }
    public override void ApplyKeys()
    {

    }
}

public class UserReply
{
    public string Comment { get; set; }
    public string Name { get; set; }
    public string ReplyUserProfilePic { get; set; }
    public long ParentCommentId { get; set; }
    public DateTime CreatedDate { get; set; }
}
