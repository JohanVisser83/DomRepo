using AutoMapper;
using Circular.Core.Entity;
using Circular.Core.DTOs;

namespace Circular.Mapper
{
	public class TicketDaysMapper : Profile
	{
		public TicketDaysMapper()
		{
            CreateMap<TicketDays, TicketsDTO>().ReverseMap();
            CreateMap<TicketDays, TicketDaysDTO>().ReverseMap();
		}
	}
}
