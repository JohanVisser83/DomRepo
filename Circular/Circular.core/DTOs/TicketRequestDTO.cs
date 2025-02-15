namespace Circular.Core.DTOs
{
	public class TicketRequestDTO
	{
		public DateTime Date { get; set; }
		public long CommunityId { get; set; }
		public long? CustomerId { get; set; }
		public long? TicketId { get; set; }

	}
}
