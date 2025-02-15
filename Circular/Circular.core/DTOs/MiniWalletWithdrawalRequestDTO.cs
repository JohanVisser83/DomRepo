namespace Circular.Core.DTOs
{
    public class MiniWalletWithdrawalRequestDTO
    {
        public long? CustomerId { get; set; }
        public decimal? Amount { get; set; }
        public long? WithDrawalRequestStatusId { get; set; }
        public long? CommunityId { get; set; }
    }
}
