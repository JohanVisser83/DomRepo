using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CustomerAuthTokensMapper : Profile
    {
        public CustomerAuthTokensMapper()
        {
            CreateMap<CustomerAuthTokens,CustomerAuthTokensDTO>().ReverseMap();
        }
    }
}
