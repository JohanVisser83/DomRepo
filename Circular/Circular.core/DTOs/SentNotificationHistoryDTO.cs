namespace Circular.Core.DTOs
{
    public class SentNotificationHistoryDTO
    {
        public string SentTo { get; set; }
        public string? Message { get; set; }
        public string? CampaignCode { get; set; }
        public string? Desc { get; set; }
        public long? SentFrom { get; set; }
    }
}
