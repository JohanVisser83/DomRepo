using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Data.Repositories.Planners;
using Circular.Framework.Middleware.Emailer;
using Circular.Framework.Notifications;
using Circular.Services.Email;
using Circular.Services.Notifications;
using NLog;

namespace Circular.Services.Planners
{
    public class PlannerService : IPlannerService
    {
        private readonly IPlannerRepository _PlannerRepository;
        INotificationService _notificationService;
        IMailService _mailService;
        ILogger _logger;
        public PlannerService(IPlannerRepository plannerRepository, INotificationService notificationService, IMailService mailService)
        {
            _PlannerRepository = plannerRepository ?? throw new ArgumentNullException(nameof(plannerRepository));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _logger = LogManager.GetLogger("database");
        }

        #region "Planner"
        public async Task<int> NewPlannerAsync(Planner planner)
        {
            planner.FillDefaultValues();
            return await _PlannerRepository.NewPlannerAsync(planner);
        }
        public async Task<List<Planner>> GetPlannerItemsAsync(string PlannerType, long plannerId, long? CommunityId)
        {
            return await _PlannerRepository.GetPlannerItemsAsync(PlannerType, plannerId, CommunityId);

        }

        public async Task<List<PaidDocument>> GetPaidDocument(long? customerId)
        {
            return await _PlannerRepository.GetPaidDocument(customerId);
        }
        public async Task<int> DeletePlannerItemsAsync(long id)
        {

            return await _PlannerRepository.DeletePlannerItemsAsync(id);
        }
        public async Task<List<PlannerType>> GetMasterPlannerTypeAsync()
        {
            return await _PlannerRepository.GetMasterPlannerTypeAsync();
        }

        public async Task<bool> EmailPlanner(EmailPlannerDTO emailPlannerDTO, long communityId)
        {

            Planner planner = GetPlannerItemsAsync("All", emailPlannerDTO.PlannerId, communityId).Result.FirstOrDefault();
            if (planner != null)
            {
                var username = await SaveCustomerName((long)emailPlannerDTO.CustomerId);

                MailRequest mailRequest = new MailRequest();
                mailRequest.FromUserId = emailPlannerDTO.CustomerId;
                mailRequest.To = emailPlannerDTO.Email;
                mailRequest.ReferenceId = emailPlannerDTO.PlannerId;
                MailSettings mailSettings = _mailService.EmailParameter(MailType.planner, ref mailRequest);
                //Parse the body
                string body = mailRequest.Body;
                string[] PlaceHolders = { "$Title", "$Desc", "$IsArchived", "$UrlHyper", "$TypeName", "$Media", "$firstname" };
                string[] Values = { planner.title, planner.Description, planner.IsArchived, planner.HyperLink, planner.PlannerTypeName, planner.Media.Replace("\\", "/"), username[0].FirstName };
                if (!string.IsNullOrEmpty(body) && (PlaceHolders.Length == Values.Length))
                {
                    for (int index = 0; index < PlaceHolders.Length; index++)
                        body = body.Replace(PlaceHolders[index], Values[index]);
                }
                mailRequest.Body = body;

                return await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);
            }
            else
                return false;
        }
        public async Task<List<CustomerDetails>> SaveCustomerName(long CustomerId)
        {
            return await _PlannerRepository.SaveCustomerName(CustomerId);
        }

        public async Task<List<Communities>> PlannerEmailDetails(long communityId, long EventId)
        {
            return await _PlannerRepository.PlannerEmailDetails(communityId, EventId);
        }
        public async Task<List<CustomerDetails>> GetFirstName(long CustomerId)
        {
            return await _PlannerRepository.GetFirstName(CustomerId);
        }
        public async Task<List<Event>> GetEventEmail(long Id)
        {
            return await _PlannerRepository.GetEventEmail(Id);
        }
        public async Task<IEnumerable<dynamic>?> GetCheckinView(long GroupId, long EventId)
        {
            return await _PlannerRepository.GetCheckinView(GroupId, EventId);
        }

