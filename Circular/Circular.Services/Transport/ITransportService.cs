using Circular.Core.DTOs;
using Circular.Core.Entity;
namespace Circular.Services.Transport
{
	public interface ITransportService
	{
		Task<List<TicketDays>> GetTicketsAsync(long CommunityId, long? CustomerId,DateTime StartDate, long? TicketId);
		Task<int> BuyTicket(long TicketDayId, long customerId);
		Task<int> BuyFlexipass(FlexipassBuyRequest flexiRequest);
		Task<List<Vehicles>> GetTransportVehciles(long CommunityId);

	}
}
