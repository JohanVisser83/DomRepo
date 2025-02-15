using Microsoft.AspNetCore.Http;
using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblArticleMedia")]



    public  class ArticleMedia: BaseEntity
    {
      public long NewFeedsId { get; set; }  

      public string Media { get; set; }   

      public string MediaType { get; set; }

      public string filename { get; set; }


      public override void ApplyKeys()
      {

      }

    }

