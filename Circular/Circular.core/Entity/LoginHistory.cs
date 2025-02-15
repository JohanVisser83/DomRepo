using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblLoginHistory")]

public class LoginHistory : BaseEntity
{
    public long CustomerId { get; set; }
    public DateTime LastLoginTime { get; set; }
    public DateTime? LastLogoutTime { get; set; }
    public string? ClientIPAddress { get; set; }
    public string? ClientMachineName { get; set; }



    public override void ApplyKeys()
    {

    }
}
