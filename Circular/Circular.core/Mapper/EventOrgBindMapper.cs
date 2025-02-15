using AutoMapper;
using Circular.Core.Entity;
using Circular.Core.DTOs;
namespace Circular.Mapper
{
    public class EventOrgBindMapper : Profile
    {
        public EventOrgBindMapper() 
        {
            CreateMap<EventOrgBind, EventOrgBindDTO>().ReverseMap();
            CreateMap<EventOrgBind, UpdatePortalUserDTO>().ReverseMap();
        }
    }
}
