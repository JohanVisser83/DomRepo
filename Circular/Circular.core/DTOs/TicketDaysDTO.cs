namespace Circular.Core.DTOs
{
	public class TicketDaysDTO
	{
		public string? TicketName { get; set; }
		public long? TicketPrice { get; set; }
		public long? TicketCount { get; set; }
		public DateTime? TicketTime { get; set; }

		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public long? CommunityId { get; set; }

		public long? MasterTicketId { get; set; }
	}
}
