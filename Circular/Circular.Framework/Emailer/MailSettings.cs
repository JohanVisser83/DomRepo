namespace Circular.Framework.Middleware.Emailer
{
    public class MailSettings
    {
        public string From { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public bool EnableSSL { get; set; }=true;
        public bool UseDefaultCredentials { get; set; } = false;
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
    }
}
