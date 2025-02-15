using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class TransactionItemsMapper : Profile
    {
        public TransactionItemsMapper()
        {
            CreateMap<TransactionItems,TransactionItemsDTO>().ReverseMap();
        }
    }
}
