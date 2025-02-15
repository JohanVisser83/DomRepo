using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblDriver")]

public class Driver : BaseEntity
{
    public long? CommunityId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Contact { get; set; }


    public override void ApplyKeys()
    {

    }
}
