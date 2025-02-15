using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Data.Repositories.Audit;
namespace Circular.Services.Audit
{
	public class AuditService : IAuditService
	{
		private readonly IAuditRepository _auditRepository;
		public AuditService(IAuditRepository auditrepository)
		{
            _auditRepository = auditrepository;
		}

        public async Task<int> SaveAudit(long CustomerId, string Activity, string ActivityDesc, string Device, string ClientDetail)
        {
            return await _auditRepository.SaveAudit(CustomerId, Activity, ActivityDesc, Device, ClientDetail);
        }

        public async Task<int> SaveAudit(string UserId, string Activity, string ActivityDesc, string Device, string ClientDetail)
        {
            return await _auditRepository.SaveAudit(UserId, Activity, ActivityDesc, Device, ClientDetail);
        }
    }
}