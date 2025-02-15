using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;



namespace Circular.Mapper
{
    public class TransportPassQRsMapper : Profile
    {
        public TransportPassQRsMapper()
        {
            CreateMap<TransportPassQRs,TransportPassQRsDTO>().ReverseMap();
        }
    }
}
