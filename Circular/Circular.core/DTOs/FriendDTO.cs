namespace Circular.Core.DTOs
{
    public class FriendDTO
    {
        public long FromId { get; set; }
        public long ToId { get; set; }
        public int FriendRequestStatusId { get; set; }
    }

    public class NetworkActionDTO
    {
        public long LoggedInUserId { get; set; }
        public long CustomerId { get; set; }
        public int FriendRequestStatusId { get; set; }
    }
}
