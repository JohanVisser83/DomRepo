using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblInvoiceVendorCode")]

public class InvoiceVendorCode : BaseEntity
{
    public long? CustomerCodeSeq { get; set; }


    public override void ApplyKeys()
    {

    }
}
