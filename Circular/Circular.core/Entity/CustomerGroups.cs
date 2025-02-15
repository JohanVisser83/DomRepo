using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblCustomerGroups")]

public class CustomerGroups : BaseEntity
{
    public long? CustomerId { get; set; }
    public long? GroupId { get; set; }

    public string? GroupName { get; set; }


    public override void ApplyKeys()
    {

    }
}
