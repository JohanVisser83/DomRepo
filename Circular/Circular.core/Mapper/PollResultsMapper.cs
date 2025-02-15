using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class PollResultsMapper : Profile
    {
        public PollResultsMapper()
        {
            CreateMap<PollResults, PollResultsDTO>().ReverseMap();
        }
    }
}
