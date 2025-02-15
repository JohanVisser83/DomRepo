using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class SponsorInformationMapper : Profile
    {
        public SponsorInformationMapper()
        {
            CreateMap<SponsorInformation, SponsorInformationDTO>().ReverseMap();
        }
    }
}
