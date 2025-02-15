using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class VehiclesMapper : Profile
    {
        public VehiclesMapper()
        {
            CreateMap<Vehicles, VehiclesDTO>().ReverseMap();
            CreateMap<Vehicles, QRCodeModelDTO>().ReverseMap();
            CreateMap<Vehicles, DeleteVecDTO>().ReverseMap();
        }
    }
}
