using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Core.Mapper
{
    public class CustomerStoreFrontAccessMapper : Profile
    {
        public CustomerStoreFrontAccessMapper()
        { 
          CreateMap<CustomerStoreFrontAccess, CustomerStoreFrontAccessDTO>().ReverseMap();
          CreateMap<CustomerStoreFrontAccess, AddPermissionDTO>().ReverseMap();
        }
    }
}
