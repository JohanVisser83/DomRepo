using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class JobApplicationMapper : Profile
    {
        public JobApplicationMapper()
        {
            CreateMap<JobApplication, JobApplicationDTO>().ReverseMap();
        }
    }
}
