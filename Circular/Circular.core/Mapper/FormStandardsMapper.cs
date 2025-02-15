using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class FormStandardsMapper : Profile
    {
        public FormStandardsMapper()
        {
            CreateMap<FormStandards,FormStandardsDTO>().ReverseMap();
        }
    }
}
