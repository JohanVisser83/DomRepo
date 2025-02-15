using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class MerchantLegalSupportingDocsMapper : Profile
    {
        public MerchantLegalSupportingDocsMapper()
        {
            CreateMap<MerchantLegalSupportingDocs, MerchantLegalSupportingDocsDTO>().ReverseMap();
        }
    }
}
