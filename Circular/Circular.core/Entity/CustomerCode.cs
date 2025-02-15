using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblCustomerCode")]

public class CustomerCode : BaseEntity
{
    public long CustomerCodeSeq { get; set; }


    public override void ApplyKeys()
    {

    }
}
