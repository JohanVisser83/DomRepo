using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class VouchersMapper : Profile
    {
        public VouchersMapper()
        {
            CreateMap<Vouchers,VouchersDTO>().ReverseMap();
        }
    }
}
