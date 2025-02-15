using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Mapper
{
    public class OrganizationsEmailMapper : Profile
    {
        public OrganizationsEmailMapper()
        {
            CreateMap<OrganizationsEmail,OrganizationsEmailDTO>().ReverseMap();
        }
    }
}
