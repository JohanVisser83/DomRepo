using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CustomerBusinessIndexMapper : Profile
    {
        public CustomerBusinessIndexMapper()
        {
            CreateMap<CustomerBusinessIndex, CustomerBusinessIndexDTO>().ReverseMap();
        }
    }
}
