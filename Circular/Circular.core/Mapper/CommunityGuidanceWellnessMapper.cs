using AutoMapper;
using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Mapper
{
	public class CommunityGuidanceWellnessMapper : Profile
	{
		public CommunityGuidanceWellnessMapper() 
		{
			CreateMap<CommunityGuidanceWellness, CommunityGuidanceWellnessDTO>().ReverseMap();
		}

	}
}
