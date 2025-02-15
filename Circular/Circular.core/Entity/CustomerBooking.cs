using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblCustomerBooking")]
public class CustomerBooking : BaseEntity
{
    public long? BookingId { get; set; }
    public long? UserId { get; set; }
    public bool? IsBookingStatus { get; set; }

    public override void ApplyKeys()
    {

    }
}
