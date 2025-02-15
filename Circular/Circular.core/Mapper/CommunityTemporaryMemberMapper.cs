using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Core.Mapper
{
    public class CommunityTemporaryMemberMapper : Profile
    {
        public CommunityTemporaryMemberMapper()
        {
            CreateMap<CommunityTemporaryMember, CommunityTemporaryMemberDTO>().ReverseMap();
        }
    }
}
