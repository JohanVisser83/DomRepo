using Circular.Core.Entity;
namespace Circular.Data.Repositories.Email
{
    public interface IMailRepository 
    {
        Task<EmailParameter> EmailParameter(MailType mailType);
       
        Task<long> SaveSentEmail(SentEmails sentEmails);

    }
}
