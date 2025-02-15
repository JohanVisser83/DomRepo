using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class SponsorSectionImagesMapper : Profile
    {
        public SponsorSectionImagesMapper()
        {
            CreateMap<SponsorSectionImages,SponsorSectionImagesDTO>().ReverseMap();
        }
    }
}
