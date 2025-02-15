using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Core.Mapper
{
    public class CustomerSupportsMapper : Profile
    {
        public CustomerSupportsMapper() 
        {
            CreateMap<CustomerSupports, CustomerSupportsDTO>().ReverseMap();
        }
    }
}
