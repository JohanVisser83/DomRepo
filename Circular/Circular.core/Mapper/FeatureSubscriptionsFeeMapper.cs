using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Core.Mapper
{
    public class FeatureSubscriptionsFeeMapper : Profile
    {
        public FeatureSubscriptionsFeeMapper()
        {
            CreateMap<FeatureSubscriptionsFee, FeatureSubscriptionFeeDTO>().ReverseMap();
            CreateMap<SelectedCommunityFeatures, SelectedCommunityFeaturesDTO>().ReverseMap();
            CreateMap<FeatureSubscriptionsFee, SelectedCommunityFeatures>().ReverseMap();

        }
    }
}
