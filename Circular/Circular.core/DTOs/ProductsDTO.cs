using static System.Net.Mime.MediaTypeNames;

namespace Circular.Core.DTOs
{
    public class ProductsDTO : BaseEntityDTO
    {
        public ProductsDTO()
        {
            if (Images == null)
                Images = new List<StoreProductImagesDTO>();
        }
        public long CustomerId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDesc { get; set; }

        public decimal SellingPrice { get; set; }
        public string? ProductImage { get; set; }
        public long? StoreId { get; set; }
        public long CategoryId { get; set; }

        public long Quantity { get; set; }
        public long Threshold { get; set; }
        public string? ProductUniqueID { get; set; }

        public List<StoreProductImagesDTO> Images { get; set; }
    }
}
