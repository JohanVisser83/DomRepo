using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class LoginHistoryMapper : Profile
    {
        public LoginHistoryMapper()
        {
            CreateMap<LoginHistory,LoginHistoryDTO>().ReverseMap();
        }
    }
}
