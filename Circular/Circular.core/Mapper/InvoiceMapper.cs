using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class InvoiceMapper : Profile
    {
        public InvoiceMapper()
        {
            CreateMap<Invoice,InvoiceDTO>().ReverseMap();
        }
    }
}
