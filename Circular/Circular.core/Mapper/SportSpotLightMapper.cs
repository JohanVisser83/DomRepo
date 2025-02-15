using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Mapper
{
    public class SportSpotLightMapper : Profile
    {
        public SportSpotLightMapper() 
        {
            CreateMap<SportFixture, SportFixtureDTO>().ReverseMap();
            CreateMap<SportFixture, ManageFixtureDTO>().ReverseMap();
        }
    }
}
