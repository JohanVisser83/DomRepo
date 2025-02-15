using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblSMSAPILogs")]

public class SMSAPILogs : BaseEntity
{
    public string? MobileNumber { get; set; }
    public string? APIResponseXML { get; set; }
    public string? APIResponseCode { get; set; }
    public string? Description { get; set; }



    public override void ApplyKeys()
    {

    }
}
