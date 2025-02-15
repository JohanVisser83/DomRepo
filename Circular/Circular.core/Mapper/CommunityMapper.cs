using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CommunityMapper : Profile
    {
        public CommunityMapper()
        {
            CreateMap<Communities, CommunityDTO>().ReverseMap();
			CreateMap<Communities , priceperkmrequestDTO>().ReverseMap();
            CreateMap<Communities ,CommunitiesAlertDTO>().ReverseMap();
			CreateMap<Communities ,CommunitiesReportDTO>().ReverseMap();
            CreateMap<Communities, QRAttendanceScannerDTO>().ReverseMap();



        }
    }
}
