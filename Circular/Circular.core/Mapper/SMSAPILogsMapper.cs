using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class SMSAPILogsMapper : Profile
    {
        public SMSAPILogsMapper()
        {
            CreateMap<SMSAPILogs,SMSAPILogsDTO>().ReverseMap();
        }
    }
}
