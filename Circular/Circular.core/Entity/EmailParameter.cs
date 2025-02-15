using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblEmailParameters")]
public class EmailParameter : BaseEntity
{
    public string EmailType { get; set; }
    public string EmailFrom { get; set; }
    public string? EmailDefaultTO { get; set; }
    public string? EmailDefaultCC { get; set; }
    public string? EmailDefaultBCC { get; set; }
    public string DisplayName { get; set; }
    public string SMTPServer { get; set; }
    public int SMTPPort { get; set; }
    public string SMTPUserName { get; set; }
    public string SMTPPassword { get; set; }
    public bool SMTPSSLEnabled { get; set; } = true;
    public string TemplateName { get; set; }
    public string EmailTemplatePath { get; set; }
    public string Subject { get; set; }
    public override void ApplyKeys()
    {

    }
}
