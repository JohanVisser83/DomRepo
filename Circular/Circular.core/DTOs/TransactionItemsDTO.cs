namespace Circular.Core.DTOs
{
    public class TransactionItemsDTO
    {
        public long? TransactionId { get; set; }
        public long? ProductId { get; set; }
        public decimal? ProductQty { get; set; }
        public decimal? ProductAmount { get; set; }
    }
}
