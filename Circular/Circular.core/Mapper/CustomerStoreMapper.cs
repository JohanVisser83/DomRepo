using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CustomerStoreMapper : Profile
    {
        public CustomerStoreMapper()
        {
            CreateMap<CustomerStore,CustomerStoreDTO>().ReverseMap();
        }
    }
}
