using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class BookingDaysMapper : Profile
    {
        public BookingDaysMapper()
        {
            CreateMap<BookingDays, BookingDaysDTO>().ReverseMap();
        }
    }
}
