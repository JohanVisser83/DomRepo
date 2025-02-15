using AutoMapper;
using Circular.Core.DTOs;


namespace Circular.Core.Mapper
{
    public  class AffiliateCodeMappingMapper : Profile
    {
        public AffiliateCodeMappingMapper() 
        {
          CreateMap<AffiliateCodeMapping, AffiliateCodeMappingDTO>().ReverseMap();
        }
    }
}
