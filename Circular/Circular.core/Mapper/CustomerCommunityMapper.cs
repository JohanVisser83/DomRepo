using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CustomerCommunityMapper : Profile
    {
        public CustomerCommunityMapper()
        {
            CreateMap<CustomerCommunity,CustomerCommunityDTO>().ReverseMap();
            CreateMap<CustomerCommunity, CustomerCommunityRequestDTO>().ReverseMap();
            CreateMap<CustomerCommunity, CommunityAccessDTO>().ReverseMap();
            CreateMap<CustomerCommunity, JoinCommunityDTO>().ReverseMap();

        }
    }
}
