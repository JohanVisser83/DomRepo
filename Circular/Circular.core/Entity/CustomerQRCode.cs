using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblCustomerQRCode")]

public class CustomerQRCode : BaseEntity
{
    public long? CustomerId { get; set; }
    public string? QRCode { get; set; }


    public override void ApplyKeys()
    {

    }
}
