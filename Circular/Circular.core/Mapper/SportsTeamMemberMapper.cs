using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Mapper
{
    public class SportsTeamMemberMapper : Profile
    {
        public SportsTeamMemberMapper()
        {
            CreateMap<SportsTeamMember, SportsTeamMemberDTO>().ReverseMap();
        }
    }
}