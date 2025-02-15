using AutoMapper;
using Circular.Core.Entity;
using Circular.Core.DTOs;

namespace Circular.Core.Mapper
{
    public  class PreOrderTimeSlotsMapper:Profile
    {
        public PreOrderTimeSlotsMapper() { 

            CreateMap<PreOrderTimeSlots , PreOrderTimeSlotsDTO>().ReverseMap();
        }    
    }
}
