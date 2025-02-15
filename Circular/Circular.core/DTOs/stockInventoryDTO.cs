using System.ComponentModel.DataAnnotations;

namespace Circular.Core.DTOs
{
    public class stockInventoryDTO : BaseEntityDTO
    {

        //public long? CustomerId { get; set; }
        public string? Product { get; set; }
        public string? Type { get; set; }
        public long? CategoryId { get; set; }
        public string ProductId { get; set; }
        public long? StoreId { get; set; }
        public long? Threshold { get; set; }
        public string? EntryStatus { get; set; }
        public decimal? Productcost { get; set; }

        public string? ProductImage { get; set; }

        public int? Quantity { get; set; }

    }
}
