using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblCustomerSafetyStatus")]

public class CustomerSafetyStatus : BaseEntity
{
    public long? CustomerId { get; set; }
    public DateTime Date { get; set; }
    public bool IsSafe { get; set; }



    public override void ApplyKeys()
    {

    }
}
