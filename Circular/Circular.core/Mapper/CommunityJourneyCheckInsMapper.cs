using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CommunityJourneyCheckInsMapper : Profile
    {
        public CommunityJourneyCheckInsMapper()
        {
            CreateMap<CommunityJourneyCheckIns, CommunityJourneyCheckInsDTO>().ReverseMap();
        }
    }
}
