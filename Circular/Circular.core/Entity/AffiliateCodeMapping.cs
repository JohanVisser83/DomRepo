using Circular.Core.Entity;
using RepoDb.Attributes;

[Map("tblAffiliatesMapping")]
public class AffiliateCodeMapping : BaseEntity
{
    public long AffiliateId { get; set; }

    public long AffiliateCodeId { get; set; }   



    public override void ApplyKeys()
    {

    }

}

