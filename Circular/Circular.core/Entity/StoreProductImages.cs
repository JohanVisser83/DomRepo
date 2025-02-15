using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblStoreProductImages")]
public class StoreProductImages : BaseEntity
    {

        public long ProductId { get; set; }
        public string? ImagePath { get; set; }
        public override void ApplyKeys()
        {

        }

}
