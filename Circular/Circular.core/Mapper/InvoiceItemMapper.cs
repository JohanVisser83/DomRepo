using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;



namespace Circular.Mapper
{
    public class InvoiceItemMapper : Profile
    {
        public InvoiceItemMapper()
        {
            CreateMap<InvoiceItem, InvoiceItemDTO>().ReverseMap();
        }
    }
}
