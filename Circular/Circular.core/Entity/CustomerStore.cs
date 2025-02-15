using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblCustomerStore")]

public class CustomerStore : BaseEntity
{
    
    public string? StoreDisplayName { get; set; }
   
    public long? CommunityId { get; set; }
    
    public bool? IsTermsAndConditionsAccepted { get; set; }

    public string? DisplayImage { get; set; }


    public override void ApplyKeys()
    {

    }
}
