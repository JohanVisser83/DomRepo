using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Core.Mapper
{
    public class CommunityTierFeaturesMapper : Profile
    {
        public CommunityTierFeaturesMapper()
        { 
           CreateMap<CommunityTierFeatures, CommunityTierFeaturesDTO>().ReverseMap();
        }    
    }
}
