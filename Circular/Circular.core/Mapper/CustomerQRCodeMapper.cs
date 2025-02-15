using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CustomerQRCodeMapper : Profile
    {
        public CustomerQRCodeMapper()
        {
            CreateMap<CustomerQRCode, CustomerQRCodeDTO>().ReverseMap();
        }
    }
}
