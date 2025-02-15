using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblCustomerPaymentGatewayDetails")]

public class CustomerPaymentGatewayDetails : BaseEntity
{
    public long CustomerId { get; set; }
    public string StripeCustomerId { get; set; }

    public override void ApplyKeys()
    {

    }
}
