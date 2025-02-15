using AutoMapper;
using Circular.Core.Entity;
using Circular.Core.DTOs;

namespace Circular.Mapper
{
    public class MasterMapper : Profile
    {
        public MasterMapper()
        {
            CreateMap<MasterEntity, MasterDTO>().ReverseMap();
        }
    }
}
