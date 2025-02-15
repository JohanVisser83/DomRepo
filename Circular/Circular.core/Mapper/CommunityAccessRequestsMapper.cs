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
    public class CommunityAccessRequestsMapper : Profile
    {
        public CommunityAccessRequestsMapper()
        { 
          CreateMap<CommunityAccessRequests, CommunityAccessRequestsDTO>().ReverseMap();
          CreateMap<CommunityAccessRequests, CommunityAccessDTO>().ReverseMap();
          CreateMap<CommunityAccessRequests, JoinCommunityDTO>().ReverseMap();
            

        }
    }
}
