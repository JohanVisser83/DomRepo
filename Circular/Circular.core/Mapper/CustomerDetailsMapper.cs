using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CustomerDetailsMapper : Profile
    {
        public CustomerDetailsMapper()
        {
            CreateMap<CustomerDetails,CustomerDetailsDTO>().ReverseMap();
            CreateMap<CustomerDetails, UserContactListDTO>().ReverseMap();
            CreateMap<CustomerDetails, CustomerDetailsBasicDTO>().ReverseMap();
            CreateMap<CustomerDetails, CommunitySignUpDTO>().ReverseMap();
        }
    }
}
