using Circular.Core.Entity;
using RepoDb.Attributes;

[Map("tblCommunityAffiliateCodes")]
public class CommunityAffiliateCodes : BaseEntity
{
    public long CommunityId { get; set; }   

    public string AffiliateCode { get; set; }   

    public override void ApplyKeys()
    {

    }
}

