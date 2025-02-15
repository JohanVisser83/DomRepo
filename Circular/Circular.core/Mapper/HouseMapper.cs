using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class HouseMapper : Profile
    {
        public HouseMapper()
        {
            CreateMap<House, HouseDTO>().ReverseMap();
        }
    }
}
