using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Mapper
{
    public class ProductsMapper : Profile
    {
        public ProductsMapper()
        {
            CreateMap<Products,ProductsDTO>().ReverseMap();
        }
    }
}
