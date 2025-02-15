using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class AdminBankAccountsMapper : Profile
    {
        public AdminBankAccountsMapper()
        {
            CreateMap<AdminConfigurations, AdminConfigurationsDTO>().ReverseMap();
        }
    }
}
