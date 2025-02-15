using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblMiniWalletWithdrawalRequest")]

public class MiniWalletWithdrawalRequest : BaseEntity
{
    public long? CustomerId { get; set; }
    public decimal? Amount { get; set; }
    public long? WithDrawalRequestStatusId { get; set; }
    public long? CommunityId { get; set; }


    public override void ApplyKeys()
    {

    }
}
