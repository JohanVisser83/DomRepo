using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class NewsFeedsMapper : Profile
    {
        public NewsFeedsMapper()
        {
            CreateMap<NewsFeeds,NewsFeedsDTO>().ReverseMap();
            CreateMap<NewsFeeds, PostArticleDTO>().ReverseMap();
           
        }
    }
}
