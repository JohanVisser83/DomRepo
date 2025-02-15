namespace Circular.Core.DTOs
{
    public class RefundDTO
    {
        public long CommunityId { get; set; }
        public long UserId { get; set;}
        public decimal Amount { get; set;}
        public String RefundNote { get; set;}
    }
}
