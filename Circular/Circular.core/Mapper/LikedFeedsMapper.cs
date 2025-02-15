using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class LikedFeedsMapper : Profile
    {
        public LikedFeedsMapper()
        {
            CreateMap<LikedFeeds,LikedFeedsDTO>().ReverseMap();
        }
    }
}
