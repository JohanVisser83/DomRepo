namespace Circular.Core.DTOs
{
    public class LoginHistoryDTO
    {
        public long CustomerId { get; set; }
        public DateTime LastLoginTime { get; set; }
        public DateTime? LastLogoutTime { get; set; }
        public string? ClientIPAddress { get; set; }
        public string? ClientMachineName { get; set; }
    }
}
