namespace Circular.Core.DTOs
{
    public class SentEmailsDTO
    {
        public long SenderId { get; set; }
        public long? ReceiverId { get; set; }
        public string? EmailType { get; set; }
        public string? EmailFrom { get; set; }
        public string? EmailTo { get; set; }
        public string? EmailCC { get; set; }
        public string? EmailBCC { get; set; }
        public string? DisplayName { get; set; }
        public string? SMTPServer { get; set; }
        public string? SMTPPort { get; set; }
        public string? SMTPUserName { get; set; }
        public bool? SMTPSSLEnabled { get; set; }
        public string? EmailTemplatePath { get; set; }
        public string? Message { get; set; }
        public string? Subject { get; set; }
        public string? memberRegistrationCode { get; set; }
        public string? PersonName { get; set; }
    }
}
