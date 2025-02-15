using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblCustomerTickets")]

public class CustomerTickets : BaseEntity
{
	public long TicketId { get; set; }
	public long UserId { get; set; }
	public long Amount { get; set; }
	public long CommunityId { get; set; }
	public bool? IsUsed { get; set; }
	public string UserName { get; set; }

	public override void ApplyKeys()
	{

	}
}
