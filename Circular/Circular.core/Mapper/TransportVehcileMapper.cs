using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Mapper
{
    public class TransportVehcileMapper : Profile
    {
        public TransportVehcileMapper()
        {
            CreateMap<TransportVehcile,TransportVehcileDTO>().ReverseMap();
            CreateMap<TransportVehcile, modalforstatusDTO>().ReverseMap();
           // CreateMap<TransportVehcile, TravelStatusDetailsDTO>().ReverseMap();
			
		}
    }
}
