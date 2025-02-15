using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class MiniWalletWithdrawalRequestMapper : Profile
    {
        public MiniWalletWithdrawalRequestMapper()
        {
            CreateMap<MiniWalletWithdrawalRequest,MiniWalletWithdrawalRequestDTO>().ReverseMap();
        }
    }
}
