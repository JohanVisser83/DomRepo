namespace Circular.Core.DTOs
{
    public class EventsRequestDTo
    {
        public long CommunityId { get; set; }
        public long CustomerId { get; set; }
        public long EventId { get; set; }
        public int IsAllUpcomingOrCompleted { get; set; } = 2;

    }
    public class EventsRegistrationRequest
    {
        public long EventId { get; set; }
        public long LoggedInCustomerId { get; set; }
        public long RegistrationForCustomerId { get; set; }
        public decimal Amount { get; set; }
    }
    public class EventsDeRegistrationRequest
    {
        public long InviteId { get; set; }

    }
}
