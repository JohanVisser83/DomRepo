using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;



namespace Circular.Mapper
{
    public class MessageTypeMapper : Profile
    {
        public MessageTypeMapper()
        {
            CreateMap<MessageType,MessageTypeDTO>().ReverseMap();
        }
    }
}
