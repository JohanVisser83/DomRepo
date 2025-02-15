using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblMedia")]

public class Media : BaseEntity
{
    public long CommunityId { get; set; }
    public string? commMediaLink { get; set; }
    public string? commMediaName { get; set; }
    //public string? Media { get; set; }


    public override void ApplyKeys()
    {

    }
}

