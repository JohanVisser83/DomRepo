using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Core.Mapper
{
    public  class PreOrderDaysSlotsMapper:Profile
    {
        public PreOrderDaysSlotsMapper() {

            CreateMap<PreOrderDaysSlot,PreOrderDaysSlotsDTO>().ReverseMap();
        }
    }
}
