using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("DocType")]

public class DocType : BaseEntity
{

    public string? DocName { get; set; }

    public override void ApplyKeys()
    {

    }
}

