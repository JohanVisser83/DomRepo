using Circular.Core.DTOs;
using Circular.Core.Entity;

namespace Circular.Data.Repositories.Safety
{
   public interface ISafetyRepository 
    {
        #region Safety- Wellness
        Task<CommunityGuidanceWellnessOptions> GetGuidance(long CommunityId);
		Task<long> UpdateGuidance(CommunityGuidanceWellnessOptions communityWellnessButtons);
		int GetCommunityWellnessIdUsingCommunityWellnessId(long CommunityWellnessId);	
        Task<int> WellnessCounselling(CommunityGuidanceWellness communityGuidanceWellness);
		Task<CommunityGuidanceWellness> GetWellnessCounsellingByID(long CommunityId);
		Task<long> UpdateWellnessCounselling(CommunityGuidanceWellness communityGuidanceWellness);
        Task<CommunityGuidanceWellness?> GetOverview(long CommunityId);
		#endregion

		#region Safety- API (Report, Sick)
		public Task<List<Communities>> sicknotemail(long CommunityId);
		Task<long> ReportItAsync(BullyingReports bullyingReports);
		public Task<List<Communities>> reportemail(long CommunityId);
		public Task<List<IncidentType>> GetIncident(long IncidentTypeId);
		Task<long> SubmitSickNoteAsync(Sicknotes sicknotes);

        #endregion

        
        Task<List<CommunityClasses>> GetaClassNameAsync(long Community);
        Task<List<StaffClasses>> GetTeacherNameAsync(long Community);

        #region Safety-Vehicle
        public Task<int> VehiclesAsync(Vehicles data);
        public Task<List<Vehicles>> GetVehiclesAsync(long Community);
		public Task<Vehicles> GetQRCodeAsync(long id);
		long deletevehicleAsync(long id);
		#endregion

		#region Safety-Price
		Task<int> PostPriceDisplayAsync(CommunityTransportPass price);
		public Task<CommunityTransportPass> GetQRCodePriceAsync(long id);
		long DeletePriceDisplayCodeAsync(long id);
		public Task<List<CommunityTransportPass>> GetPriceDisplayAsync(long Community);
		public Task<List<Communities>> GetPricePerKmAsync(long CommunityId);
		long UpdatePriceperkmAsync(long CommunityId, decimal PricePerKm);

		#endregion

		#region Safety-Ticket
		public Task<long> AddNewTicketAsync(TicketDays ticket1);
		Task<int> DeleteTicketAsync(long Id);
		public Task<List<TicketDays>> GetViewTicketAsync(long Id);
		public Task<List<CustomerTickets>> GetTicketSoldList(long Id);
		public Task<List<TicketDays>> GetTicketsaleitemAsync(long communityid);
		#endregion

		#region Safety -AttendanceQR
		public QR GetAttendanceQRCode(long id);
		#endregion

		#region Safety-Travell Status
		public Task<List<TransportVehcile>> GetTravellerStatusAsync(long CommunityId, long VehicleId, string TravelDate);
		#endregion

		#region Safety - SickNotesSubmission
		public Task<List<Sicknotes>> GetSickNotesAsync(long Customerid, long communityid, long classid, string Fromdatesick, long Teacherid);
		#endregion

		#region Safety-Attendance
		public Task<List<AttendanceResult>> GetAttendanceRegistryAsync(AttendanceRegistryRequestDTO attendanceRegistryRequestDTO);
		#endregion





		#region "Safety"


		Task<int> SafetyAsync(CommunityClasses safety);
	
		long SaveQRCodeAttendence(string AttendanceScannerImage, long CommunityId);
		public Task<List<Communities>> ShowQRCodeAsync(long id);

		

		

	
	
		Task<IEnumerable<dynamic>?> AttendanceSafety(string Mobileno, int PermissionId, long communityId);
	

		Task<int> DeleteUserManagementSafety(long Id);
		Task<IEnumerable<dynamic>?> GetManageUserSafety(long PermissionId, long communityId);
		Task<int> UpdateAttendanceScannerAsycn(long id, bool IsScannerActive, int PermissionId);



		


		public Task<List<Vehicles>> PostVehiclesAsync();
        public Task<List<CommunityTransportPass>> PostPriceDisplayAsync();	
		public Task<List<TicketDays>> PostNewTicketAsync();
        

		//START API
		Task<int> AddVehicleList(Core.Entity.Vehicles vehicles);
		Task<IEnumerable<Vehicles>?> GetVehicleList(long id, long CommunityId);
		Task<IEnumerable<TransportVehcile>?> GetScanDetails(long CommunityId, long vehicleId, DateTime ScanOnDate, DateTime ScanOffDate, bool ScanOn, bool Scanoff);
		Task<int> VehicleTransportonoff(Core.Entity.TransportVehcile transportvehicles);
          
        #endregion

    }
}
