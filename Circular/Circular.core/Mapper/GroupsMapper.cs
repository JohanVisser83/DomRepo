using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;



namespace Circular.Mapper
{
    public class GroupsMapper : Profile
    {
        public GroupsMapper()
        {
            CreateMap<Groups, GroupsDTO>().ReverseMap();
            //CreateMap<Groups, GroupMemberDetails>().ReverseMap();
        }
    }
}
