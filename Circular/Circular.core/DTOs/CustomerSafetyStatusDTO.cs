namespace Circular.Core.DTOs
{
    public class CustomerSafetyStatusDTO
    {
        public long? CustomerId { get; set; }
        public DateTime Date { get; set; }
        public bool IsSafe { get; set; }

    }
}
