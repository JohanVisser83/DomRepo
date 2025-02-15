using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblOrderDetails")]

public class OrderDetails : BaseEntity
{
    public long? OrderId { get; set; }
    public long? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductImage { get; set; }
    public long? Quantity { get; set; }
    public decimal? Amount { get; set; }
    public decimal? TotalAmount { get; set; }



    public override void ApplyKeys()
    {

    }
}
