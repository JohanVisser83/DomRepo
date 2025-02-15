using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace CircularWeb.Mapper
{
    public class EventMapper : Profile
    {
        public EventMapper()
        {

            
            CreateMap<Event, EventDTO>().ReverseMap();
            CreateMap<Event, EventsRequestDTo>().ReverseMap();  
        }

       
    }
}
