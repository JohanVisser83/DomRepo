
using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CommunitySignUpMapper : Profile
    {
        public CommunitySignUpMapper()
        {
            CreateMap<CommunitySignUp, CommunitySignUpDTO>().ReverseMap();
        }
    }
}