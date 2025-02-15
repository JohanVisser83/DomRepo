using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Core.Mapper
{
    public class AdminFeatureMapper : Profile
    {

        public AdminFeatureMapper()
        { 
            CreateMap<AdminFeature, AdminFeatureDTO>().ReverseMap();
            CreateMap<AdminFeature, AddPermissionDTO>().ReverseMap();
        }

    }
}
