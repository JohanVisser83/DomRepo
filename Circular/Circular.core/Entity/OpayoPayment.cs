using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblOpayoPayment")]

public class OpayoPayment : BaseEntity
{
    public long? CustomerId { get; set; }
    public string? UniqueRefId { get; set; }
    public string? TransactionId { get; set; }
    public decimal? Amount { get; set; }
    public string? RequestPayload { get; set; }
    public string? Status { get; set; }


    public override void ApplyKeys()
    {

    }
}
