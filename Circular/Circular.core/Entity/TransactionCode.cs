using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblTransactionCode")]

public class TransactionCode : BaseEntity
{
    public long TransactionCodeSeq { get; set; }


    public override void ApplyKeys()
    {

    }
}
