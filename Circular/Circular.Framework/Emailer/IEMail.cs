using Circular.Framework.Middleware.Emailer;

namespace Circular.Framework.Emailer
{
    public interface IEMail
    {
        Task<bool> SendEmailAsync(MailRequest mailRequest, MailSettings _mailSettings);
    }
}