namespace Circular.Core.DTOs
{
    public class CommunitySubscriptionDTO
    {
        public long CommunityId { get; set; }
        public long SubscriptionStatusId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? EstimatedUsers { get; set; }
        public decimal? Price { get; set; }
        public decimal? EstimatedMonthlyCost { get; set; }
        public long? Discount { get; set; }
        public decimal? CustomerMonthlyFee { get; set; }
        public string? Partner { get; set; }
        public decimal? PartnerRebatePercent { get; set; }
        public decimal? PartnerRebate { get; set; }
        public decimal? CircularFinalInvoice { get; set; }
        public decimal? AnnualCommunityEarning { get; set; }
        public long? ProductId { get; set; }
    }
}
