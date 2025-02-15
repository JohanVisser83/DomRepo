using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblAddCart")]

public class AddCart : BaseEntity
{
    public long? ProductId { get; set; }
    public string? ProductName {get; set; }

    public long? CustomerId { get; set; }
    public long? OrderId { get; set; }
    public long? Quantity { get; set; }
    public decimal? Amount { get; set; }
    public long? StoreId { get; set; }
    

    public string? Mobile { get; set; }

    public string? CustomerName { get; set; }
    public string? PaymentType { get; set; }


    public override void ApplyKeys()
    {

    }
}
