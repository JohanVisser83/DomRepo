using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Core.Mapper
{
    public class SubscriptionBillingMapper : Profile
    {
        public SubscriptionBillingMapper() 
        { 
         CreateMap<SubscriptionBilling, SubscriptionBillingDTO>().ReverseMap();
        }
    }
}
