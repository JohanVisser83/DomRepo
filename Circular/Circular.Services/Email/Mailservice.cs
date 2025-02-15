using MimeKit;
using Circular.Data.Repositories.Email;
using Microsoft.Extensions.Configuration;
using Circular.Framework.Middleware.Emailer;
using Circular.Core.Entity;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using Circular.Framework.Emailer;

namespace Circular.Services.Email
{
    public class Mailservice : IMailService
    {
        private readonly IMailRepository _mailRepository;
        private readonly IEMail _eMail;
       

        public Mailservice(IConfiguration configuration, IMailRepository mailRepository,IEMail Email)
        {
            _mailRepository = mailRepository;
            _eMail = Email;
        }
       
   

        public MailSettings EmailParameter(MailType mailType, ref MailRequest mailRequest)
        {
            EmailParameter emailParameter = _mailRepository.EmailParameter(mailType).Result;
            mailRequest.Subject = emailParameter.Subject;
            mailRequest.BCC = emailParameter.EmailDefaultBCC;
            mailRequest.EmailParameterId = emailParameter.Id;
            mailRequest.CC = emailParameter.EmailDefaultCC;
            mailRequest.EmailType = mailType.ToString();
            mailRequest.EMailParameterId = emailParameter.Id;

            MailSettings mailSettings = new MailSettings();
            mailSettings.UseDefaultCredentials = false;
            mailSettings.UserName = emailParameter.SMTPUserName;
            mailSettings.Password = emailParameter.SMTPPassword;
            mailSettings.DisplayName = emailParameter.DisplayName;
            mailSettings.From = emailParameter.EmailFrom;
            mailSettings.EnableSSL= emailParameter.SMTPSSLEnabled;
            mailSettings.Host = emailParameter.SMTPServer;
            mailSettings.Port = emailParameter.SMTPPort;


            var pathToFile = System.AppDomain.CurrentDomain.BaseDirectory + emailParameter.EmailTemplatePath + emailParameter.TemplateName;
            

            StreamReader reader = System.IO.File.OpenText(pathToFile);
            string body = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();
            mailRequest.Body = body;
            mailRequest.EmailTemplatePath = pathToFile;

            return mailSettings;

        }

        public async Task<bool> SaveAndSendMailAsync(MailRequest mailRequest,MailSettings mailSettings)
        {
            try
            {
                if (await SaveMailAsync(mailRequest, mailSettings))
                    return await SendMailAsync(mailRequest, mailSettings);
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
                //throw ex;
            }

        }

        
        public async Task<bool> SendMailAsync(MailRequest mailRequest, MailSettings _mailSettings)
        {
            try
            {
                return await _eMail.SendEmailAsync(mailRequest, _mailSettings);
            }
            catch (Exception ex)
            {
                return false; 
            }
        }
        public async Task<bool> SaveMailAsync(MailRequest mailRequest, MailSettings mailSettings)
        {
            try
            {
                SentEmails sentEmail = new SentEmails();
                
                sentEmail.SMTPUserName = mailSettings.UserName;
                sentEmail.DisplayName = mailSettings.DisplayName;
                sentEmail.EmailFrom = mailSettings.From;
                sentEmail.EmailTo = mailRequest.To;
                sentEmail.SMTPSSLEnabled = mailSettings.EnableSSL;
                sentEmail.SMTPServer = mailSettings.Host;
                sentEmail.SMTPPort = mailSettings.Port;
                sentEmail.EmailTemplatePath = mailRequest.EmailTemplatePath;

                sentEmail.Subject = mailRequest.Subject;
                sentEmail.EmailBCC = mailRequest.BCC;
                sentEmail.EMailParameterId = mailRequest.EMailParameterId;
                sentEmail.EmailCC = mailRequest.CC;
                sentEmail.Message = mailRequest.Body;
                sentEmail.EmailType = mailRequest.EmailType;
                sentEmail.ReferenceId = mailRequest.ReferenceId;
                sentEmail.SenderId = mailRequest.FromUserId;

                await _mailRepository.SaveSentEmail(sentEmail);
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


    }
}
