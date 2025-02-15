using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblTransactionItems")]

public class TransactionItems : BaseEntity
{
    public long? TransactionId { get; set; }
    public long? ProductId { get; set; }
    public decimal? ProductQty { get; set; }
    public decimal? ProductAmount { get; set; }

    public override void ApplyKeys()
    {

    }
}
