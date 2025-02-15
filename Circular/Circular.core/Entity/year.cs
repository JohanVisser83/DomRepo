using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblyear")]

public class year : BaseEntity
{
    public long? Year { get; set; }


    public override void ApplyKeys()
    {

    }
}
