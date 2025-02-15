using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Core.Mapper
{
    public  class SubscriptionFeaturesSelectedPlanMapper : Profile
    {
        public SubscriptionFeaturesSelectedPlanMapper()
        {
          CreateMap<SubscriptionFeaturesSelectedPlan, SubscriptionFeaturesSelectedPlanDTO>().ReverseMap();
          //CreateMap<SelectedFeatures, SelectedFeaturesDTO>().ReverseMap();
            
        }
    }
}
