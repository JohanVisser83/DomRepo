using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblBankBranches")]

public class BankBranches : BaseEntity
{
    public long? BankId { get; set; }
    public string? BranchCode { get; set; }
    public string? BranchAddress { get; set; }


    public override void ApplyKeys()
    {

    }
}
