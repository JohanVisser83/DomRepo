using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblCustomerDevices")]
public class CustomerDevices : BaseEntity
{
    public long CustomerId { get; set; }
    public string Device { get; set; } = "";

    public string? Desc { get; set; }

	public string? AppVersion { get; set; } = "1.0";

    public string? TimeZone { get; set; }
    public override void ApplyKeys()
    {

    }
}