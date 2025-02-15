using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblCustomerSupportlevel")]



public class CustomerSupports : BaseEntity
{
    public string Code { get; set; }

    public long CommunityId { get; set; }

    public long CustomerId { get; set; }

    public override void ApplyKeys()
    {

    }

}

