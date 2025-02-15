using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblCustomerWithdrawalRequest")]

public class CustomerWithdrawalRequest : BaseEntity
{
    public long? CustomerId { get; set; }
    public long? SavedBankId { get; set; }
    public decimal? Amount { get; set; }
    public decimal? ServiceFee { get; set; }
    public long? WithDrawalRequestStatusId { get; set; }
    public long? TransactionId { get; set; }
    public long? ReferenceId { get; set; }
    public long? CommunityId { get; set; }
    public bool? IsDeletedByAdmin { get; set; }
    public String? ReferenceType { get; set; }
    public String? BankName { get; set; }
    public String? ReferenceComment { get; set; }

    public String? currency { get; set; }

    public override void ApplyKeys()
    {

    }
}
