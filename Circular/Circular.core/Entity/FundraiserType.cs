using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblFundraiserType")]

public class FundraiserType : BaseEntity
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Desc { get; set; }
    public string Code { get; set; }
  

    public override void ApplyKeys()
    {

    }
}
