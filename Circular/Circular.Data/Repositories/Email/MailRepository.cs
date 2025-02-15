using Circular.Core.Entity;
using Microsoft.Data.SqlClient;
using RepoDb;

namespace Circular.Data.Repositories.Email
{
    public class MailRepository : DbRepository<SqlConnection>, IMailRepository
    {
        public MailRepository(string connectionString) : base(connectionString)
        {
        }
        public async Task<EmailParameter> EmailParameter(MailType mailType)
        {
            return QueryAsync<EmailParameter?>(E => E.EmailType == mailType.ToString() && E.IsActive == true).Result.FirstOrDefault() ?? null;
        }
        public async Task<long> SaveSentEmail(SentEmails sentEmail)
        {
            try
            {
                sentEmail.FillDefaultValues();
                return await InsertAsync<SentEmails, int>(sentEmail);

            }
            catch (Exception ex)
            {
                throw ex;
            }




        }


       
    }
}
