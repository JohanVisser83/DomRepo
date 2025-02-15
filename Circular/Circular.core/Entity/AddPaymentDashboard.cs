using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("AddPaymentDashboard")]

public class AddPaymentDashboard : BaseEntity
{
    public long CustomerId { get; set; }
    public long EventId { get; set; }
    public decimal Amount { get; set; }


    public override void ApplyKeys()
    {

    }
}