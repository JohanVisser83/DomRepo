namespace Circular.Core.DTOs
{
    public class OrderDTO
    {
        public long Id { get; set; } 
        public string? BuyerName { get; set; }
        public long? CustomerId { get; set; }
        public long? StoreId { get; set; }
        public decimal Amount { get; set; }
        public long NoOfItems { get; set; }
        public long? OrderdForId { get; set; }
        public bool IsCollected { get; set; }
        public bool IsPaid { get; set; }
        public string? TransactionCode { get; set; }
        public decimal DeliveryFee { get; set; }
        public DateTime? OrderDate { get; set; }

        public string? Mobile { get; set; }
        public string? QRCode { get; set; }
        public long? Status { get; set; }

        public long? OrderId { get; set; }
       

        public string? CustomerName { get; set; }

        public string? ProductName { get; set; }

        public int? Quantity { get; set; }
        public long Count { get; set; }
        public decimal TotalSales { get; set; }

        public decimal Cashsales { get; set; }

        public decimal CircularPayment { get; set; }

        public decimal TotalReceipent { get; set; }

        public decimal Diffrence { get; set; }
    }
}
