using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblSubCampaignCodes")]

public class SubCampaignCodes : BaseEntity
{
    public long? OrgId { get; set; }
    public long CustomerId { get; set; }
    public string? CampaignCode { get; set; }
    public string? CampaignName { get; set; }
    public string? Desc { get; set; }


    public override void ApplyKeys()
    {

    }
}
