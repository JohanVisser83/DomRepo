using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Mapper
{
    public class StoreProductCategoryMapper : Profile
    {
        public StoreProductCategoryMapper()
        {
            CreateMap<StoreProductCategory,StoreProductCategoryDTO>().ReverseMap();
        }
    }
}
