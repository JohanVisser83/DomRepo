using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class customerBookingMapper : Profile
    {
        public customerBookingMapper()
        {
            CreateMap<CustomerBookingDTO,CustomerBooking>().ReverseMap();
        }
    }
}