        #endregion

        #region "Event\Schedule"
        public async Task<List<Event>> GetEventsAsync(long communityId, int IsAllUpcomingOrCompleted)
        {
            return await _PlannerRepository.GetEventsAsync(communityId, IsAllUpcomingOrCompleted);

        }
        public async Task<IEnumerable<dynamic>?> GetEvents(long Id, long CommunityId, long CustomerId)
        {
            return await _PlannerRepository.GetEvents(Id, CommunityId, CustomerId);

        }



        public async Task<EventListResponse?> Events(long Id, long CommunityId, long CustomerId, int IsAllUpcomingOrCompleted)
        {
            return await _PlannerRepository.Events(Id, CommunityId, CustomerId, IsAllUpcomingOrCompleted);

        }
        public async Task<int?> RegisterForEvents(long EventId, string currency, long CustomerId, long RegistrationForCustomerId, decimal Amount)
        {

            var FirstName = await GetFirstName((long)CustomerId);
            var Details = await GetEventEmail((long)EventId);
            var id = await _PlannerRepository.RegisterForEvents(EventId, currency, CustomerId, RegistrationForCustomerId, Amount);
            var Emails = await SendEmailPlanner((long)CustomerId);

            if (Amount > 0)
            {

                MailRequest mailRequest = new MailRequest();
                mailRequest.FromUserId = CustomerId;
                mailRequest.To = Emails[0].Email;
                mailRequest.ReferenceId = EventId;
                MailSettings mailSettings = _mailService.EmailParameter(MailType.Event_Email_Paid, ref mailRequest);

                string body = mailRequest.Body;
                string[] PlaceHolders = { "$FirstName", "$Title", "$Location", "$eventstartdate", "$starttime", " $OrgName", };
                string[] Values = { FirstName[0].FirstName, Details[0].Title, Details[0].Location, Details[0].EventStartDate.ToString("dd MMM yyyy"), Details[0].StartTime.ToString(@"hh\:mm"), Details[0].CommunityName };
                if (!string.IsNullOrEmpty(body) && (PlaceHolders.Length == Values.Length))
                {
                    for (int index = 0; index < PlaceHolders.Length; index++)
                        body = body.Replace(PlaceHolders[index], Values[index]);
                }
                mailRequest.Body = body;

                int result1 = Convert.ToInt16(await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings));
                return result1;
            }
            else
            {

                MailRequest mailRequest = new MailRequest();
                mailRequest.FromUserId = CustomerId;
                mailRequest.To = Emails[0].Email;
                mailRequest.ReferenceId = EventId;
                MailSettings mailSettings = _mailService.EmailParameter(MailType.Event_Email_Free, ref mailRequest);

                string body = mailRequest.Body;
                string[] PlaceHolders = { "$FirstName", "$Title", " $OrgName", "$organisername", "$Location", "$eventstartdate", "$starttime" };
                string[] Values = { FirstName[0].FirstName, Details[0].Title, Details[0].CommunityName, Details[0].OrganizerName, Details[0].Location, Details[0].EventStartDate.ToString("dd MMM yyyy"), Convert.ToString(Details[0].StartTime) };
                if (!string.IsNullOrEmpty(body) && (PlaceHolders.Length == Values.Length))
                {
                    for (int index = 0; index < PlaceHolders.Length; index++)
                        body = body.Replace(PlaceHolders[index], Values[index]);
                }
                mailRequest.Body = body;

                int result1 = Convert.ToInt16(await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings));
                return result1;
            }
        }
        public async Task<List<CustomerDetails>> SendEmailPlanner(long LoggedInCustomerId)
        {
            return await _PlannerRepository.SendEmailPlanner(LoggedInCustomerId);
        }
        //public async Task<int?> RegisterForEvents(long EventId, string currency, long CustomerId, long RegistrationForCustomerId, decimal Amount)
        //{
        //   return await _PlannerRepository.RegisterForEvents(EventId, currency, CustomerId, RegistrationForCustomerId, Amount);
        //}
        public async Task<int?> DeregisterForEvents(long invitationId)
        {
            return await _PlannerRepository.DeregisterForEvents(invitationId);
        }

        #endregion



        public async Task<int> SaveAsync(Event item, DateTime dtSchedule)
        {
            item.FillDefaultValues();
            item.TotalMembersAtCreation = _notificationService.GetMemberCount(item.GroupId ?? 0, item.CommunityId ?? 0, 0);
            int result = await _PlannerRepository.SaveAsync(item);
            try
            {

                if (result > 0)
                {
                    //if (item.Individual != 0)
                    //item.GroupId = -1;

                    BulkEventEmail bulkEventEmail = new BulkEventEmail();
                    bulkEventEmail.EventId = result;
                    bulkEventEmail.Scheduledeliverydate = item.ScheduleDate;
                    if (bulkEventEmail.Scheduledeliverydate == null)
                        bulkEventEmail.Scheduledeliverydate = DateTime.Now.Date;
                    bulkEventEmail.Scheduleddeliverytime = item.ScheduleTime;
                    if (bulkEventEmail.Scheduleddeliverytime == null)
                        bulkEventEmail.Scheduleddeliverytime = DateTime.Now.TimeOfDay;
                    bulkEventEmail.communityId = item.CommunityId ?? 0;

                    if ((item.GroupId ?? 0) == 0)
                    {
                        bulkEventEmail.IsGroup = 0;
                        bulkEventEmail.GroupId = 0;
                    }

                    else
                    {
                        bulkEventEmail.IsGroup = 1;
                        bulkEventEmail.GroupId = item.GroupId ?? 0;
                    }
                    await SendBulkEventEmail(bulkEventEmail);
                }

                //if (result > 0 && (item.ScheduleDate == null || dtSchedule <= DateTime.Now))
                //{
                //    if ((item.GroupId ?? 0) <= 0)
                //    {
                //        _notificationService.Notify(NotificationTypes.New_Event,
                //        NotificationTopics.Circular_community_ReferenceId.ToString().Replace("ReferenceId", item.CommunityId.ToString()),
                //        item.CommunityName, item.Title + " event is just added in your community."
                //        , result, item.TicketPrice, false, "", item.CreatedBy, item.CommunityId ?? 0, "", item.GroupId ?? 0);
                //    }
                //    else
                //    {
                //        _notificationService.Notify(NotificationTypes.New_Event,
                //        NotificationTopics.Circular_communityGroups_ReferenceId.ToString().Replace("ReferenceId", item.GroupId.ToString()),
                //        item.CommunityName, item.Title + " event is just added in your community group."
                //        , result, item.TicketPrice, false, "", item.CreatedBy, item.CommunityId ?? 0, "", item.GroupId ?? 0);
                //    }

                //    return result;
                //}
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<List<Planner>> PostAllNewItemAsync()
        {
            return await _PlannerRepository.PostAllNewItemAsync();
        }
        public async Task<int> updateScheduleNotify(long id)
        {
            return await _PlannerRepository.updateScheduleNotify(id);
        }
        public async Task<List<Bookings>> PostNewBookingAsync()
        {
            return await _PlannerRepository.PostNewBookingAsync();
        }
        public async Task<List<Bookings>> GetBookingActiveItemsAsync()
        {
            return await _PlannerRepository.GetBookingActiveItemsAsync();

        }
        public async Task<int> DeleteBookingActiveItemsAsync(long id)

        {

            return await _PlannerRepository.DeleteBookingActiveItemsAsync(id);


        }
        public async Task<int> DeleteUpcomingItemsAsync(long id)

        {

            return await _PlannerRepository.DeleteUpcomingItemsAsync(id);


        }
        public async Task<List<Event>> PostAddEventAsync()
        {
            return await _PlannerRepository.PostAddEventAsync();
        }
        public async Task<int> SendBulkEventEmail(BulkEventEmail bulkEventEmail)
        {
            bulkEventEmail.FillDefaultValues();
            return await _PlannerRepository.SendBulkEventEmail(bulkEventEmail);
        }

        #region "Booking"
        public async Task<List<BookingDays>> GetBookingDetails(long? CommunityId, DateTime Date, long? UserId, long? Bookingid, bool IsMyBooking)
        {
            return await _PlannerRepository.GetBookingDetails(CommunityId, Date, UserId, Bookingid, IsMyBooking);

        }
        public async Task<long> CustomerBooking(CustomerBooking customerBooking)
        {
            customerBooking.FillDefaultValues();
            long result = await _PlannerRepository.CustomerBooking(customerBooking);
            if (result > 0)
            {
                var bookingId = customerBooking.BookingId;

                //  var booking =  GetBookingActiveItemsAsync();
                var registered = await BookingRegistered(bookingId ?? 0);
                if (registered[0].IsNotificationActive)
                {
                    MailRequest mailRequest = new MailRequest();
                    mailRequest.FromUserId = customerBooking.UserId ?? 0;
                    mailRequest.To = registered[0].Email;
                    mailRequest.ReferenceId = bookingId;
                    MailSettings mailSettings = _mailService.EmailParameter(MailType.Booking_Notification, ref mailRequest);

                    string body = mailRequest.Body;
                    string[] PlaceHolders = { "$userfirstname", "$title", "$communityname", "$Date", "$Time", "$endtime" };
                    string[] Values = { registered[0].HostName, registered[0].Title, registered[0].CommunityName, registered[0].StartDate.ToString("dd MMM yyyy"), Convert.ToString(registered[0].StartTime), Convert.ToString(registered[0].EndTime) };
                    if (!string.IsNullOrEmpty(body) && (PlaceHolders.Length == Values.Length))
                    {
                        for (int index = 0; index < PlaceHolders.Length; index++)
                            body = body.Replace(PlaceHolders[index], Values[index]);
                    }
                    mailRequest.Body = body;

                    int result1 = Convert.ToInt16(await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings));
                    return result1;
                }

            }
            return result;
        }
        public async Task<int> CancelCustomerBooking(CustomerBooking customerBooking)
        {
            int result = await _PlannerRepository.CancelCustomerBooking(customerBooking);
            try
            {
                if (result > 0)
                {
                    var bookingId = customerBooking.BookingId;

                    //  var booking =  GetBookingActiveItemsAsync();
                    var registered = await BookingRegistered(bookingId ?? 0);
                    if (registered[0].IsNotificationActive)
                    {
                        MailRequest mailRequest = new MailRequest();
                        mailRequest.FromUserId = customerBooking.UserId ?? 0;
                        mailRequest.To = registered[0].Email;
                        mailRequest.ReferenceId = bookingId;
                        MailSettings mailSettings = _mailService.EmailParameter(MailType.Booking_Unregistered, ref mailRequest);

                        string body = mailRequest.Body;
                        string[] PlaceHolders = { "$userfirstname", "$title", "$communityname", "$Date", "$Time", "$endtime" };
                        string[] Values = { registered[0].HostName, registered[0].Title, registered[0].CommunityName, registered[0].StartDate.ToString("dd MMM yyyy"), Convert.ToString(registered[0].StartTime), Convert.ToString(registered[0].EndTime) };
                        if (!string.IsNullOrEmpty(body) && (PlaceHolders.Length == Values.Length))
                        {
                            for (int index = 0; index < PlaceHolders.Length; index++)
                                body = body.Replace(PlaceHolders[index], Values[index]);
                        }
                        mailRequest.Body = body;

                        int result1 = Convert.ToInt16(await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings));
                        return result1;
                    }

                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<long> SaveBookings(Bookings bookings)
        {
            bookings.FillDefaultValues();
            return await _PlannerRepository.SaveBookings(bookings);
        }
        public async Task<dynamic> GetBookingMemberDetails(long? BookingId)
        {
            return await _PlannerRepository.GetBookingMemberDetails(BookingId);
        }
        public async Task<dynamic> BookedMembers(long BookindDayId)
        {
            return await _PlannerRepository.BookedMembers(BookindDayId);
        }

        public async Task<dynamic> BookingRegistered(long BookingId)
        {
            return await _PlannerRepository.BookingRegistered(BookingId);
        }
        public async Task<IEnumerable<Bookings>?> BookedSpaces(long primayCommmunityId)
        {
            return await _PlannerRepository.BookedSpaces(primayCommmunityId);
        }


        #endregion

        public async Task<int> UpdateEvent(Event item)
        {
            item.FillDefaultValues();
            return await _PlannerRepository.UpdateEvent(item);
        }
        public async Task<List<EventInvitees>> ViewUpcomingItemsAsync(long EventId)
        {
            return await _PlannerRepository.ViewUpcomingItemsAsync(EventId);

        }
        public async Task<long> ScannerProfile(long UserId, int PermissionId, long communityId)
        {
            return await _PlannerRepository.ScannerProfile(UserId, PermissionId, communityId);
        }
        public async Task<IEnumerable<dynamic>?> GetManageUser(long PermissionId, long communityId)
        {
            return await _PlannerRepository.GetManageUser(PermissionId, communityId);
        }
        public async Task<int> DeleteUserManagement(long id)

        {
            return await _PlannerRepository.DeleteUserManagement(id);

        }
        public async Task<int> UpdateUserManagement(long id, bool DeniedOrGranted)

        {

            return await _PlannerRepository.UpdateUserManagement(id, DeniedOrGranted);

        }
        public async Task<IEnumerable<dynamic>?> BookingAttendance(DateTime Date, long Id)
        {
            return await _PlannerRepository.BookingAttendance(Date, Id);
        }
        public async Task<IEnumerable<GetAttendance>?> GetBookingAttendance(long CommunityId)
        {
            return await _PlannerRepository.GetBookingAttendance(CommunityId);
        }
        public async Task<IEnumerable<dynamic>> GetActiveBookingById(long Id)
        {
            return await _PlannerRepository.GetActiveBookingById(Id);
        }
        public async Task<IEnumerable<dynamic>> GetEventAttendace(long EventId)
        {
            return await _PlannerRepository.GetEventAttendace(EventId);
        }
        public async Task<IEnumerable<dynamic>> GetBookingDetails(long BookingId, bool IsBookingStatus)
        {
            return await _PlannerRepository.GetBookingDetails(BookingId, IsBookingStatus);
        }
        public async Task<IEnumerable<dynamic>> GetBookingDays(long MasterBookingId)
        {
            return await _PlannerRepository.GetBookingDays(MasterBookingId);

        }

        public async Task<bool> UpdateCheckin(long EnviteeId, bool Checkin)
        {
            return await _PlannerRepository.UpdateCheckin(EnviteeId, Checkin);
        }

        public async Task<List<EventNotificationDTO>> EventScheduleNotification()
        {
            return await _PlannerRepository.EventScheduleNotification();

        }

        public async Task<bool> SendBulkEmails()
        {

            string msg = "";
            string topic = "";
            long receiverId = 0;
            try
            {
                _logger.Info("Inside SendBulkEmails-Planner");
                List<BulkEventEmail?> result = await _PlannerRepository.GetBulkEmaildetails();
                if (result != null)
                {
                    _logger.Info("Bulk Event Email Exists");

                    foreach (BulkEventEmail bulkEmail in result)
                    {
                        _logger.Info("Bulk Event Email:" + bulkEmail.Title);

                        msg = "";
                        topic = "";
                        receiverId = 0;
                        List<CustomerDetails> customerList = new List<CustomerDetails>();
                        if (bulkEmail.GroupId == 0 || bulkEmail.GroupId == null)
                        {
                            topic = NotificationTopics.Circular_community_ReferenceId.ToString().Replace("ReferenceId", bulkEmail.communityId.ToString());
                            msg = bulkEmail.Title + " event is just added in your community.";
                            receiverId = 0;
                            customerList = await _PlannerRepository.SendAddEventEmail(bulkEmail.communityId, 0);
                        }
                        else
                        {

                            topic = NotificationTopics.Circular_communityGroups_ReferenceId.ToString().Replace("ReferenceId", bulkEmail.GroupId.ToString());
                            msg = bulkEmail.Title + " event is just added in your community group.";
                            receiverId = bulkEmail.GroupId;
                            customerList = await _PlannerRepository.SendAddEventEmail(bulkEmail.GroupId, 1);
                        }

                        _notificationService.Notify(NotificationTypes.New_Event, topic, bulkEmail.CommunityName, msg
                            , bulkEmail.EventId, bulkEmail.Amount ?? 0, false, "", bulkEmail.CreatedBy, bulkEmail.communityId, "", receiverId);

                        _logger.Info("Bulk Event Email notification sent:" + bulkEmail.Title);

                        if (customerList != null)
                        {
                            _logger.Info("Starting to send Bulk Event Email:" + customerList.Count().ToString());
                            int i = 1;
                            foreach (CustomerDetails cd in customerList)
                            {
                                _logger.Info("Sending Bulk Event Email:" + i.ToString());
                                MailRequest mailRequest = new MailRequest();
                                mailRequest.FromUserId = bulkEmail.Eventcustomer;
                                mailRequest.ReferenceId = bulkEmail.IsGroup == 0 ? bulkEmail.communityId : bulkEmail.GroupId;
                                MailSettings mailSettings;
                                //if (bulkEmail.PaymentStatus?.ToLower() == "free")
                                //    mailSettings = _mailService.EmailParameter(MailType.Event_Email_Free, ref mailRequest);
                                //else if(bulkEmail.PaymentStatus?.ToLower() == "Paid")
                                //    mailSettings = _mailService.EmailParameter(MailType.Event_Email_Paid, ref mailRequest);

                                //else

                                mailSettings = _mailService.EmailParameter(MailType.Event_Notify, ref mailRequest);

                                mailRequest.To = cd.Email;
                                mailRequest.Body = mailRequest.Body.Replace("$Title", bulkEmail.Title);
                                mailRequest.Body = mailRequest.Body.Replace("$OrgName", bulkEmail.CommunityName);
                                mailRequest.Body = mailRequest.Body.Replace("$FirstName", cd.Name);
                                mailRequest.Body = mailRequest.Body.Replace("$organisername", bulkEmail.OrganiserName);
                                mailRequest.Body = mailRequest.Body.Replace("$eventstartdate", bulkEmail.EventStartDate?.ToString("dd MMM yyyy"));
                                mailRequest.Body = mailRequest.Body.Replace("$starttime", bulkEmail.StartTime.ToString(@"hh\:mm"));
                                mailRequest.Body = mailRequest.Body.Replace("$endtime", bulkEmail.EndTime.ToString(@"hh\:mm"));
                                mailRequest.Body = mailRequest.Body.Replace("$Location", bulkEmail.Location);
                                mailRequest.Body = mailRequest.Body.Replace("$Community Name2", bulkEmail.CommunityName);

                                await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);
                                _logger.Info("Bulk Event Email sent:" + i.ToString());
                                i++;
                            }
                            await _PlannerRepository.UpdateIsSentForBukEmail(bulkEmail);
                        }
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<int> AddPaidDocument(PaidDocument item)
        {

            item.FillDefaultValues();
            return await _PlannerRepository.AddPaidDocument(item);
        }
    }
}
