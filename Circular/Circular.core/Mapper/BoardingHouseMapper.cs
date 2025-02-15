using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class BoardingHouseMapper : Profile
    {
        public BoardingHouseMapper()
        {
            CreateMap<BoardingHouses, BoardingHousesDTO>().ReverseMap();
        }
    }
}
