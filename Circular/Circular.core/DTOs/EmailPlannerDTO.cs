namespace Circular.Core.DTOs
{
    public class EmailPlannerDTO
    {
        public long CustomerId { get; set; }
        public long PlannerId { set; get; }
        public string Email { set; get; }
    }

    public class EmailEventDTO
    {
        public long CustomerId { get; set; }
        public long EventId { set; get; }
        public string Email { set; get; }
    }

    public class EmailOrderDTO
    {
        public long CustomerId { get; set; }
        public long OrderId { set; get; }
        public string Email { set; get; }
    }
}
