using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class SponsorSectionDetailsMapper : Profile
    {
        public SponsorSectionDetailsMapper()
        {
            CreateMap<SponsorSectionDetails, SponsorSectionDetailsDTO>().ReverseMap();
        }
    }
}
