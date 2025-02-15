using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Mapper
{
    public class SettingsMapper : Profile
    {
        public SettingsMapper() 
        {
            CreateMap<Settings, SettingsDTO>().ReverseMap();
        }
    }
}
