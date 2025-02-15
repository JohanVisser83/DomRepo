using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblMerchantLegalSupportingDocs")]

public class MerchantLegalSupportingDocs : BaseEntity
{
    public long? CustomerId { get; set; }
    public string? DocName { get; set; }
    public string? DocType { get; set; }
    public string? DocPath { get; set; }



    public override void ApplyKeys()
    {

    }
}
