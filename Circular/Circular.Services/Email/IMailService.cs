using Circular.Core.Entity;
using Circular.Framework.Middleware.Emailer;
namespace Circular.Services.Email
{
    public interface IMailService
    {
        MailSettings EmailParameter(MailType mailType, ref MailRequest mailRequest);
        Task<bool> SaveAndSendMailAsync(MailRequest mailRequest, MailSettings mailSettings);

        Task<bool> SendMailAsync(MailRequest mailRequest, MailSettings mailSettings);
       
        
    }
}
