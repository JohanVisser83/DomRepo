using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class TransactionsMapper : Profile
    {
        public TransactionsMapper()
        {
            CreateMap<Transactions,TransactionsDTO>().ReverseMap();
            CreateMap<TransactionRequest, TransactionRequestDTO>().ReverseMap();
        }

    }
}
