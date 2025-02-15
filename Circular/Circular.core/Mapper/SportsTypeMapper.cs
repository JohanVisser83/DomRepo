using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Mapper
{
    public  class SportsTypeMapper: Profile
    {
        public SportsTypeMapper()
        { 
            CreateMap<SportsType, SportsTypeDTO>().ReverseMap();
        }
    }
}
