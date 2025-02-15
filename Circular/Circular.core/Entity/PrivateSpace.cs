using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblPrivateSpace")]



    public  class PrivateSpace : BaseEntity
    {
       public string? Title { get; set; }    

       public string?  MediaThumbnail { get; set; }  

       public string? Description { get; set; }  

	   public string?  DiscoverUrl { get; set; } 
       public override void ApplyKeys()
       {
           throw new NotImplementedException();
       }
    }

