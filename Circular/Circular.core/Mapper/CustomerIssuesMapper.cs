using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CustomerIssuesMapper : Profile
    {
        public CustomerIssuesMapper()
        {
            CreateMap<CustomerIssues, CustomerIssuesDTO>().ReverseMap();
        }
    }
}
