namespace Circular.Core.DTOs
{
    public class CustomerAuthTokensDTO
    {
        public long? CustomerId { get; set; }
        public string AuthToken { get; set; }
        public string? DeviceId { get; set; }
    }
}
