using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblSentNotifications")]

public class SentNotifications : BaseEntity
{
    public string? SentTo { get; set; }
    public string? Message { get; set; }
    public string CampaignCode { get; set; }


    public override void ApplyKeys()
    {

    }
}
