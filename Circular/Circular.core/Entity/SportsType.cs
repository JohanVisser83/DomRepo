using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("mtblSportsType")]
 public class SportsType : BaseEntity
 {
    

     public string Activities { get; set; }

     public override void ApplyKeys()
     {

     }

 }

