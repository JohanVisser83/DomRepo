using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class BankBranchesMapper : Profile
    {
        public BankBranchesMapper()
        {
            CreateMap<BankBranches, BankBranchesDTO>().ReverseMap();
        }
    }
}
