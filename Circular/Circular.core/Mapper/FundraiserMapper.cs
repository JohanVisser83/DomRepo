using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;



namespace Circular.Mapper
{
    public class FundraiserMapper : Profile
    {
        public FundraiserMapper()
        {
            CreateMap<Fundraiser, FundraiserDTO>().ReverseMap();
        }
    }
}