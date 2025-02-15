using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;



namespace Circular.Mapper
{
    public class SicknotesMapper : Profile
    {
        public SicknotesMapper()
        {
            CreateMap<Sicknotes,SicknotesDTO>().ReverseMap();
    
            //CreateMap<Sicknotes, SicknotesDTO>().ReverseMap();
			CreateMap<Sicknotes,NotesDetailsDTO>().ReverseMap();
			//CreateMap<sicknotes,sicknotesDetailDTO>().ReverseMap();

		}
    }
}
