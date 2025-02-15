using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class PollOptionsMapper : Profile
    {
        public PollOptionsMapper()
        {
            CreateMap<PollOptions, PollOptionsDTO>().ReverseMap();
        }
    }
}
