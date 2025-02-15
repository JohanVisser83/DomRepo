using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblSubscriptionTest")]

public class SubscriptionTest : BaseEntity
{
    public long? EstimatedUsers { get; set; }
    public decimal? Price { get; set; }
    public decimal? EstimatedMonthlyCost { get; set; }
    public long? Discount { get; set; }
    public decimal? CustomerMonthlyFee { get; set; }
    public string? Partner { get; set; }
    public long? PartnerRebatePercent { get; set; }
    public decimal? PartnerRebate { get; set; }
    public decimal? CircularFinalInvoice { get; set; }
    public decimal? AnnualCommunityEarning { get; set; }
    public long? ProductId { get; set; }



    public override void ApplyKeys()
    {

    }
}
