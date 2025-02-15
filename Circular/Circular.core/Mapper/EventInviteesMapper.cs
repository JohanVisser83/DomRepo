using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;



namespace Circular.Mapper
{
    public class EventInviteesMapper : Profile
    {
        public EventInviteesMapper()
        {
            CreateMap<EventInvitees, EventInviteesDTO>().ReverseMap();
        }
    }
}
