using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Data.Repositories.Safety;
using Circular.Framework.Middleware.Emailer;
using Circular.Services.Email;
using Org.BouncyCastle.Math;

namespace Circular.Services.Safety
{
	public class SafetyService : ISafetyService
	{
		private readonly ISafetyRepository _safetyRepository;
        IMailService _mailService;

        public SafetyService(ISafetyRepository Safetyrepository, IMailService mailService)
		{
            _safetyRepository = Safetyrepository;
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        }

		#region Safety- Wellness
		public async Task<CommunityGuidanceWellnessOptions?> GetGuidance(long Id)
        {
            return await _safetyRepository.GetGuidance(Id);
        }
        public async Task<long> UpdateGuidance(CommunityGuidanceWellnessOptions communityWellnessButtons)
        {
            return await _safetyRepository.UpdateGuidance(communityWellnessButtons);
        }
		public async Task<int> WellnessCounselling(CommunityGuidanceWellness communityGuidanceWellness)
        {
            communityGuidanceWellness.FillDefaultValues();
            return await _safetyRepository.WellnessCounselling(communityGuidanceWellness);
        }
        public async Task<CommunityGuidanceWellness?> GetWellnessCounsellingByID(long CommunityId)
        {
            return await _safetyRepository.GetWellnessCounsellingByID(CommunityId);
        }
		public async Task<long> UpdateWellnessCounselling(CommunityGuidanceWellness communityGuidanceWellness)
        {
            communityGuidanceWellness.FillDefaultValues();
            return await _safetyRepository.UpdateWellnessCounselling(communityGuidanceWellness);
        }
		public async Task<CommunityGuidanceWellness?> GetOverview(long CommunityId)
		{
			return await _safetyRepository.GetOverview(CommunityId);
		}
		#endregion

