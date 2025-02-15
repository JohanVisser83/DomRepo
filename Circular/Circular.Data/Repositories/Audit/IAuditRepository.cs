using Circular.Core.DTOs;
using Circular.Core.Entity;
namespace Circular.Data.Repositories.Audit
{
	public interface IAuditRepository
	{
        Task<int> SaveAudit(long CustomerId, string Activity, string ActivityDesc, string Device, string ClientDetail);

        Task<int> SaveAudit(string UserId, string Activity, string ActivityDesc, string Device, string ClientDetail);


    }

}
