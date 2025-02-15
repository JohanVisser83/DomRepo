using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Framework.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using RepoDb;
namespace Circular.Data.Repositories.Planners
{
    public class PlannerRepository : DbRepository<SqlConnection>, IPlannerRepository
    {
        private readonly IHelper _helper;
        IHttpContextAccessor _httpContextAccessor;
        public PlannerRepository(string connectionString, IHelper helper, IHttpContextAccessor httpContextAccessor) : base(connectionString)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

        }

        #region "Planner"
        public async Task<List<Planner>> GetPlannerItemsAsync(string PlannerType, long plannerId, long? CommunityId)
        {
            try
            {
                long ptId = 0;
                List<PlannerType> plannerTypes = QueryAll<PlannerType>().Where(pt => pt.IsActive == true).ToList<PlannerType>();
                if (PlannerType == "All" || PlannerType == "-1")
                    ptId = 0;
                else
                    ptId = plannerTypes.Where(p => p.Icon == PlannerType).FirstOrDefault().Id;


                return QueryAll<Planner>().Where(p => p.IsActive == true
                                    && p.PlannerTypeId == (ptId == 0 ? p.PlannerTypeId : ptId)
                                    && p.Id == (plannerId == 0 ? p.Id : plannerId)).OrderBy(p => p.title)
                    .Select(x => new Planner
                    {
                        CommunityId = x.CommunityId,
                        PlannerTypeId = x.PlannerTypeId,
                        title = x.title,
                        Price= x.Price,
                        Description = x.Description ?? "",
                        IsArchived = x.IsArchived,
                        HyperLink = x.HyperLink ?? "",
                        Media = x.Media ?? (x.HyperLink ?? ""),
                        PlannerTypeName = plannerTypes.Find(pt => pt.Id == x.PlannerTypeId).Name,
                        Id = x.Id,
                    }).Where(x => x.CommunityId == CommunityId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<List<PaidDocument>> GetPaidDocument(long? customerId)
        {
            try
            {
                var result = QueryAll<PaidDocument>().Where(e => e.IsActive == true && e.UserId == customerId).Select(x => new PaidDocument
                {
                    UserId = x.UserId,
                    DocumentId = x.DocumentId
                }).OrderByDescending(y => y.Id).ToList();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public async Task<List<Planner>> PostAllNewItemAsync()
        {
            return QueryAll<Planner>().Where(e => e.IsActive == true).ToList();
        }
        public async Task<int> NewPlannerAsync(Planner item)
        {
            try { 
            var key = await InsertAsync<Planner, int>(item);
            return key;
              
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<int> DeletePlannerItemsAsync(long id)
        {
            Planner planner = new Planner() { Id = id, ModifiedBy = 101, ModifiedDate = DateTime.Now, IsActive = false };
            var fields = Field.Parse<Planner>(x => new
            {
                x.IsActive,
                x.ModifiedBy,
                x.ModifiedDate
            });
            var updatedrow = await UpdateAsync<Planner>(entity: planner, fields: fields);
            return updatedrow;
        }
        public async Task<IEnumerable<dynamic>> GetCheckinView(long GroupId, long EventId)
        {
            var result = await ExecuteQueryAsync<dynamic>("Exec [dbo].[USP_EventCheckInView] '" + GroupId + "','" + EventId + "'");
            return result;
        }

        public async Task<List<PlannerType>> GetMasterPlannerTypeAsync()
        {
            return QueryAll<PlannerType>().Where(e => e.IsActive == true).ToList();
        }

        public async Task<List<CustomerDetails>> SaveCustomerName(long CustomerId)
        {
            return QueryAll<CustomerDetails>().Where(G => G.IsActive == true && G.CustomerId == CustomerId).ToList();
        }

        #endregion

        #region "Event\Schedule"

        public async Task<int?> DeregisterForEvents(long invitationId)
        {
            var updatedRows = 0;
            EventInvitees invite = QueryAsync<EventInvitees?>(J => J.Id == invitationId && J.IsActive == true).Result.FirstOrDefault() ?? null;
            if (invite != null)
            {
                invite.IsActive = false;
                invite.UpdateModifiedByAndDateTime();
                var fields = Field.Parse<EventInvitees>(e => new
                {
                    e.IsActive,
                    e.ModifiedBy,
                    e.ModifiedDate
                });

                updatedRows = Update<EventInvitees>(entity: invite, fields: fields);
            }
            return updatedRows;
        }
        public async Task<int?> RegisterForEvents(long EventId, string currency, long CustomerId,
            long RegistrationForCustomerId, decimal Amount)
        {
            int result = await ExecuteScalarAsync<int>("Exec [dbo].[Usp_Planner_Schedule_RegisterForEvent] " +
                   EventId + "," + RegistrationForCustomerId + "," + Amount + "," + Amount + ","
                   + CustomerId + ",'" + currency + "'");
            return result;
        }
        public async Task<List<CustomerDetails>> SendEmailPlanner(long LoggedInCustomerId)
        {
            return QueryAll<CustomerDetails>().Where(G => G.IsActive == true && G.CustomerId == LoggedInCustomerId).ToList();
        }
        public async Task<EventListResponse> Events(long Id, long CommunityId, long CustomerId, int IsAllUpcomingOrCompleted)
        {
            List<Event> lstEvents = ExecuteQueryAsync<Event>("Exec [dbo].[Usp_Planner_Schedule_GetEventList] "
                + CustomerId + "," + CommunityId + "," + IsAllUpcomingOrCompleted + "," + Id).Result.ToList<Event>();
            EventListResponse eventListResponse = null;
            if (lstEvents != null)
            {
                eventListResponse = new EventListResponse();
                lstEvents.ForEach(n =>
                {
                    if (n.IsMemberPaid == 1)
                    {
                        string QRCodePath = Directory.GetCurrentDirectory() + "/Uploads/QRs/EventTickets/";
                        var browsePath = _httpContextAccessor?.HttpContext.Request.Scheme + "://" + _httpContextAccessor?.HttpContext.Request.Host + "/Uploads/QRs/EventTickets/";
                        string filename = _helper.EncryptUsingSHA1Hashing(n.InvitationId.ToString()) + ".png";
                        n.QRCode.QRCode = _helper.GetQRCode("EventId:" + n.Id.ToString() + ",customerId:" + n.InviteeId.ToString(), filename, ref QRCodePath);
                        n.QRCode.QRPath = browsePath + filename;
                    }
                    // group the data in dates
                    int index = eventListResponse.EventGroups.FindIndex(ng => ng.EventDate == (new DateTime(n.EventStartDate.Year, n.EventStartDate.Month, 1)));
                    if (index == -1)
                    {
                        EventGroupResponse eventGroupResponse = new EventGroupResponse();
                        eventGroupResponse.EventDate = (new DateTime(n.EventStartDate.Year, n.EventStartDate.Month, 1));
                        eventGroupResponse.EventList?.Add(n);
                        eventListResponse.EventGroups.Add(eventGroupResponse);
                    }
                    else
                        eventListResponse.EventGroups[index].EventList?.Add(n);
                }
                );
            }
            return eventListResponse;
        }
        public async Task<int> SaveAsync(Event item)
        {
            return await InsertAsync<Event, int>(item);
        }
        public async Task<List<Event>> GetEventsAsync(long communityId, int IsAllUpcomingOrCompleted)
        {
            //return QueryAll<Event>().Where(e => e.IsActive == true
            //&& e.CommunityId == communityId).ToList();

            List<Event> lstEvents = ExecuteQueryAsync<Event>("Exec [dbo].[Usp_Planner_Schedule_GetEventListForCommunityPortal] "
                 + communityId + "," + IsAllUpcomingOrCompleted).Result.ToList<Event>();

            return lstEvents;
        }
        public async Task<List<Event>> PostAddEventAsync()
        {
            return QueryAll<Event>().Where(e => e.IsActive == true).ToList();
        }
        public async Task<int> UpdateEvent(Event item)
        {
            try
            {
                Event booking = new Event();
                
                booking.Id = item.Id;
                booking.EventStartDate = item.EventStartDate;
                booking.EventEndDate = item.EventEndDate;
                booking.Description = item.Description;
                booking.StartTime = item.StartTime;
                booking.EndTime = item.EndTime;
                booking.Location = item.Location;

                booking.UpdateModifiedByAndDateTime();
                var fields = Field.Parse<Event>(x => new
                {

                    x.EventStartDate,
                    x.EventEndDate,
                    x.Location,
                    //x.StartTime,
                   // x.EndTime,
                    x.Description,
                    x.ModifiedBy,
                    x.ModifiedDate
                });
                return await UpdateAsync<Event>(entity: booking, fields: fields);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<dynamic>?> GetEvents(long Id, long CommunityId, long CustomerId)
        {

            var result = await ExecuteQueryAsync<dynamic>("Exec [dbo].[USP_Event_Getdetails] '" + Id + "','" + CommunityId + "','" + CustomerId + "'");
            return result;
        }
        public async Task<List<EventInvitees>> ViewUpcomingItemsAsync(long EventId)
        {

            var result = await ExecuteQueryAsync<EventInvitees>("Exec [dbo].[USP_GetCustomerDetails] '" + EventId + "'");
            return (List<EventInvitees>)result;
        }
        public async Task<IEnumerable<dynamic>> GetEventAttendace(long EventId)
        {
            try
            {
                return await ExecuteQueryAsync<dynamic>("Exec [dbo].[Usp_Planner_EventCollectionData] " + EventId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> DeleteUpcomingItemsAsync(long id)
        {
            Event delete = new Event();
            delete.Id = id;
            delete.IsActive = false;
            var fields = Field.Parse<Event>(x => new
            {
                x.IsActive


            });
            var updaterow = await UpdateAsync<Event>(entity: delete, fields: fields);
            return updaterow;
        }
        public async Task<List<EventNotificationDTO>> EventScheduleNotification()
        {

            var result = await ExecuteQueryAsync<EventNotificationDTO>("Exec [dbo].[Usp_EventScheduleNotification]");
            return (List<EventNotificationDTO>)result;
        }
        public async Task<int> SendBulkEventEmail(BulkEventEmail bulkEventEmail)
        {
            bulkEventEmail.FillDefaultValues();
            var key = await InsertAsync<BulkEventEmail, int>(bulkEventEmail);
            return key;
        }
        public async Task<List<BulkEventEmail>> GetBulkEmaildetails()
        {
            try
            {
                List<BulkEventEmail> bulkEventEmails = ExecuteQuery<BulkEventEmail>("exec dbo.USP_Event_GetBulkEventEmail").ToList();
                return bulkEventEmails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }
        public async Task<long> UpdateIsSentForBukEmail(BulkEventEmail bulkEventEmail)
        {
            bulkEventEmail.IsSent = true;
            bulkEventEmail.UpdateModifiedByAndDateTime();
            var fields = Field.Parse<BulkEventEmail>(x => new
            {
                x.IsSent,
                x.ModifiedBy,
                x.ModifiedDate
            });
            return await UpdateAsync<BulkEventEmail>(entity: bulkEventEmail, fields: fields);
        }
        public async Task<List<CustomerDetails>> SendAddEventEmail(long Id, int IsGroup)
        {
            var response = ExecuteQuery<CustomerDetails>("exec dbo.[USP_Event_SentEventEmail] " + Id + "," + IsGroup).ToList();
            return response;
        }

        #endregion

        #region "Booking"

        public async Task<long> CustomerBooking(CustomerBooking customerBooking)
        {
            long result = 0;
            var cb = QueryAsync<CustomerBooking>(e => e.UserId == customerBooking.UserId &&
            e.BookingId == customerBooking.BookingId && e.IsActive == true).Result.FirstOrDefault();
            if (cb == null)
            {
                customerBooking.IsBookingStatus = true;
                result = await InsertAsync<CustomerBooking, int>(customerBooking);
            }
            else
            {
                cb.IsBookingStatus = true;
                var fields = Field.Parse<CustomerBooking>(x => new
                {
                    x.IsBookingStatus
                });
                var updaterow = await UpdateAsync<CustomerBooking>(entity: cb, fields: fields);
                result = cb.Id;
            }

            return result;
        }
        public async Task<int> CancelCustomerBooking(CustomerBooking customerBooking)
        {
            CustomerBooking booking = new CustomerBooking();
            booking.Id = customerBooking.Id;
            booking.IsBookingStatus = false;
            booking.ModifiedDate = DateTime.Now;
            var fields = Field.Parse<CustomerBooking>(x => new
            {
                x.IsBookingStatus,
                x.ModifiedDate
            });
            var updaterow = await UpdateAsync<CustomerBooking>(entity: booking, fields: fields);
            return updaterow;
        }
        public async Task<List<BookingDays>> GetBookingDetails(long? CommunityId, DateTime Date, long? UserId, long? Bookingid, bool IsMyBooking)
        {

            try
            {
                if (Date.ToString() == "01-01-0001 00:00:00" || Date == null)
                {
                    var result = await ExecuteQueryAsync<BookingDays>("Exec [dbo].[Usp_Booking_List]   '" + CommunityId + "',null,'" + UserId + "','" + Bookingid + "','" + IsMyBooking + "'");
                    //var result = await ExecuteQueryAsync<BookingDays>(req);
                    return (List<BookingDays>)result;
                }
                else
                {
                    var result = await ExecuteQueryAsync<BookingDays>("Exec [dbo].[Usp_Booking_List]   '" + CommunityId + "','" + Date.ToString("MM-dd-yyyy HH:mm:ss") + "','" + UserId + "','" + Bookingid + "','" + IsMyBooking + "'");
                    //var result = await ExecuteQueryAsync<BookingDays>(req);
                    return (List<BookingDays>)result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
          
        }
        public async Task<long> SaveBookings(Bookings bookings)
        {
            try
            {
                var Query = "Exec [dbo].[Usp_Booking_AddBooking] " + bookings.CommunityId + ",'" + bookings.Title + "','" + bookings.Days + "','" + bookings.StartDate?.ToString("MM-dd-yyyy HH:mm:ss") + "','" + bookings.EndDate?.ToString("MM-dd-yyyy HH:mm:ss") + "'," + bookings.NoOfBooking + ",'" + bookings.StartTime?.ToString("MM-dd-yyyy HH:mm:ss") + "','" + bookings.EndTime?.ToString("MM-dd-yyyy HH:mm:ss") + "','" + bookings.HostName + "','" + bookings.Email + "'," + bookings.IsNotificationActive + "," + bookings.CreatedBy;
                return (long)await ExecuteScalarAsync(Query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<dynamic> GetBookingMemberDetails(long? BookingId)
        {
            return ExecuteQueryAsync("Exec [dbo].[Usp_Booking_Members] " + BookingId).Result?.ToList();
        }
        public async Task<List<Bookings>> PostNewBookingAsync()
        {
            return QueryAll<Bookings>().Where(e => e.IsActive == true).ToList();
        }
        public async Task<List<Bookings>> GetBookingActiveItemsAsync()
        {
            var result = QueryAll<Bookings>().Where(e => e.IsActive == true && e.EndDate >= DateTime.Now.Date).OrderBy(x => x.Title).ToList();

            foreach (var item in result)
            {

            }

            return (List<Bookings>)result;
        }
        public async Task<int> DeleteBookingActiveItemsAsync(long id)
        {
            Bookings delete = new Bookings();
            delete.Id = id;
            delete.IsActive = false;
            var fields = Field.Parse<Bookings>(x => new
            {
                x.IsActive

            });
            var updaterow = await UpdateAsync<Bookings>(entity: delete, fields: fields);
            if (updaterow >= 0)
            {
                updaterow = await ExecuteNonQueryAsync("Exec [dbo].[usp_Bookingdelete] " + id);
            }
            return updaterow;

        }
        public async Task<IEnumerable<dynamic>> GetActiveBookingById(long Id)
        {
            try
            {
                return await ExecuteQueryAsync<dynamic>("Exec [dbo].[USP_GetEventDataById] '" + Id + "'");
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<IEnumerable<dynamic>> GetBookingDetails(long BookingId, bool IsBookingStatus)
        {
            return await ExecuteQueryAsync<dynamic>("Exec [dbo].[USP_GetBookingDetails] '" + BookingId + "','" + IsBookingStatus + "'");
        }
        public async Task<IEnumerable<dynamic>?> BookingAttendance(DateTime Date, long Id)
        {
            try
            {
                var result = await ExecuteQueryAsync<dynamic>("Exec [dbo].[Usp_Planner_BookingAttendance] '" + Date.ToString("MM-dd-yyyy") + "','" + Id + "'");
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<IEnumerable<GetAttendance>> GetBookingAttendance(long CommunityId)
        {
            try
            {
                var result = QueryAll<Bookings>().Where(e => e.IsActive == true && e.CommunityId == CommunityId).Select(x => new GetAttendance
                {
                    Title = x.Title,
                    Id = x.Id
                }).OrderByDescending(y => y.Id).ToList();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<dynamic>> GetBookingDays(long MasterBookingId)
        {
            return await ExecuteQueryAsync<dynamic>("Exec [dbo].[Usp_BookingDays] '" + MasterBookingId + "'");
        }
        public async Task<dynamic> BookedMembers(long BookindDayId)
        {
            return await ExecuteQueryAsync<dynamic>("Exec [dbo].[Usp_Booking_BookedMembers] '" + BookindDayId + "'");
        }
        public async Task<IEnumerable<Bookings>?> BookedSpaces(long primayCommmunityId)
        {
            try
            {
                return await ExecuteQueryAsync<Bookings>("Exec [dbo].[Usp_Booking_BookingSpaces] ' " + primayCommmunityId + "'");
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<dynamic> BookingRegistered(long BookingId)
        {
            return await ExecuteQueryAsync<dynamic>("Exec [dbo].[Usp_BookingRegistered] '" + BookingId + "'");
        }

        #endregion

        #region "Ticktet"

        public async Task<long> ScannerProfile(long UserId, int PermissionId, long communityId)
        {

            var result = await ExecuteNonQueryAsync("Exec [dbo].[Usp_Planner_ScannerProfile] '" + UserId + "','" + PermissionId + "'," + communityId);
            return result;

        }
        public async Task<IEnumerable<dynamic>> GetManageUser(long PermissionId, long communityId)
        {
            var result = await ExecuteQueryAsync<dynamic>("Exec [dbo].[USP_pLanner_UserManage]' " + PermissionId + "'," + communityId);
            return result;
        }

        public async Task<int> DeleteUserManagement(long id)
        {
            CustomerPermission delete = new CustomerPermission();
            delete.Id = id;
            delete.IsActive = false;
            delete.UpdateModifiedByAndDateTime();
            var fields = Field.Parse<CustomerPermission>(x => new
            {
                x.IsActive,
                x.ModifiedBy,
                x.ModifiedDate
            });
            var updaterow = await UpdateAsync<CustomerPermission>(entity: delete, fields: fields);
            return updaterow;
        }
        public async Task<int> UpdateUserManagement(long id, bool DeniedOrGranted)
        {

            try
            {
                CustomerPermission update = new CustomerPermission();
                update.Id = id;
                update.DeniedOrGranted = DeniedOrGranted == true ? 1 : 0;
                var fields = Field.Parse<CustomerPermission>(x => new
                {
                    x.DeniedOrGranted
                });
                var updaterow = await UpdateAsync<CustomerPermission>(entity: update, fields: fields);
                return updaterow;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<List<CustomerDetails>> GetFirstName(long CustomerId)
        {
            return QueryAll<CustomerDetails>().Where(G => G.IsActive == true && G.CustomerId == CustomerId).ToList();
        }
        public async Task<List<Event>> GetEventEmail(long Id)
        {
            var result = QueryAll<Event>().Where(G => G.IsActive == true && G.Id == Id).ToList();
            result[0].CommunityName = QueryAll<Communities>().Where(x => x.IsActive == true && x.Id == result[0].CommunityId)?.FirstOrDefault()?.OrgName ?? "";
            return result;
        }

        public async Task<List<Communities>> PlannerEmailDetails(long communityId, long EventId)
        {
            var results = await ExecuteQueryAsync<Communities>("exec [dbo].[USP_Planner_GetEmailDetails]" + " '" + communityId + " '" + EventId + "'");
            if (results != null)
                return (List<Communities>)results;
            else
                return null;
        }

        //public async Task<IEnumerable<dynamic>?> GetRsvpDetails(long GroupId, long EventId)
        //{
        //    var result = await ExecuteQueryAsync<dynamic>("Exec [dbo].[USP_GetUpcomingEventInviteesCount] '" + GroupId + "','" + EventId + "'");
        //    return result;
        //}

        public async Task<bool> UpdateCheckin(long EnviteeId, bool Checkin)
        {
            EventInvitees eventInvitees = new EventInvitees() { Id = EnviteeId, ModifiedBy = 101, ModifiedDate = DateTime.Now, IsActive = false, Ischeckedin = Checkin };
            var fields = Field.Parse<EventInvitees>(x => new
            {
                x.Ischeckedin,
                x.IsActive,
                x.ModifiedBy,
                x.ModifiedDate
            });
            var updatedrow = await UpdateAsync<EventInvitees>(entity: eventInvitees, fields: fields);
            if (updatedrow > 0)
                return true;
            else
                return false;
        }

        public async Task<int> updateScheduleNotify(long id)
        {
            Event upgrade = new Event();
            upgrade.Id = id;
            upgrade.IsScheduleNotificationSent = true;
            var fields = Field.Parse<Event>(x => new
            {
                x.IsScheduleNotificationSent

            });
            var updaterow = await UpdateAsync<Event>(entity: upgrade, fields: fields);
            return updaterow;
        }

        public async Task<int> AddPaidDocument(PaidDocument item)
        {
            try
            {
                //var key = await InsertAsync<PaidDocument, int>(item);
                int key = await ExecuteScalarAsync<int>("Exec [dbo].Usp_Planner_PaidDocument " +
                item.DocumentId.ToString() + "," + item.UserId.ToString());
                return key;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        #endregion
    }
}

