using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblNotificationTarget")]

public class NotificationTarget : BaseEntity
{
    public string Name { get; set; }

    public override void ApplyKeys()
    {

    }
}
