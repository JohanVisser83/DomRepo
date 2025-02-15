using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;


namespace Circular.Mapper
{
    public class AddPaymentDashboardMapper : Profile
    {
        public AddPaymentDashboardMapper() 
        {
            CreateMap<AddPaymentDashboard,AddPaymentDashboardDTO>().ReverseMap();
        }
    }
}
