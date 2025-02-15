using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblSecurityAnswer")]

public class SecurityAnswer : BaseEntity
{
    public long? Customerid { get; set; }
    public long? QusestionId { get; set; }
    public string? Answer { get; set; }



    public override void ApplyKeys()
    {

    }
}
