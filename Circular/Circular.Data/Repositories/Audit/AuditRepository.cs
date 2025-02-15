using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Framework.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using RepoDb;
using System.Dynamic;

namespace Circular.Data.Repositories.Audit
{
	public class AuditRepository : DbRepository<SqlConnection>, IAuditRepository
	{
		public AuditRepository(string connectionString) : base(connectionString)
		{
		}
        public async Task<int> SaveAudit(long CustomerId, string Activity, string ActivityDesc, string Device, string ClientDetail)
        {
            int result = await ExecuteScalarAsync<int>($"Exec [dbo].[Usp_SaveAudit] {CustomerId}, '{Activity}', '{ActivityDesc}', '{Device}', '{ClientDetail}'");
            return result;
        }

        public async Task<int> SaveAudit(string UserId, string Activity, string ActivityDesc, string Device, string ClientDetail)
        {
            // int result = await ExecuteScalarAsync<int>($"Exec [dbo].[Usp_SaveAuditApi] '{UserId}', '{Activity}', '{ActivityDesc}', '{Device}', '{ClientDetail}'");
            // return result;
            return 1;
        }
    }
}
