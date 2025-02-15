namespace Circular.Core.DTOs
{
	public class CustomerTicketsDTO
	{
		public long TicketId { get; set; }
		public long UserId { get; set; }
		public long Amount { get; set; }
		public long CommunityId { get; set; }
		public bool? IsUsed { get; set; }
		public string UserName { get; set; }


	}
	public class TicketBuyRequest
	{
		public long TicketDayId { get; set; }
		public long CustomerId { get; set; }
	}

	public class FlexipassBuyRequest
	{
		public long CommunityId { get; set; }
		public long CustomerId { get; set; }
		public long LoggedInCustomerId { get; set; }
		public decimal Amount { get; set; }
		public decimal distance { get; set; }

	}
}


