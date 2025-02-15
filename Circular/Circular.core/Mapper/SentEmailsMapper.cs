using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class SentEmailsMapper : Profile
    {
        public SentEmailsMapper()
        {
            CreateMap<SentEmails,SentEmailsDTO>().ReverseMap();
        }
    }
}
