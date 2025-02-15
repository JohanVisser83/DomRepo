
namespace Circular.Core.DTOs
{
    public class CustomerPasswordChangeRequestDTO
    {
        public long CustomerId { get; set; }
        public long PasswordActivationCode { get; set; }
        public bool? IsVerified { get; set; }
       

    }
}