		#region Safety -API 
		public async Task<bool> ReportItAsync(BullyingReports bullyingReports)
		{

            bullyingReports.FillDefaultValues();
			try
			{
                var x = await _safetyRepository.ReportItAsync(bullyingReports);
                var getreportmail = await reportemail(bullyingReports.CommunityId);

				var getincedenttypename = await GetIncident(bullyingReports.IncidentTypeId);


				MailRequest mailRequest = new MailRequest();
                mailRequest.FromUserId = bullyingReports.CustomerId;
                mailRequest.ReferenceId = x;
                mailRequest.To = getreportmail[0].EmailAddressReport; ;
                MailSettings mailSettings = _mailService.EmailParameter(MailType.Helpline, ref mailRequest);
                string body = mailRequest.Body;


                string[] PlaceHolders = { "$IncidentTime", "$IncidentLocation", "$Description", "$ReportingPersonName", "$IncidentTypeId", "$RecipientName" ,"$orgname", "$Image" };
                string[] Values = { Convert.ToString(bullyingReports.IncidentTime), bullyingReports.IncidentLocation, getincedenttypename[0].Name, bullyingReports.ReportingPersonName,bullyingReports.Description, getreportmail[0].RecipientNameReport, getreportmail[0].OrgName,bullyingReports.Image ?? "#"};

                if (!string.IsNullOrEmpty(body) && (PlaceHolders.Length == Values.Length))
                {
                    //for (int index = 0; index < PlaceHolders.Length; index++)
                    //  body = body.Replace(PlaceHolders[index], Values[index]);
                    for (int index = 0; index < PlaceHolders.Length; index++)
                    {
                        if (PlaceHolders[index] == "$Image")
                        {
                            if (string.IsNullOrEmpty(Values[index]))
                            {
                                body = body.Replace(PlaceHolders[index], "<a href=''>NA</a>");
                            }
                            else
                            {
                                body = body.Replace(PlaceHolders[index], $"<a href=\"{Values[index]}\" target=\"_blank\"> View </a>");
                            }
                        }
                        else
                        {
                            body = body.Replace(PlaceHolders[index], Values[index]);
                        }
                    }
                }
               
                mailRequest.Body = body;
                await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);
                return x > 0 ? true : false;
            
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<List<Communities>> reportemail(long Community)
        {
            return await _safetyRepository.reportemail(Community);
        }
		public async Task<List<IncidentType>> GetIncident(long IncidentTypeId)
		{
			return await _safetyRepository.GetIncident(IncidentTypeId);
		}
		public async Task<bool> SubmitSickNoteAsync(Sicknotes sicknotes)
		{
			sicknotes.FillDefaultValues();
			try
			{
				var getemail = await sicknotemail((long)sicknotes.CommunityId);
				var id = await _safetyRepository.SubmitSickNoteAsync(sicknotes);
				MailRequest mailRequest = new MailRequest();
				mailRequest.FromUserId = sicknotes.CustomerId ?? 0;
				mailRequest.ReferenceId = id;
				mailRequest.To = getemail[0].SickNoteMailBox;

				MailSettings mailSettings = _mailService.EmailParameter(MailType.SickNotes, ref mailRequest);
				string body = mailRequest.Body;
				string[] PlaceHolders = { "$Url", "$fromdate", "$Todate", "$fullname", "$dateofsubmission", "$recipientname", "$forname" };
				string[] Values = { sicknotes.Url ?? "#", sicknotes.Fromdate?.ToString("dd MMM yyyy"), sicknotes.Todate?.ToString("dd MMM yyyy"), sicknotes.FullName, sicknotes.CreatedDate.ToString("dd MMM yyyy"), getemail[0].SickNoteRecipientName, sicknotes.ForCustomerName };
				if (!string.IsNullOrEmpty(body) && (PlaceHolders.Length == Values.Length))
				{
					//for (int index = 0; index < PlaceHolders.Length; index++)
					//body = body.Replace(PlaceHolders[index], Values[index]);
					for (int index = 0; index < PlaceHolders.Length; index++)
					{
						if (PlaceHolders[index] == "$Url")
						{
							if (string.IsNullOrEmpty(Values[index]))
							{
								body = body.Replace(PlaceHolders[index], "<a href=''>NA</a>");
							}
							else
							{
								body = body.Replace(PlaceHolders[index], $"<a href=\"{Values[index]}\" target=\"_blank\"> View </a>");
							}
						}
						else
						{
							body = body.Replace(PlaceHolders[index], Values[index]);
						}
					}
                 }
				mailRequest.Body = body;
				await _mailService.SaveAndSendMailAsync(mailRequest, mailSettings);
				return id > 0 ? true : false;
			}


			catch (Exception ex)
			{
				throw ex;
			}
		}
		public async Task<List<Communities>> sicknotemail(long Community)
		{
			return await _safetyRepository.sicknotemail(Community);
		}
#endregion

		#region Safety -Vehicle
		public async Task<int> VehiclesAsync(Vehicles data)
        {
            data.FillDefaultValues();
            return await _safetyRepository.VehiclesAsync(data);
        }
        public async Task<List<Vehicles>> GetVehiclesAsync(long Community)
        {
            return await _safetyRepository.GetVehiclesAsync(Community);
        }
		public async Task<Vehicles> GetQRCodeAsync(long id)
		{
			return await _safetyRepository.GetQRCodeAsync(id);
		}
		public long deletevehicleAsync(long id)
		{
			return _safetyRepository.deletevehicleAsync(id);
		}

		#endregion

		#region Safety - Price
		public async Task<int> PostPriceDisplayAsync(CommunityTransportPass price)
		{
			FillDefaultValues1(price);
			return await _safetyRepository.PostPriceDisplayAsync(price);
		}
		public async Task<CommunityTransportPass> GetQRCodePriceAsync(long id)
		{
			return await _safetyRepository.GetQRCodePriceAsync(id);
		}
		public long DeletePriceDisplayCodeAsync(long id)
		{
			return _safetyRepository.DeletePriceDisplayCodeAsync(id);
		}
		public async Task<List<CommunityTransportPass>> GetPriceDisplayAsync(long Community)
		{
			return await _safetyRepository.GetPriceDisplayAsync(Community);
		}
		public async Task<List<Communities>> GetPricePerKmAsync(long CommunityId)
		{
			return await _safetyRepository.GetPricePerKmAsync(CommunityId);
		}
		public long UpdatePriceperkmAsync(long CommunityId, decimal PricePerKm)
		{
			return _safetyRepository.UpdatePriceperkmAsync(CommunityId, PricePerKm);
		}

		#endregion

		#region Safety-Ticket
		public async Task<long> AddNewTicketAsync(TicketDays ticket1)
		{
			ticket1.FillDefaultValues();
			return await _safetyRepository.AddNewTicketAsync(ticket1);
		}
		public async Task<int> DeleteTicketAsync(long Id)
		{
			return await _safetyRepository.DeleteTicketAsync(Id);
		}
		public async Task<List<TicketDays>> GetViewTicketAsync(long Id)
		{
			return await _safetyRepository.GetViewTicketAsync(Id);
		}
		public async Task<List<CustomerTickets>> GetTicketSoldList(long Id)
		{
			return await _safetyRepository.GetTicketSoldList(Id);
		}
		public async Task<List<TicketDays>> GetTicketsaleitemAsync(long communityid)
		{
			return await _safetyRepository.GetTicketsaleitemAsync(communityid);
		}
		#endregion

		#region Safety-Attendance QR
		public QR GetAttendanceQRCode(long id)
		{
			return _safetyRepository.GetAttendanceQRCode(id);
		}
		#endregion

		#region Safety- TravelStatus
		public async Task<List<TransportVehcile>> GetTravellerStatusAsync(long CommunityId, long VehicleId, string TravelDate)
		{
			return await _safetyRepository.GetTravellerStatusAsync(CommunityId, VehicleId, TravelDate);
		}

		#endregion

		#region Safety - SickNotesSubmission
		public async Task<List<Sicknotes>> GetSickNotesAsync(long Customerid, long communityid, long classid, string Fromdatesick, long Teacherid)
		{
			return await _safetyRepository.GetSickNotesAsync(Customerid, communityid, classid, Fromdatesick, Teacherid);
		}

		#endregion

		#region Safety-Attendance
		public async Task<List<AttendanceResult>> GetAttendanceRegistryAsync(AttendanceRegistryRequestDTO attendanceRegistryRequestDTO)
		{
			return await _safetyRepository.GetAttendanceRegistryAsync(attendanceRegistryRequestDTO);
		}
		#endregion

		#region Safety- UserManagement

		public async Task<IEnumerable<dynamic>?> AttendanceSafety(string Mobileno, int PermissionId,long communityId)
		{
			return await _safetyRepository.AttendanceSafety(Mobileno, PermissionId, communityId);
		}
		public async Task<int> DeleteUserManagementSafety(long Id)
		{
			return await _safetyRepository.DeleteUserManagementSafety(Id);
		}
		public async Task<IEnumerable<dynamic>?> GetManageUserSafety(long PermissionId, long communityId)
		{
			return await _safetyRepository.GetManageUserSafety(PermissionId, communityId);
		}
		public async Task<int> UpdateAttendanceScannerAsycn(long id, bool DeniedOrGranted, int PermissionId)
		{
			return await _safetyRepository.UpdateAttendanceScannerAsycn(id, DeniedOrGranted, PermissionId);
		}
		#endregion

		#region Safety- modal
		public async Task<List<CommunityClasses>> GetaClassNameAsync(long Community)
		{
			return await _safetyRepository.GetaClassNameAsync(Community);
		}
		public async Task<List<StaffClasses>> GetTeacherNameAsync(long Community)
		{
			return await _safetyRepository.GetTeacherNameAsync(Community);
		}
		#endregion




		#region "Safety
		public Task<int> SafetyAsync(CommunityClasses safety)
		{
			throw new NotImplementedException();
		}
		public async Task<List<Communities>> ShowQRCodeAsync(long id)
		{
			return await _safetyRepository.ShowQRCodeAsync(id);
		}
		public long SaveQRCodeAttendence(string AttendanceScannerImage, long CommunityId)
		{
			return _safetyRepository.SaveQRCodeAttendence(AttendanceScannerImage, CommunityId);
		}

	   public async Task<List<CommunityTransportPass>> PostPriceDisplayAsync()
        {
            return await _safetyRepository.PostPriceDisplayAsync();
        }
	   private static void FillDefaultValues1(BaseEntity entity)
        {
                if (entity != null)
                {
                    entity.IsActive = true;
                    entity.CreatedBy = (entity.CreatedBy == null || entity.CreatedBy == 0) ? 101 : entity.CreatedBy;
                    entity.ModifiedBy = (entity.ModifiedBy == null || entity.ModifiedBy == 0) ? 101 : entity.ModifiedBy;
                    entity.CreatedDate = (entity.CreatedDate == null || entity.CreatedDate == DateTime.MinValue) ? DateTime.Now : entity.CreatedDate;
                    entity.ModifiedDate = (entity.ModifiedDate == null || entity.ModifiedDate == DateTime.MinValue) ? DateTime.Now : entity.ModifiedDate;
                }
        }
	   public async Task<List<TicketDays>> PostNewTicketAsync()
	   {
			 return await _safetyRepository.PostNewTicketAsync();
	   }
	   public async Task<List<Vehicles>> PostVehiclesAsync()
		{
			return await _safetyRepository.PostVehiclesAsync();
		}
		//API 
		public async Task<int> AddVehicleList(Core.Entity.Vehicles vehicles)
            {
                vehicles.FillDefaultValues();
                return await _safetyRepository.AddVehicleList(vehicles);
            }
       public async Task<IEnumerable<Vehicles>?> GetVehicleList(long id, long CommunityId)
            {
                return await _safetyRepository.GetVehicleList(id, CommunityId);
            }
        public async Task<int> VehicleTransportonoff(Core.Entity.TransportVehcile transportvehicles)
            {
                transportvehicles.FillDefaultValues();
                return await _safetyRepository.VehicleTransportonoff(transportvehicles);
            }
 	   #endregion





	}

}

