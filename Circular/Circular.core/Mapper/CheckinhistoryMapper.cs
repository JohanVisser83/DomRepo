using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class CheckinhistoryMapper : Profile
    {
        public CheckinhistoryMapper()
        {
            CreateMap<Checkinhistory,CheckinhistoryDTO>().ReverseMap();
        }
    }
}
