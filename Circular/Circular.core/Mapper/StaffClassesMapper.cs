using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class StaffClassesMapper : Profile
    {
        public StaffClassesMapper()
        {
            CreateMap<StaffClasses,StaffClassesDTO>().ReverseMap();
        }
    }
}
