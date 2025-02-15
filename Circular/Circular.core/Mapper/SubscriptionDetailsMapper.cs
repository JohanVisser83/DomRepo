using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Core.Mapper
{
    public  class SubscriptionDetailsMapper : Profile
    {
        public SubscriptionDetailsMapper()
        { 
          CreateMap<SubscriptionDetails, SubscriptionDetailsDTO>().ReverseMap();
        }  
    }
}
