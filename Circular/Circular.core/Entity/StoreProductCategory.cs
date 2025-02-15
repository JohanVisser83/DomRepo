using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblStoreProductCategory")]

public class StoreProductCategory : BaseEntity
{
    public int Id {get;set;}
    public long? StoreId { get; set; }
 
    public string? CategoryName { get; set; }
    public string? CategoryDesc { get; set; }
    public string? Icon { get; set; }


    public override void ApplyKeys()
    {

    }
}
