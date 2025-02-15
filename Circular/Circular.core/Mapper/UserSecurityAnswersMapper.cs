using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class UserSecurityAnswersMapper : Profile
    {
        public UserSecurityAnswersMapper()
        {
            CreateMap<UserSecurityAnswers,UserSecurityAnswersDTO>().ReverseMap();
        }
    }
}
