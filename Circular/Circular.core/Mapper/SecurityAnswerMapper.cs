using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class SecurityAnswerMapper : Profile
    {
        public SecurityAnswerMapper()
        {
            CreateMap<SecurityAnswer,SecurityAnswerDTO>().ReverseMap();
        }
    }
}
