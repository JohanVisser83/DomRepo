using AutoMapper;
using Circular.Core.Entity;
using Circular.Core.DTOs;

namespace Circular.Mapper
{
    public class MasterPermissionMapper : Profile
    {
        public MasterPermissionMapper()
        {
            CreateMap<MasterPermission, MasterPermissionDTO>().ReverseMap();
        }
    }
}
