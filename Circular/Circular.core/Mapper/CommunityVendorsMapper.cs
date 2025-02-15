using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CommunityVendorsMappern : Profile
    {
        public CommunityVendorsMappern()
        {
            CreateMap<CommunityVendors,CommunityVendorsDTO>().ReverseMap();
        }
    }
}
