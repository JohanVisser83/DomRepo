using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using System.Net;


namespace Circular.Mapper
{
    public class MessagesMapper : Profile
    {
        public MessagesMapper()
        {
            CreateMap<Messages,MessagesDTO>().ReverseMap();
            CreateMap<Messages, SaveMessagesRequest>().ReverseMap();
            CreateMap<Messages, GetMessagesRequest>().ReverseMap();
            CreateMap<Messages, MessagesGroupResponse>().ReverseMap();


            CreateMap<MessagesListResponse, MessagesListResponseDTO>().ReverseMap();
            CreateMap<MessagesGroupResponse, MessagesGroupResponseDTO>().ReverseMap();


        }
    }
}
