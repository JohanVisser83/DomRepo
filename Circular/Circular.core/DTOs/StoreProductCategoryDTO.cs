namespace Circular.Core.DTOs
{
    public class StoreProductCategoryDTO
    {
        public long Id { get; set; }    
        public long? StoreId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryDesc { get; set; }
        public string? Icon { get; set; }
  

    }
}
