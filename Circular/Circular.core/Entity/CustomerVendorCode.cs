using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblCustomerVendorCode")]

public class CustomerVendorCode : BaseEntity
{
    public long? CustomerCodeSeq { get; set; }


    public override void ApplyKeys()
    {

    }
}
