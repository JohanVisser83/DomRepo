using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Core.Mapper
{
    public class StorefrontScheduleMapper:Profile
    {
        public  StorefrontScheduleMapper()
        {
            CreateMap<StorefrontSchedule,StorefrontScheduleDTO>().ReverseMap();

        }
    }
}
