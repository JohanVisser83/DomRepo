using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;



namespace Circular.Mapper
{
    public class BroadcastMapper : Profile
    {
        public BroadcastMapper()
        {
            CreateMap<Broadcast, BroadcastDTO>().ReverseMap();
            CreateMap<Broadcast, PostBroadcast>().ReverseMap();
        }
    }
}
