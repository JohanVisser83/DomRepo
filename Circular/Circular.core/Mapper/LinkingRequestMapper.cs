using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class LinkingRequestMapper : Profile
    {
        public LinkingRequestMapper()
        {
            CreateMap<LinkedMembers,LinkedMembersDTO>().ReverseMap();
        }
    }
}
