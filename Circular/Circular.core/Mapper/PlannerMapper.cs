using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class PlannerMapper : Profile
    {
        public PlannerMapper()
        {
            CreateMap<Planner,PlannerDTO>().ReverseMap();
            CreateMap<Planner, PlannerAddDTO>().ReverseMap();
        }
    }
}
