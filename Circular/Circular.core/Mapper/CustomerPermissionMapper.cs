using AutoMapper;
using Circular.Core.Entity;
using Circular.Core.DTOs;

namespace Circular.Mapper
{
    public class CustomerPermissionMapper : Profile
    {


        public CustomerPermissionMapper()
        {
            CreateMap<CustomerPermission, CustomerPermissionDTO>().ReverseMap();
        }
    }
}
