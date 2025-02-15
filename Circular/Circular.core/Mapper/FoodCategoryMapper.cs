using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;



namespace Circular.Mapper
{
    public class FoodCategoryMapper : Profile
    {
        public FoodCategoryMapper()
        {
            CreateMap<FoodCategory,FoodCategoryDTO>().ReverseMap();
        }
    }
}
