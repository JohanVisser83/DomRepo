using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class AddCartMapper : Profile
    {
        public AddCartMapper()
        {
            CreateMap<AddCart, AddCartDTO>().ReverseMap();
        }
    }
}
