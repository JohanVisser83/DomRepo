using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class OrderDetailsMapper : Profile
    {
        public OrderDetailsMapper()
        {
            CreateMap<OrderDetails, OrderDetailsDTO>().ReverseMap();
        }
    }
}
