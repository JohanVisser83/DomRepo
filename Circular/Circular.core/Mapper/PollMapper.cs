using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Mapper
{
    public class PollMapper:Profile
    {
        public PollMapper()
        {
            CreateMap<Poll, PollDTO>().ReverseMap();
        }
    }
}
