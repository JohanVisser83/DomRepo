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
    public class RefundMapper : Profile
    {
        public RefundMapper()
        {
            CreateMap<Refund, RefundDTO>().ReverseMap();
        }
    }
}
