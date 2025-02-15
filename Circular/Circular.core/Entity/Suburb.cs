using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblSuburb")]

public class Suburb : BaseEntity
{
    public string? Name { get; set; }
    public long? CityId { get; set; }
    public string? Desc { get; set; }

    public override void ApplyKeys()
    {

    }
}
