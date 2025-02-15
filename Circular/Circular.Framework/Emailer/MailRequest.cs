namespace Circular.Framework.Middleware.Emailer
{
    public class MailRequest
    {
        public string Subject { get; set; }
        public string To { get; set; }
        public string? CC { get; set; }
        public string? BCC { get; set; }
        public string Body { get; set; }
        public long FromUserId { get; set; }
        public long EmailParameterId { get; set; }
        public string EmailType { get; set; }
        public long? ReferenceId { get; set; }
        public long? EMailParameterId { get; set; }
        public string? EmailTemplatePath { get; set; }


    }
}
