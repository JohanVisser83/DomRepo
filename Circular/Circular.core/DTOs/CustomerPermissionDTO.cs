namespace Circular.Core.DTOs
{
    public class CustomerPermissionDTO
    {
        public long UserId { get; set; }
        public long PermissionId { get; set; }
        public int DeniedOrGranted { get; set; }
	
	}
}
