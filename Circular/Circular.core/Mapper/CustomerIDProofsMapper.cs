using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CustomerIDProofsMapper : Profile
    {
        public CustomerIDProofsMapper()
        {
            CreateMap<CustomerIDProofs, CustomerIDProofsDTO>().ReverseMap();
        }
    }
}
