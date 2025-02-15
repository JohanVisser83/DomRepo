using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class SubscriptionTestMapper : Profile
    {
        public SubscriptionTestMapper()
        {
            CreateMap<SubscriptionTest, SubscriptionTestDTO>().ReverseMap();
        }
    }
}
