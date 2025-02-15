using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CustomerAccountStatementMapper : Profile
    {
        public CustomerAccountStatementMapper()
        {
            CreateMap<CustomerAccountStatement,CustomerAccountStatementDTO_>().ReverseMap();
        }
    }
}
