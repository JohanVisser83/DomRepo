using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblNewsFeeds")]
public class NewsFeeds : BaseEntity
{

    public NewsFeeds() 
    {
            if (ArticleMedia == null)
            ArticleMedia = new List<ArticleMedia>();
    }

    public string? Title { get; set; }
    public string? Summary { get; set; }
    public bool? IsArchived { get; set; }
    public long? CommunityId { get; set; }

    public long CustomerId { get; set; }
    public long? IsGroup { get; set; }
    public long? ReferenceTypeId { get; set; }
    public long? ReferenceId { get; set; }
    public string? MessageMedia { get; set; }
    public string? MessageMediaThumbnail { get; set; }
    public string? WroteArticle { get; set; }
    public string? PostedByName { get; set; }
    public string? PostedByProfilePic { get; set; }
    public int IsLiked { get; set; }
    public string? DocumentPath { get; set; }
    public string? Type { get; set; }
    public long TotalLikes { get; set; }
    public string? postURL { get; set; }

    public string CommunityName { get; set; }

    public string? GroupName { get; set; }

    public int Isfeatured { get; set; }

    public string TotalViews { get; set; }



    public List<ArticleMedia> ArticleMedia { get; set; }

    public override void ApplyKeys()
    {
        if (ArticleMedia != null)
        {
            foreach (var item in ArticleMedia)
            {
                item.NewFeedsId = Id;
                item.GUID = new Guid(Guid.NewGuid().ToString());
            }
        }
    }
}

