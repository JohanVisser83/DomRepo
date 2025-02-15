using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblCustomerAuthTokens")]

public class CustomerAuthTokens : BaseEntity
{
    public long? CustomerId { get; set; }
    public string AuthToken { get; set; }
    public string? DeviceId { get; set; }


    public override void ApplyKeys()
    {

    }
}