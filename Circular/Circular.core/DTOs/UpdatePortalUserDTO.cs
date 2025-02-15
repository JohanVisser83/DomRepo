namespace Circular.Core.DTOs
{
    public class UpdatePortalUserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public long RolesId { get; set; }
        public long CustomerId { get; set; }

    }
}
