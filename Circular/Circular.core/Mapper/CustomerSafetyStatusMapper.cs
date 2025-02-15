using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CustomerSafetyStatusMapper : Profile
    {
        public CustomerSafetyStatusMapper()
        {
            CreateMap<CustomerSafetyStatus,CustomerSafetyStatusDTO>().ReverseMap();
        }
    }
}
