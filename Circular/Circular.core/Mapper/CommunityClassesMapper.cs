using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CommunityClassesMapper : Profile
    {
        public CommunityClassesMapper()
        {
            CreateMap<CommunityClasses, CommunityClassesDTO>().ReverseMap();
			CreateMap <CommunityClasses,AttendanceRegistryRequestDTO>().ReverseMap();
			//CreateMap <CommunityClasses,attendanceRegistryResponseDTO>().ReverseMap();


		}
    }
}
