using RepoDb.Attributes;
using static System.Net.Mime.MediaTypeNames;

namespace Circular.Core.Entity;

[Map("tblProducts")]

public class Products : BaseEntity
{
    public Products()
    {
        if (Images == null)
            Images = new List<StoreProductImages>();
    }
    public long Id { get; set; }
    public long? CustomerId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductDesc { get; set; }
    
    public decimal? SellingPrice { get; set; }
    public string? ProductImage { get; set; }
    public long? StoreId { get; set; }
    public long? CategoryId { get; set; }
    
    public long? Quantity { get; set; }
    public long? Threshold { get; set; }
    public string? ProductUniqueID { get; set; }
    public List<StoreProductImages> Images { get; set; }

    public override void ApplyKeys()
    {

    }
}