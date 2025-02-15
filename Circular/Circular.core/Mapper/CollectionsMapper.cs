using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Mapper
{
    public class CollectionsMapper : Profile
    {
        public CollectionsMapper()
        {
            CreateMap<Collections, CollectionsDTO>().ReverseMap();
        }
    }
}
