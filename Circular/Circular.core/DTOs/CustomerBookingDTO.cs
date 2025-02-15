namespace Circular.Core.DTOs
{
    public class CustomerBookingDTO : BaseEntityDTO
    {
        public long? BookingId { get; set; }
        public long? UserId { get; set; }
        public bool? IsBookingStatus { get; set; }
    }
}
