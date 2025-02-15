using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;



namespace Circular.Mapper
{
    public class MessageSummaryMapper : Profile
    {
        public MessageSummaryMapper()
        {
            CreateMap<MessageSummary,MessageSummaryDTO>().ReverseMap();
            CreateMap<MessageSummary, ReadMessageRequest>().ReverseMap();

        }
    }
}
