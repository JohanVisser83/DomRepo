using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class SuburbMapper : Profile
    {
        public SuburbMapper()
        {
            CreateMap<Suburb,SuburbDTO>().ReverseMap();
        }
    }
}
