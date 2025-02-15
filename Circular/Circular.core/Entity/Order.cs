using RepoDb.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Circular.Core.Entity;

[Map("tblOrder")]

public class Order : BaseEntity
{
    public Order ()
    {
        this.orderDetails = new List<OrderDetails>();
        if (QRCode1 == null)
            QRCode1 = new QR();
    }

    public long Id { get; set; }    
    public string? BuyerName {get; set;}          
    public long CustomerId { get; set; }
    public long? StoreId { get; set; }
    public string? StoreName { get; set; }
    public decimal Amount { get; set; }
    [Column("No.OfItems")]
    public long NoOfItems { get; set; }
    public long? OrderdForId { get; set; }
    public string? OrderedForName { get; set; }
    public bool IsCollected { get; set; }
    public bool IsPaid { get; set; }
    public string? TransactionCode { get; set; }
    public decimal DeliveryFee { get; set; }
    public DateTime? OrderDate { get; set; }
    
    public string? Mobile {get; set; }
    public string? QRCode { get; set; }
    public long? Status { get; set; }

    public long ? CommunityId { get; set; }

    public List<OrderDetails>orderDetails { get; set; }

    public long? OrderID { get; set; }
    
    public string? CustomerName { get; set;}
    
    public string? ProductName {get; set;}

    public int? Quantity { get; set; }

    public long Count { get; set; }
    public decimal TotalSales { get; set; }

    public decimal Cashsales { get; set; }

    public decimal TotalReceipent { get; set; }

    public decimal CircularPayment {get; set;}

    public decimal Diffrence { get; set; }



   
    public QR? QRCode1 { get; set; }


    public override void ApplyKeys()
    {

    }
}
