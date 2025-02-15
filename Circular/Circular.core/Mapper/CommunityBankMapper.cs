using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CommunityBankMapper : Profile
    {
        public CommunityBankMapper()
        {
            CreateMap<CommunityBank, CommunityBankDTO>().ReverseMap();
        }
    }
}
