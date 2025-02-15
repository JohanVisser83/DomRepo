using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Core.Mapper
{
    public class AffiliateCodeMapper : Profile
    {
        public AffiliateCodeMapper()
        {
            CreateMap<AffiliatedCodeDetails, AffiliatedCodeDTO>().ReverseMap();
        }
    }
}
