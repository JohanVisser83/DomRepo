using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Core.Mapper
{
    public class CommunityMembershipTypeMapper : Profile
    {
        public CommunityMembershipTypeMapper()
        { 
        
            CreateMap<CommunityMembershipType, CommunityMembershipTypeDTO>().ReverseMap();
        
        }  
    }
}
