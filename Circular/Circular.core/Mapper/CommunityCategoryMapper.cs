using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Core.Mapper
{
    public  class CommunityCategoryMapper : Profile
    {
        public CommunityCategoryMapper()
        { 
        
            CreateMap<CommunityCategory, CommunityCategoryDTO>().ReverseMap();  
        
        }
    }
}
