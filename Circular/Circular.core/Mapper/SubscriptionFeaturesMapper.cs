using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class SubscriptionFeaturesMapper : Profile
    {
        public SubscriptionFeaturesMapper() 
        {
            CreateMap<SubscriptionFeatures, SubscriptionFeaturesDTO>().ReverseMap();
        }
    }
}
