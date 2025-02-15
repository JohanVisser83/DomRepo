using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CollectionAggregateMapper : Profile
    {
        public CollectionAggregateMapper()
        {
            CreateMap<CollectionAggregate, CollectionAggregateDTO>().ReverseMap();
        }
    }
}
