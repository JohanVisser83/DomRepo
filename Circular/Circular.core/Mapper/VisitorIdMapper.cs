using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Core.Mapper
{
    public  class VisitorIdMapper : Profile
    {
        public VisitorIdMapper()
        {
            CreateMap<Visitor, VisitorDTO>().ReverseMap();
        }    
    }
}
