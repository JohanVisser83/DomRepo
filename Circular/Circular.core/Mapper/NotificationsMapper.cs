using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity; 

namespace Circular.Mapper
{
    public class NotificationsMapper : Profile
    {
        public NotificationsMapper()
        {
            CreateMap<Notification,NotificationsDTO>().ReverseMap();
        }
    }
}
