using Circular.Core.Entity;
using Microsoft.AspNetCore.Http;

namespace Circular.Core.DTOs
{
    public class NewsFeedsDTO : BaseEntityDTO
    {
        public NewsFeedsDTO()
        {
            if (ArticleMedia == null)
                ArticleMedia = new List<ArticleMediaDTO>();
        }
        public string? Title { get; set; }
        public string? Summary { get; set; }
        public bool? IsArchived { get; set; }
        public long CommunityId { get; set; }

        public long CustomerId { get; set; }
        public long IsGroup { get; set; }
        public long? ReferenceTypeId { get; set; }
        public long? ReferenceId { get; set; }
        public string? MessageMedia { get; set; }
        public string? MessageMediaThumbnail { get; set; }
        public string? WroteArticle { get; set; }
        public string? PostedByName { get; set; }
        public string? PostedByProfilePic { get; set; }
        public int IsLiked { get; set; }
        public string? DocumentPath { get; set; }

        public long TotalLikes { get; set; }

        public string? postURL { get; set; }    
        public string? Type { get; set; }

       
        public IFormFile? Mediafile { get; set; }
        public string CommunityName { get; set; }

        public string? GroupName { get; set; }

        public int Isfeatured { get; set; }

        public string TotalViews { get; set; }
        public List<ArticleMediaDTO> ArticleMedia { get; set; }

       

    }



    public class PostArticleDTO 
    {

        public PostArticleDTO()
        {
            if (ArticleMedia == null)
                ArticleMedia = new List<ArticleMediaDTO>();
        }
        public string? Title { get; set; }
        public string? Summary { get; set; }

        public bool? IsArchived { get; set; }
        public long CommunityId { get; set; }
        public long CustomerId { get; set; }

        public long IsGroup { get; set; }
        public long? ReferenceTypeId { get; set; }
        public long? ReferenceId { get; set; }
        public string? MessageMedia { get; set; }
        public string? MessageMediaThumbnail { get; set; }
        public string? WroteArticle { get; set; }

        public string? DocumentPath { get; set; }

        public int Isfeatured { get; set; }

        public string? Type { get; set; }

        public string CommunityName { get; set; } 
        public List<ArticleMediaDTO> ArticleMedia { get; set; }


    }

    public class GroupCommunityId
    {
        public long CommunityId { get; set; }
    }

}
