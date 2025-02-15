using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;



namespace Circular.Mapper
{
    public class CustomerGroupsMapper : Profile
    {
        public CustomerGroupsMapper()
        {
            CreateMap<CustomerGroups,CustomerGroupsDTO>().ReverseMap();
        }
    }
}
