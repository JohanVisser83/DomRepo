using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class stockInventoryMapper : Profile
    {
        public stockInventoryMapper()
        {
            CreateMap<stockInventory,stockInventoryDTO>().ReverseMap();
        }
    }
}
