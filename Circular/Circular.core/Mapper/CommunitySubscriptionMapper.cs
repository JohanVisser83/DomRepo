using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CommunitySubscriptionMapper : Profile
    {
        public CommunitySubscriptionMapper()
        {
            CreateMap<CommunitySubscription,CommunitySubscriptionDTO>().ReverseMap();
        }
    }
}
