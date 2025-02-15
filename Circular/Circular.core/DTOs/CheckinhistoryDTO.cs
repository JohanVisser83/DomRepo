namespace Circular.Core.DTOs
{
    public class CheckinhistoryDTO
    {
        public long? CustomerId { get; set; }
        public DateTime? Date { get; set; }
        public TimeOnly? Timespan { get; set; }
        public decimal? Amount { get; set; }
        public string? Scannedfor { get; set; }
        public decimal? Distance { get; set; }
        public bool? IsFlexiPass { get; set; }
    }
}
