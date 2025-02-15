using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblNotificationType")]

public class NotificationType : BaseEntity
{
    public string Name { get; set; }


    public override void ApplyKeys()
    {

    }
}

