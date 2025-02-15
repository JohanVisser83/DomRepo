using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("mtblAdminConfigurations")]
public class AdminConfigurations : BaseEntity
{
    public string? BankName { get; set; }
    public string? AccountName { get; set; }
    public string? AccountNumber { get; set; }
    public string? BranchCode { get; set; }
    public string? SwiftCode { get; set; }
    public string? AccountType { get; set; }
    public string? Reference { get; set; }
    public string? Desc { get; set; }
    public string? adminType { get; set; }
    public override void ApplyKeys()
    {

    }
}

