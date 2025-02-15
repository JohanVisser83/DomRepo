using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;



namespace Circular.Mapper
{
    public class CommunityTransportPassMapper : Profile
    {
        public CommunityTransportPassMapper()
        {
            CreateMap<CommunityTransportPass,CommunityTransportPassDTO>().ReverseMap();
            CreateMap<CommunityTransportPass, PriceDisplayQRCodeModelDTO>().ReverseMap();
			CreateMap <CommunityTransportPass,DeleteTransDTO>().ReverseMap();

		}
    }
}
