using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Core.Mapper
{
    public class AdvertisementMediaMapper : Profile
    {
        public AdvertisementMediaMapper() 
        { 
         CreateMap<AdvertisementMedia, AdvertisementMediaDTO>().ReverseMap();
        }
    }
}
