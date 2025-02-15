using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblTicketDays")]
public class TicketDays : BaseEntity
{
	public TicketDays()
	{
		TicketQR = new QR();
	}
	public string? TicketName { get; set; }
	public decimal? TicketPrice { get; set; }
	public long? TicketCount { get; set; }
	public DateTime TicketTime { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	public long? CommunityId { get; set; }
	public long? MasterTicketId { get; set; }
	public string? CommunityName { get; set; }
	public int? BookedIn { get; set; }
	public long? CountOfBooking { get; set; }
	public int? IsFull { get; set; }
	public long? CustomerTicketId { get; set; }
	public QR TicketQR { get; set; }

	public long? CustomerId { get; set; }

	public int? IsClaimed { get; set; }

	public DateTime? PurchaseDate { get; set; }

	public string StartDateTime { get; set; }

	public long? SoldCount { get; set; }


	public override void ApplyKeys()
	{

	}
}


