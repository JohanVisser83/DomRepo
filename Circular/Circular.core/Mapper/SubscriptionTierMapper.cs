using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Core.Mapper
{
    public class SubscriptionTierMapper : Profile
    {
        public SubscriptionTierMapper()
        {
         CreateMap<SubscriptionTier,  SubscriptionTierDTO>().ReverseMap();
        } 
    }
}
