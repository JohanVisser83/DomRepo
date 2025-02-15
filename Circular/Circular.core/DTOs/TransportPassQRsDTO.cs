namespace Circular.Core.DTOs
{
    public class TransportPassQRsDTO
    {
        public long? CommunityId { get; set; }
        public string? Title { get; set; }
        public decimal? Price { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? QR { get; set; }
        public bool? IsPass { get; set; }
    }
}
