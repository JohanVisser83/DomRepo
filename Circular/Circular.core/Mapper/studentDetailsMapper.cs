using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class studentDetailsMapper : Profile
    { 
        public studentDetailsMapper()
        {
            CreateMap<studentDetails,studentDetailsDTO>().ReverseMap();
        }
    }
}
