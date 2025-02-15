using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblChildPascodes")]

public class ChildPascodes : BaseEntity
{
    public long CustomerId { get; set; }
    public string? Passcode { get; set; }
    public string? Description { get; set; }


    public override void ApplyKeys()
    {

    }
}
