using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CustomerWithdrawalRequestMapper : Profile
    {
        public CustomerWithdrawalRequestMapper()
        {
            CreateMap<CustomerWithdrawalRequest, CustomerWithdrawalRequestDTO>().ReverseMap();
        }
    }
}
