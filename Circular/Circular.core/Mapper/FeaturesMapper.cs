using AutoMapper;
using Circular.Core.Entity;
using Circular.Core.DTOs;

namespace Circular.Mapper
{
	public class FeaturesMapper : Profile
	{
		public FeaturesMapper() 
		{
			CreateMap<Features, FeaturesDTO>().ReverseMap();
		}
	}
}
