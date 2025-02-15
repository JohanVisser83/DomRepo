namespace Circular.Core.DTOs
{
    public class BookingListDTO
    {
        public long? CustomerId { get; set; }
        public long? BookingId { get; set; }
        public long? CommunityId { get; set; }
        public DateTime Date { get; set; }

    }
}
