using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Mapper
{
    public class MerchantAddressAndLegalInfoMapper : Profile
    {
        public MerchantAddressAndLegalInfoMapper()
        {
            CreateMap<MerchantAddressAndLegalInfo,MerchantAddressAndLegalInfoDTO>().ReverseMap();
        }
    }
}
