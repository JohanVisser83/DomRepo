using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("CustomerStoreDocuments")]

public class CustomerStoreDocuments : BaseEntity
{
    public long StoreId { get; set; }
    public string DocName { get; set; }


    public override void ApplyKeys()
    {

    }
}
