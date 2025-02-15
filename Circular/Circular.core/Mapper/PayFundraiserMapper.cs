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
    public class PayFundraiserMapper : Profile
    {
        public PayFundraiserMapper()
        {
            CreateMap<Fundraiser, PayFundraiserDTO>().ReverseMap();
            CreateMap<Fundraiser, FundhubActiveOrderDTO>().ReverseMap();
        }
    }
}
