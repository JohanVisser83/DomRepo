using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblSentNotificationHistory")]

public class SentNotificationHistory : BaseEntity
{
    public string SentTo { get; set; }
    public string? Message { get; set; }
    public string? CampaignCode { get; set; }
    public string? Desc { get; set; }  
    public long? SentFrom { get; set; }


    public override void ApplyKeys()
    {

    }
}
