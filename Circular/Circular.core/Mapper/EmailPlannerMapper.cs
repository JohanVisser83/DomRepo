using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class EmailPlannerMapper : Profile
    {
        public EmailPlannerMapper()
        {
            CreateMap<EmailPlanner, EmailPlannerDTO>().ReverseMap();
        }
    }
}
