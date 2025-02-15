using AutoMapper;
using Circular.Core.Entity;
using Circular.Core.DTOs;

namespace Circular.Mapper
{
	public class TicketsMapper : Profile
	{
		public TicketsMapper()
		{
			//CreateMap<Tickets, TicketsDTO>().ReverseMap();
			CreateMap<Tickets,TicketRequestDTO>().ReverseMap();
		}
	}
}
