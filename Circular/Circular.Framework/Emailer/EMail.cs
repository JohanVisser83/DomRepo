using Circular.Framework.Middleware.Emailer;
using System.Net.Mail;
using System.Net;

namespace Circular.Framework.Emailer
{
    public class EMail : IEMail
    {
        public async Task<bool> SendEmailAsync(MailRequest mailRequest, MailSettings _mailSettings)
        {
            MailMessage msg = new MailMessage();
            try
            {
                msg.IsBodyHtml = true;
                msg.BodyEncoding = System.Text.Encoding.UTF8;
                msg.Subject = mailRequest.Subject;
                msg.Body = mailRequest.Body;
                msg.From = new MailAddress(_mailSettings.From, _mailSettings.DisplayName);
                string[] multiToEmail = mailRequest.To.Split(',');
                foreach (string multi in multiToEmail)
                    msg.To.Add(new MailAddress(multi));
                if (mailRequest.BCC != null && mailRequest.BCC != "")
                {
                    string[] multiBccEmail = mailRequest.BCC.Split(',');
                    foreach (string multiBcc in multiBccEmail)
                        msg.Bcc.Add(new MailAddress(multiBcc));
                }
                if (mailRequest.CC != null && mailRequest.CC != "")
                {
                    string[] multiCcEmail = mailRequest.CC.Split(',');
                    foreach (string multiCc in multiCcEmail)
                        msg.CC.Add(new MailAddress(multiCc));
                }

                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                client.Credentials = new NetworkCredential(_mailSettings.UserName, _mailSettings.Password);
                client.EnableSsl = _mailSettings.EnableSSL;
                client.Host = _mailSettings.Host;
                client.Port = _mailSettings.Port;
                client.UseDefaultCredentials = _mailSettings.UseDefaultCredentials;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                await client.SendMailAsync(msg);
                return true;
            }
            catch (Exception ex)
            {
                string strEx = ex.Message;
                return false;
            }
            finally
            {
                msg.Dispose();
            }
        }
    }
}
