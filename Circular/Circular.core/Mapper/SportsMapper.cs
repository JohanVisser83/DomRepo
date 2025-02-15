
using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Mapper
{
    public class SportsMapper : Profile
    {
        public SportsMapper()
        {
            CreateMap<Sports, SportsDTO>().ReverseMap();
        }
    }
}
