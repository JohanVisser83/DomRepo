using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Core.Mapper
{
    public class AdvertisementMapper : Profile
    {
        public AdvertisementMapper()
        {
            CreateMap<Advertisement, AdvertisementDTOs>().ReverseMap();
        }
    }
}
