using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Mapper;
using Google.Apis.Http;

namespace Circular.Services.Safety
{
    public interface ISafetyService
    {

        #region Safety- Wellness
        Task<CommunityGuidanceWellnessOptions?> GetGuidance(long CommunityId);
		Task<long> UpdateGuidance(CommunityGuidanceWellnessOptions communityWellnessButtons);
		Task<int> WellnessCounselling(CommunityGuidanceWellness communityGuidanceWellness);
		Task<CommunityGuidanceWellness?> GetWellnessCounsellingByID(long CommunityId);
		Task<long> UpdateWellnessCounselling(CommunityGuidanceWellness communityGuidanceWellness);
		public Task<CommunityGuidanceWellness?> GetOverview(long CommunityId);
        #endregion

        #region Safety-API (Report, Sick)
        public Task<bool> ReportItAsync(BullyingReports bullyingReports);
        public Task<List<Communities>> reportemail(long CommunityId);
		public Task<List<IncidentType>> GetIncident(long IncidentTypeId);
		public Task<List<Communities>> sicknotemail(long CommunityId);
		public Task<bool> SubmitSickNoteAsync(Sicknotes sicknotes);
        #endregion

        #region Safety-Modal Bind
        public Task<List<CommunityClasses>> GetaClassNameAsync(long Community);
		public Task<List<StaffClasses>> GetTeacherNameAsync(long Community);
        #endregion

        #region Safety -Vehicle
        public Task<int> VehiclesAsync(Vehicles data);
        public Task<List<Vehicles>> GetVehiclesAsync(long Community);
		public Task<Vehicles> GetQRCodeAsync(long id);
		long deletevehicleAsync(long id);
		#endregion

		#region Safety-Price
		public Task<int> PostPriceDisplayAsync(CommunityTransportPass price);
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

		#region Safety-Attendance QR
		public QR GetAttendanceQRCode(long id);
		#endregion

		#region Safety-TravellStatus
		public Task<List<TransportVehcile>> GetTravellerStatusAsync(long CommunityId, long VehicleId, string TravelDate);
		#endregion

		#region Safety - SickNotesSubmission
		public Task<List<Sicknotes>> GetSickNotesAsync(long Customerid, long communityid, long classid, string Fromdatesick, long Teacherid);
		#endregion

		#region Safety- Attendance 
		public Task<List<AttendanceResult>> GetAttendanceRegistryAsync(AttendanceRegistryRequestDTO attendanceRegistryRequestDTO);
        #endregion




        #region "Safety"
        long SaveQRCodeAttendence(string AttendanceScannerImage, long CommunityId);
		public Task<List<Communities>> ShowQRCodeAsync(long id);





        public Task<int> SafetyAsync(CommunityClasses safety);
        Task<IEnumerable<dynamic>?> AttendanceSafety(string Mobileno, int PermissionId, long communityId);
		Task<int> DeleteUserManagementSafety(long Id);
		Task<IEnumerable<dynamic>?> GetManageUserSafety(long PermissionId,long communityId);
		Task<int> UpdateAttendanceScannerAsycn(long id, bool IsScannerActive, int PermissionId);



		


		public Task<List<Vehicles>> PostVehiclesAsync();
        public Task<List<CommunityTransportPass>> PostPriceDisplayAsync();
		public Task<List<TicketDays>> PostNewTicketAsync();



		//START API
		public Task<int> AddVehicleList(Core.Entity.Vehicles vehicles);
		public Task<IEnumerable<Vehicles>?> GetVehicleList(long id,long CommunityId);		
		public Task<int> VehicleTransportonoff(Core.Entity.TransportVehcile transportvehicles);
		#endregion
	}
}
