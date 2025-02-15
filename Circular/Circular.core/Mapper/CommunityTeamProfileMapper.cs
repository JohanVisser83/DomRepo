using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Core.Mapper
{
    public class CommunityTeamProfileMapper : Profile
    {
        public CommunityTeamProfileMapper() 
        {
            CreateMap<CommunityTeamProfile, CommunityTeamProfileDTO>().ReverseMap();
        }
    }
}

