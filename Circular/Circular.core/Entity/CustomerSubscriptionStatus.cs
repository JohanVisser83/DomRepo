using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblCustomerSubscriptionStatus")]

public class CustomerSubscriptionStatus : BaseEntity
{
    public long id { get; set; }
    public string Name { get; set; }

    public string? Desc { get; set; }

    public string? Code { get; set; }


    public override void ApplyKeys()
    {

    }
}
