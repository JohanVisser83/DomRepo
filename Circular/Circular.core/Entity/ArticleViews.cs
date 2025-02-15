using RepoDb.Attributes;
namespace Circular.Core.Entity;

[Map("tblArticleViews")]

    public  class ArticleViews : BaseEntity
    {
        public int Feedid { get; set; }
        public long Userid { get; set; }

        public long ViewCount { get; set; } = 1;
        public override void ApplyKeys()
        {

        }
    }

