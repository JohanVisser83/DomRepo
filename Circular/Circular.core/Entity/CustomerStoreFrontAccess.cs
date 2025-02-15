using RepoDb.Attributes;

namespace Circular.Core.Entity;
[Map("tblCustomerStoreFrontAccess")]
public class CustomerStoreFrontAccess : BaseEntity
{

    public long CustomerId { get; set; }    

    public long StoreId { get; set; }

    public string StoreDisplayName { get; set; }
    public override void ApplyKeys()
    {

    }
}

