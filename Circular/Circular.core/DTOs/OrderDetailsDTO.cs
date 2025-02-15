namespace Circular.Core.DTOs
{
    public class OrderDetailsDTO
    {
        public long? OrderId { get; set; }
        public long? ProductId { get; set; }
        public long? Quantity { get; set; }
        public decimal? Amount { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}
