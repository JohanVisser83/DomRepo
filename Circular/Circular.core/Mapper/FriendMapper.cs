using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;



namespace Circular.Mapper
{
    public class FriendMapper : Profile
    {
        public FriendMapper()
        {
            CreateMap<Friend,FriendDTO>().ReverseMap();
        }
    }
}
