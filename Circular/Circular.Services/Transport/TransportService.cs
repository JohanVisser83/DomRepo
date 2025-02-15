using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Data.Repositories.Transport;
namespace Circular.Services.Transport
{
	public class TransportService : ITransportService
	{
		private readonly ITransportRepository _transportRepository;
		public TransportService(ITransportRepository transportrepository)
		{
			_transportRepository = transportrepository;
		}
		
		public async Task<List<TicketDays>> GetTicketsAsync(long CommunityId, long? CustomerId, DateTime StartDate, long? TicketId)
		{
			return await _transportRepository.GetTicketsAsync(CommunityId, CustomerId??0, StartDate,TicketId??0);
		}

		public async Task<int> BuyTicket(long TicketDayId, long customerId)
		{
            return await _transportRepository.BuyTicket(TicketDayId, customerId);
		}
		public async Task<int> BuyFlexipass(FlexipassBuyRequest flexiRequest)
		{
			return await _transportRepository.BuyFlexipass(flexiRequest);
		}

		public async Task<List<Vehicles>> GetTransportVehciles(long CommmunityId)
		{
			return await _transportRepository.GetTransportVehciles(CommmunityId);

        }


    }
}