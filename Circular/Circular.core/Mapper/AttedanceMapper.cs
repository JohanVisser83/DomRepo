using Circular.Core.DTOs;
using Circular.Core.Entity;
using AutoMapper;

namespace Circular.Mapper
{
    public class AttedanceMapper : Profile
    {
        public AttedanceMapper()
        {
            CreateMap<Attendance,AttendanceDTO>().ReverseMap();
        }
    }
}
