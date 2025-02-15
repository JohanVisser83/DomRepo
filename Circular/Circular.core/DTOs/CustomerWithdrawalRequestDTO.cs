namespace Circular.Core.DTOs
{
    public class CustomerWithdrawalRequestDTO
    {
        public long? CustomerId { get; set; }
        public long? SavedBankId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? ServiceFee { get; set; }
        public long? WithDrawalRequestStatusId { get; set; }
        public long? TransactionId { get; set; }
        public long? CommunityId { get; set; }
        public bool? IsDeletedByAdmin { get; set; }
        public long? ReferenceId { get; set; }
        public string? BankName { get; set; }
        public String? ReferenceType { get; set; }
        public decimal? FixedCharge { get; set; }
        public String? ReferenceComment { get; set; }
    }
}
