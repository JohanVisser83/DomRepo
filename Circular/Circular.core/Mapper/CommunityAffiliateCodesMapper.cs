using AutoMapper;
using Circular.Core.DTOs;

namespace Circular.Core.Mapper
{
    public  class CommunityAffiliateCodesMapper : Profile
    {
        public CommunityAffiliateCodesMapper() 
        { 
            CreateMap<CommunityAffiliateCodes, CommunityAffiliateCodesDTO>().ReverseMap();
        
        }
    }
}
