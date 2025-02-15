namespace Circular.Core.DTOs
{
    public class UserCommunityRoleDTO
    {
        public long? CustomerId { get; set; }
        public long? CommunityRoleId { get; set; }
        public long? CommunityId { get; set; }
        public bool? IsOrganizer { get; set; }
    }
}
