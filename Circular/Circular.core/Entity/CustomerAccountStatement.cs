using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblCustomerAccountStatement")]

public class CustomerAccountStatement : BaseEntity
{
    public long? CustomerId { get; set; }
    public long? TransactionId { get; set; }
    public string? TransactionDetails { get; set; }
    public decimal? Amount { get; set; }
    public string? Status { get; set; }
    public string? CampaignCode { get; set; }
    public long? CollectionReqId { get; set; }
    public long? EventId { get; set; }

	public long? TicketDayId { get; set; }


	public override void ApplyKeys()
    {

    }
}
