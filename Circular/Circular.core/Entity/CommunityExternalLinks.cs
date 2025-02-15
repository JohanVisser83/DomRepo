using RepoDb.Attributes;

namespace Circular.Core.Entity;
    [Map("tblCommunityExternalLinks")]

    public class CommunityExternalLinks : BaseEntity
    {
    public long CommunityId { get; set; }
    public string ButtonName { get; set; }
    public string Link { get; set; }
    public string ButtonText { get; set; }
    public override void ApplyKeys()
    {

    }
}

