using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("mtblHouse")]

public class House : MasterEntity
{
    public long? CommunityId { get; set; }

    public override void ApplyKeys()
    {

    }
}