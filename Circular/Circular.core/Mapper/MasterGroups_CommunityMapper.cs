using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class MasterGroups_CommunityMapper : Profile
    {
        public MasterGroups_CommunityMapper()
        {
            CreateMap<MasterGroups_Community, MasterGroups_CommunityDTO>().ReverseMap();
        }
    }
}
