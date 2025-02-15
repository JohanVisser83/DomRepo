using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CommunityHouseMapper : Profile
    {
        public CommunityHouseMapper()
        {
            CreateMap<CommunityHouse,CommunityHouseDTO>().ReverseMap();
        }
    }
}
