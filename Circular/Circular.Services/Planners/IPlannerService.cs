using Circular.Core.DTOs;
using Circular.Core.Entity;
using System.Reflection.Metadata;

namespace Circular.Services.Planners;

public interface IPlannerService
{
    #region "Planner"

    Task<List<Planner>> GetPlannerItemsAsync(string plannerType, long plannerId, long? communityid);
    Task<List<PaidDocument>> GetPaidDocument(long? customerId);
    Task<bool> EmailPlanner(EmailPlannerDTO emailPlannerDTO, long communityid);
    public Task<List<CustomerDetails>> SaveCustomerName(long CustomerId);
    public Task<List<CustomerDetails>> GetFirstName(long CustomerId);
    public Task<List<Event>> GetEventEmail(long Id);
    Task<bool> SendBulkEmails();

    Task<int> NewPlannerAsync(Planner item);
    Task<int> DeletePlannerItemsAsync(long id);
    Task<List<Planner>> PostAllNewItemAsync();
    Task<IEnumerable<dynamic>?> GetCheckinView(long GroupId, long EventId);
    //Task<IEnumerable<dynamic>?> GetRsvpDetails(long GroupId, long EventId);
    Task<List<PlannerType>> GetMasterPlannerTypeAsync();
    public Task<List<Communities>> PlannerEmailDetails(long communityId, long EventId);
    #endregion


    #region "Event\Schedule"
    Task<List<Event>> GetEventsAsync(long communityId, int IsAllUpcomingOrCompleted);
    Task<IEnumerable<dynamic>?> GetEvents(long Id, long CommunityId, long CustomerId);
    Task<int> UpdateEvent(Event item);
    Task<int> SaveAsync(Event item, DateTime dtSchedule);


    //Task<List<BulkEventEmail?>> GetBulkEmaildetails();

   // Task<long> UpdateIsSentForBukEmail(BulkEventEmail bulkEventEmail);

    Task<EventListResponse?> Events(long Id, long CommunityId, long CustomerId, int IsAllUpcomingOrCompleted);
    Task<int?> RegisterForEvents(long EventId, string currency, long CustomerId, long RegistrationForCustomerId, decimal Amount);
    public Task<List<CustomerDetails>> SendEmailPlanner(long LoggedInCustomerId);
    Task<int?> DeregisterForEvents(long invitationId);

    //Task<List<CustomerDetails>> SendAddEventEmail(long Id, int IsGroup);
    #endregion



    #region "Booking"


    Task<long> CustomerBooking(CustomerBooking customerBooking);
    Task<int> CancelCustomerBooking(CustomerBooking customerBooking);

    Task<List<Bookings>> PostNewBookingAsync();
    Task<List<Bookings>> GetBookingActiveItemsAsync();
    //public Task<List<customerBooking>> GetCustomerBooking();

    Task<int> DeleteBookingActiveItemsAsync(long id);
    Task<int> DeleteUpcomingItemsAsync(long id);
    Task<List<BookingDays>> GetBookingDetails(long? CommunityId, DateTime Date, long? UserId, long? Bookingid, bool IsMyBooking);
    Task<dynamic> GetBookingMemberDetails(long? BookingId);
    Task<dynamic> BookedMembers(long BookindDayId);
    Task<dynamic> BookingRegistered(long BookingId);
    Task<IEnumerable<Bookings>?> BookedSpaces(long primayCommmunityId);

    #endregion

    Task<List<Event>> PostAddEventAsync();

    Task<List<EventInvitees>> ViewUpcomingItemsAsync(long EventId);

    Task<long> ScannerProfile(long UserId, int PermissionId, long communityId);
    Task<IEnumerable<dynamic>?> GetManageUser(long PermissionId, long communityId);

    Task<int> DeleteUserManagement(long id);
    Task<int> UpdateUserManagement(long id, bool IsScannerActive);
    Task<IEnumerable<dynamic>?> BookingAttendance(DateTime Date, long Id);
    Task<IEnumerable<GetAttendance>?> GetBookingAttendance(long CommunityId);
    Task<IEnumerable<dynamic>> GetActiveBookingById(long Id);

    Task<IEnumerable<dynamic>> GetEventAttendace(long EventId);
    Task<IEnumerable<dynamic>> GetBookingDetails(long BookingId, bool IsBookingStatus);
    Task<long> SaveBookings(Bookings availableBookingDays);
    Task<IEnumerable<dynamic>> GetBookingDays(long MasterBookingId);
    //  

    Task<bool> UpdateCheckin(long EnviteeId, bool Checkin);
    public Task<List<EventNotificationDTO>> EventScheduleNotification();
    Task<int> updateScheduleNotify(long id);
    Task<int> SendBulkEventEmail(BulkEventEmail bulkEventEmail);

    Task<int> AddPaidDocument(PaidDocument item);
 
}
