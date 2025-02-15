using AutoMapper;
using Circular.Core.Entity;
using Circular.Core.DTOs;

namespace Circular.Mapper
{
	public class AuditMapper : Profile
	{

		public AuditMapper()
		{
			CreateMap<Audit, AuditDTO>().ReverseMap();
			CreateMap<Audit, AuditdetailDTO>().ReverseMap();

		}
	}
}
