namespace Circular.Core.DTOs
{
    public class AddCartDTO
    {
        public long? ProductId { get; set; }
        
        public string? ProductName { get; set; }
        public long? CustomerId { get; set; }
        public long? OrderId { get; set; }
        public long? Quantity { get; set; }
        public decimal? Amount { get; set; }
        public long? StoreId { get; set; }

        public string? Mobile { get; set; } 
        
        public string? CustomerName { get; set; }
       
        public string? PaymentType { get; set; }
    }

   
}
