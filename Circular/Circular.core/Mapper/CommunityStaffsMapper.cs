using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CommunityStaffsMapper : Profile
    {
        public CommunityStaffsMapper()
        {
            CreateMap<CommunityStaffs,CommunityStaffsDTO>().ReverseMap();
        }
    }
}
