using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class UserDevicesMapper : Profile
    {
        public UserDevicesMapper()
        {
            CreateMap<CustomerDevices, CustomerDevicesDTO>().ReverseMap();
        }
    }
}
