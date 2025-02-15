using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CommunityJourneyMapper : Profile
    {
        public CommunityJourneyMapper()
        {
            CreateMap<CommunityJourney, CommunityJourneyDTO>().ReverseMap();
        }
    }
}
