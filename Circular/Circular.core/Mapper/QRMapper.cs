using Circular.Core.DTOs;
using Circular.Core.Entity;
using AutoMapper;
namespace Circular.Core.Mapper
{
    public class QRMapper : Profile
    {
        public QRMapper()
        {
            CreateMap<QR, QRDTO>().ReverseMap();
        }
    }
}
