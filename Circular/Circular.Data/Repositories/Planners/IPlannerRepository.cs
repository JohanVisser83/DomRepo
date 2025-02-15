using Circular.Core.DTOs;
using Circular.Core.Entity;
namespace Circular.Data.Repositories.Planners
{
    public interface IPlannerRepository
    {
        #region "Planner"
        Task<int> NewPlannerAsync(Planner item);
        Task<List<Planner>> GetPlannerItemsAsync(string PlannerType, long plannerId,long? CommunityId);
        Task<List<PaidDocument>> GetPaidDocument(long? customerId);
        Task<int> DeletePlannerItemsAsync(long id);
        Task<List<PlannerType>> GetMasterPlannerTypeAsync();
        Task<IEnumerable<dynamic>?> GetCheckinView(long GroupId ,long EventId);
        #endregion


        #region "Event\Schedule"
        Task<List<Event>> GetEventsAsync(long communityId, int IsAllUpcomingOrCompleted);
        public Task<List<Communities>> PlannerEmailDetails(long communityId,long EventId);
        Task<List<Event>> PostAddEventAsync();
        Task<IEnumerable<dynamic>?> GetEvents(long Id, long CommunityId, long CustomerId);
        public Task<List<CustomerDetails>> GetFirstName(long CustomerId);
        public Task<List<CustomerDetails>> SendEmailPlanner(long LoggedInCustomerId);
        public Task<List<Event>> GetEventEmail(long Id);
        Task<EventListResponse?> Events(long Id, long CommunityId, long CustomerId, int IsAllUpcomingOrCompleted);
        Task<int?> RegisterForEvents(long EventId, string currency, long CustomerId, long RegistrationForCustomerId, decimal Amount);
        Task<int?> DeregisterForEvents(long invitationId);

        #endregion


        Task<List<Planner>> PostAllNewItemAsync();
        Task<long> CustomerBooking(CustomerBooking customerBooking);
        Task<int> CancelCustomerBooking(CustomerBooking customerBooking);
        Task<int> UpdateEvent(Event item);
        Task<int> SaveAsync(Event item);
        Task<long> SaveBookings(Bookings bookings);
        Task<List<Bookings>> PostNewBookingAsync();
        public Task<List<CustomerDetails>> SaveCustomerName(long CustomerId);

        public Task<List<Bookings>> GetBookingActiveItemsAsync();
       Task<IEnumerable<dynamic>> GetBookingDays(long MasterBookingId);
        Task<dynamic> BookingRegistered(long BookingId);

        Task<int> DeleteBookingActiveItemsAsync(long id);
        Task<int> DeleteUpcomingItemsAsync(long id);

        public Task<List<EventInvitees>> ViewUpcomingItemsAsync(long EventId);
      //  public Task<List<EventInvitees>> ViewUpcomingItemsAsync(long EventId);

        Task<long> ScannerProfile(long UserId, int PermissionId, long communityId);

        Task<List<BookingDays>> GetBookingDetails(long? CommunityId, DateTime Date, long? UserId, long? Bookingid, bool IsMyBooking);
        Task<dynamic> GetBookingMemberDetails(long? BookingId);
       Task<dynamic> BookedMembers(long BookindDayId);

        Task<IEnumerable<dynamic>?> GetManageUser(long  PermissionId, long communityId);
        Task<int> DeleteUserManagement(long id);
        Task<int> UpdateUserManagement(long id, bool DeniedOrGranted);
        Task<IEnumerable<dynamic>?> BookingAttendance(DateTime Date, long Id);
        Task<IEnumerable<GetAttendance>?> GetBookingAttendance(long CommunityId);
        Task<IEnumerable<dynamic>> GetActiveBookingById(long Id);
        Task<IEnumerable<Bookings>?> BookedSpaces(long primayCommmunityId);
        Task<IEnumerable<dynamic>> GetEventAttendace(long EventId);
        Task<IEnumerable<dynamic>> GetBookingDetails(long BookingId, bool IsBookingStatus);
        //Task<IEnumerable<dynamic>?> GetRsvpDetails(long GroupId, long EventId);
        Task<bool> UpdateCheckin(long EnviteeId, bool Checkin);

        public  Task<List<EventNotificationDTO>> EventScheduleNotification();
        Task<int> SendBulkEventEmail(BulkEventEmail bulkEventEmail);
        Task<List<BulkEventEmail?>> GetBulkEmaildetails();
        Task<long> UpdateIsSentForBukEmail(BulkEventEmail bulkEventEmail);
        Task<List<CustomerDetails>> SendAddEventEmail(long Id, int IsGroup);

        Task<int> updateScheduleNotify(long id);

        Task<int> AddPaidDocument(PaidDocument item);
    }
}
