using RepoDb.Attributes;
using static System.Net.Mime.MediaTypeNames;

namespace Circular.Core.Entity;

[Map("tblStockInventory")]

public class stockInventory : BaseEntity
{
    public stockInventory()
    {
        if (Images == null)
            Images = new List<StoreProductImages>();
    }


    public string? Product { get; set; }
    public long? CategoryId { get; set; }
    public string? ProductId { get; set; }
    public long? StoreId { get; set; }
    public long? Threshold { get; set; }
    public string? EntryStatus { get; set; }
    public decimal? Productcost { get; set; }
    public string? ProductImage {get; set; }

    public int? Quantity { get; set; }
    public string? categoryName { get; set; }

    public List<StoreProductImages> Images { get; set; }

    public override void ApplyKeys()
    {

    }
}
