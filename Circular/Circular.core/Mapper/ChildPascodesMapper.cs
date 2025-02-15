using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;



namespace Circular.Mapper
{
    public class ChildPascodesMapper : Profile
    {
        public ChildPascodesMapper()
        {
            CreateMap<ChildPascodes, ChildPascodesDTO>().ReverseMap();
        }
    }
}
