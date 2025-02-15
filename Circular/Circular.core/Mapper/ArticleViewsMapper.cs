using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Core.Mapper
{
    public  class ArticleViewsMapper : Profile
    {
        public ArticleViewsMapper() 
        { 
            CreateMap<ArticleViews, ArticleViewsDTO>().ReverseMap();
            CreateMap<ArticleComments, ArticleCommentDTO>().ReverseMap();
        }
    }
}
