namespace Circular.Core.DTOs
{
    public class CommunityJourneyCheckInsDTO
    {
        public long? JourneyId { get; set; }
        public long? CustomerId { get; set; }
        public decimal CustLang { get; set; }
        public decimal CustLong { get; set; }
    }
}
