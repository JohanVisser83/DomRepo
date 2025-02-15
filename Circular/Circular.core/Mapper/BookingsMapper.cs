using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Mapper
{
    public class BookingsMapper : Profile
    {
       public BookingsMapper()
        {
            CreateMap<Bookings, BookingsDTO>().ReverseMap();
        }
    }
}
