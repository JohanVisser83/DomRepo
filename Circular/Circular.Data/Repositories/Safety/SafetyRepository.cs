using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Data.Repositories.Message;
using Circular.Framework.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RepoDb;
using RepoDb.Enumerations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace Circular.Data.Repositories.Safety
{
	public class SafetyRepository : DbRepository<SqlConnection>, ISafetyRepository
	{
		private readonly IHelper _helper;
		IHttpContextAccessor _httpContextAccessor;
		public SafetyRepository(string connectionString, IHelper helper, IHttpContextAccessor httpContextAccessor) : base(connectionString)
		{
			_helper = helper ?? throw new ArgumentNullException(nameof(helper));
			_httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
		}

		//Guidance & councelling
		public async Task<CommunityGuidanceWellnessOptions?> GetGuidance(long Id)
		{
			var user = QueryAsync<CommunityGuidanceWellnessOptions>(x => x.Id == Id && x.IsActive == true).Result.FirstOrDefault();
			return user;
		}

		public async Task<long> UpdateGuidance(CommunityGuidanceWellnessOptions communityWellnessButtons)
		{
			var updatedRows = 0;
			if (communityWellnessButtons.Id <= 0)
			{
				communityWellnessButtons.FillDefaultValues();
				updatedRows = await InsertAsync<CommunityGuidanceWellnessOptions, int>(communityWellnessButtons);
			}
			else
			{
				communityWellnessButtons.UpdateModifiedByAndDateTime();


				var fields = Field.Parse<CommunityGuidanceWellnessOptions>(e => new
				{
					e.ButtonName,
					e.Email,
					e.ContactNumber,
					e.BookSessionHyperlink,
					e.Information,
					e.WellnessOptionId,
					e.IsVisible,
					e.ModifiedDate,
					e.ModifiedBy
				});
				updatedRows = Update(entity: communityWellnessButtons, fields: fields);
			}

			return updatedRows;
		}

		public int GetCommunityWellnessIdUsingCommunityWellnessId(long CommunityWellnessId)
		{
			var guidance = QueryAsync<CommunityGuidanceWellnessOptions>(e => e.CommunityId == CommunityWellnessId && e.IsActive == true).Result;
			return (int)guidance.FirstOrDefault()?.Id;
		}

		public async Task<int> WellnessCounselling(CommunityGuidanceWellness communityGuidanceWellness)
		{
			var key = await InsertAsync<CommunityGuidanceWellness, int>(communityGuidanceWellness);
			return key;
		}

		public async Task<CommunityGuidanceWellness?> GetWellnessCounsellingByID(long CommunityId)
		{
			var communityGuidanceWellness = QueryAsync<CommunityGuidanceWellness>(x => x.CommunityId == CommunityId && x.IsActive == true).Result.FirstOrDefault();
			var communityGuidanceWellnessOptions = QueryAsync<CommunityGuidanceWellnessOptions>(x => x.CommunityId == CommunityId && x.IsActive == true).Result.ToList();

			communityGuidanceWellness.wellnessOptions = communityGuidanceWellnessOptions;

			return communityGuidanceWellness;
		}

		//UpdateGuidance
		public async Task<long> UpdateWellnessCounselling(CommunityGuidanceWellness communityGuidanceWellness)
		{
			CommunityGuidanceWellness oCGW = QueryAsync<CommunityGuidanceWellness>(x => x.Id == communityGuidanceWellness.Id).Result.FirstOrDefault();
			if (string.IsNullOrEmpty(communityGuidanceWellness.CoverImage) || communityGuidanceWellness.CoverImage == "undefined")
				communityGuidanceWellness.CoverImage = oCGW.CoverImage;

			communityGuidanceWellness.UpdateModifiedByAndDateTime();
			var fields = Field.Parse<CommunityGuidanceWellness>(e => new
			{
				e.CoverImage,
				e.IncidentEmail,
				e.Overview,
				e.LandingPage,
				e.ModifiedDate,
				e.ModifiedBy
			});
			var updatedRows = Update(entity: communityGuidanceWellness, fields: fields);
			return updatedRows;
		}


		public async Task<long> ReportItAsync(BullyingReports bullyingReports)
		{
			var Report = await InsertAsync<BullyingReports, int>(bullyingReports);
			return Report;
		}
		public async Task<List<Communities>> reportemail(long CommunityId)
		{
			var results = await ExecuteQueryAsync<Communities>("exec [dbo].[Usp_Setting_GetReportIT]" + " '" + CommunityId + "'");
			if (results != null)
				return (List<Communities>)results;
			else
				return null;
		}
		public async Task<List<IncidentType>> GetIncident(long IncidentTypeId)
		{
			var results = QueryAll<IncidentType>().Where(e => e.Id == IncidentTypeId && e.IsActive == true).ToList();
			if (results != null)
				return results;
			else
				return null;
		}

		public async Task<CommunityGuidanceWellness?> GetOverview(long CommunityId)
		{
			CommunityGuidanceWellness overview = QueryAll<CommunityGuidanceWellness?>().Where(x => x.CommunityId == CommunityId && x.IsActive == true).FirstOrDefault();
			overview.wellnessOptions = QueryAll<CommunityGuidanceWellnessOptions>().Where(x => x.CommunityId == CommunityId && x.IsActive == true && x.IsVisible == 1).ToList();
			return overview;
		}
		public async Task<long> SubmitSickNoteAsync(Sicknotes sicknotes)
		{
			var id = await InsertAsync<Sicknotes, long>(sicknotes);
			return id;
		}
		public async Task<List<Communities>> sicknotemail(long CommunityId)
		{
			var results = await ExecuteQueryAsync<Communities>("exec [dbo].[Usp_Setting_GetSickNoteSettings]" + " '" + CommunityId + "'");
			if (results != null)
				return (List<Communities>)results;
			else
				return null;
		}




		#region "Safety Vehicle"
		public async Task<int> VehiclesAsync(Vehicles data)
		{
			var key = await InsertAsync<Vehicles, int>(data);
			return key;
		}
		public async Task<List<Vehicles>> GetVehiclesAsync(long Community)
		{
			var results = QueryAll<Vehicles>().Where(e => e.CommunityId == Community && e.IsActive == true).ToList();
			if (results != null)
				return results;
			else
				return null;
		}
		public async Task<Vehicles> GetQRCodeAsync(long id)
		{
			Vehicles vehicle = QueryAll<Vehicles>().Where(e => e.Id == id && e.IsActive == true).ToList().FirstOrDefault();

			string QRCodePath = Directory.GetCurrentDirectory() + "/Uploads/QRs/Vehicle/";
			var browsePath = _httpContextAccessor?.HttpContext.Request.Scheme + "://" + _httpContextAccessor?.HttpContext.Request.Host + "/Uploads/QRs/Vehicle/";
			string filename = _helper.EncryptUsingSHA1Hashing(vehicle.Id.ToString()) + ".png";
			vehicle.QRCode.QRCode = _helper.GetQRCode(vehicle.Id.ToString(), filename, ref QRCodePath);
			vehicle.QRCode.QRPath = browsePath + filename;
			if (vehicle != null)
				return vehicle;
			else
				return null;
		}
		public long deletevehicleAsync(long id)
		{
			Vehicles deletedVehicle = new Vehicles();
			deletedVehicle.Id = id;
			deletedVehicle.IsActive = false;
			deletedVehicle.UpdateModifiedByAndDateTime();
			var fields = Field.Parse<Vehicles>(x => new
			{
				x.IsActive,
				x.ModifiedDate,
				x.ModifiedBy
			});
			var updaterow = Update<Vehicles>(entity: deletedVehicle, fields: fields);
			return updaterow;
		}

		#endregion

		#region Safety - Price
		public async Task<int> PostPriceDisplayAsync(CommunityTransportPass price)
		{
			var key = await InsertAsync<CommunityTransportPass, int>(price);
			return key;
		}
		public async Task<CommunityTransportPass> GetQRCodePriceAsync(long id)
		{
			var results = QueryAll<CommunityTransportPass>().Where(e => e.Id == id && e.IsActive == true).ToList().FirstOrDefault();
			string QRCodePath = Directory.GetCurrentDirectory() + "/Uploads/QRs/Vehicle/";
			var browsePath = _httpContextAccessor?.HttpContext.Request.Scheme + "://" + _httpContextAccessor?.HttpContext.Request.Host + "/Uploads/QRs/Vehicle/";
			string filename = _helper.EncryptUsingSHA1Hashing(results.Price.ToString()) + ".png";
			results.QRCode.QRCode = _helper.GetQRCode(results.Price.ToString(), filename, ref QRCodePath);
			results.QRCode.QRPath = browsePath + filename;
			if (results != null)
				return results;
			else
				return null;
		}
		public long DeletePriceDisplayCodeAsync(long id)
		{
			CommunityTransportPass deleteP = new CommunityTransportPass();
			deleteP.Id = id;
			deleteP.IsActive = false;
			deleteP.UpdateModifiedByAndDateTime();
			var fields = Field.Parse<CommunityTransportPass>(x => new
			{
				x.IsActive,
				x.ModifiedDate,
				x.ModifiedBy

			});
			var updaterow = Update<CommunityTransportPass>(entity: deleteP, fields: fields);
			return updaterow;
		}
		public async Task<List<CommunityTransportPass>> GetPriceDisplayAsync(long Community)
		{
			var results = QueryAll<CommunityTransportPass>().Where(e => e.OrgId == Community && e.IsActive == true).ToList();
			if (results != null)
				return results;
			else
				return null;
		}
		public async Task<List<Communities>> GetPricePerKmAsync(long CommunityId)
		{
			var results = await ExecuteQueryAsync<Communities>("exec [dbo].[USP_Safety_GetPricePerKmByCommunityId]" + " '" + CommunityId + "'");

			if (results != null)
				return (List<Communities>)results;
			else
				return null;
		}
		public long UpdatePriceperkmAsync(long CommunityId, decimal PricePerKm)
		{
			Communities updateppkm = new Communities();
			updateppkm.Id = CommunityId;
			updateppkm.PricePerKm = PricePerKm;
			updateppkm.UpdateModifiedByAndDateTime();
			var fields = Field.Parse<Communities>(x => new
			{
				x.PricePerKm,
				x.ModifiedBy,
				x.ModifiedDate
			});
			var updaterow = Update<Communities>(entity: updateppkm, fields: fields);
			return updaterow;
		}

		#endregion

		#region Safety-Ticket
		public async Task<long> AddNewTicketAsync(TicketDays ticket1)
		{
			try
			{
				return (long)await ExecuteScalarAsync("Exec [dbo].[Usp_Safety_AddTicket] "
				+ ticket1.CommunityId + ",'" + ticket1.TicketName + "','" + ticket1.TicketPrice + "','"
				+ ticket1.TicketCount + "','" + ticket1.StartDate.ToString("MM-dd-yyyy HH:mm:ss")
				+ "','" + ticket1.EndDate.ToString("MM-dd-yyyy HH:mm:ss") + "','" + ticket1.TicketTime.ToString("MM-dd-yyyy HH:mm:ss") + "'," + ticket1.CreatedBy);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public async Task<int> DeleteTicketAsync(long Id)
		{
			TicketDays delete = new TicketDays();
			delete.Id = Id;
			delete.IsActive = false;
			var fields = Field.Parse<TicketDays>(x => new
			{
				x.IsActive

			});
			var updaterow = Update<TicketDays>(entity: delete, fields: fields);
			return updaterow;
		}
		public async Task<List<TicketDays>> GetViewTicketAsync(long Id)
		{
			var results = QueryAll<TicketDays>().Where(x => x.Id == Id && x.IsActive == true).ToList();
			if (results != null)
				return results;
			else
				return null;
		}
		public async Task<List<CustomerTickets>> GetTicketSoldList(long Id)
		{
			var results = await ExecuteQueryAsync<CustomerTickets>("exec [dbo].[USP_Safety_TicketSold]" + " '" + Id + "'");
			if (results != null)
				return (List<CustomerTickets>)results;
			else
				return null;
		}
		public async Task<List<TicketDays>> GetTicketsaleitemAsync(long communityid)
		{
			var results = await ExecuteQueryAsync<TicketDays>("exec [dbo].[USP_Safety_GetTicketSales] " + communityid);
			if (results != null)
				return (List<TicketDays>)results;
			else
				return null;
		}
		#endregion

		#region Safety-Attendance QR
		public QR GetAttendanceQRCode(long id)
		{
			QR attendanceqr = new QR();
			string QRCodePath = Directory.GetCurrentDirectory() + "/Uploads/QRs/Vehicle/";
			var browsePath = _httpContextAccessor?.HttpContext.Request.Scheme + "://" + _httpContextAccessor?.HttpContext.Request.Host + "/Uploads/QRs/Vehicle/";
			string filename = _helper.EncryptUsingSHA1Hashing(id.ToString()) + ".png";
			attendanceqr.QRCode = _helper.GetQRCode(id.ToString(), filename, ref QRCodePath);
			attendanceqr.QRPath = browsePath + filename;
			return attendanceqr;
		}
		#endregion

		#region Safety-TravellStatus
		public async Task<List<TransportVehcile>> GetTravellerStatusAsync(long CommunityId, long VehicleId, string TravelDate)
		{
			List<TransportVehcile> results = new List<TransportVehcile>();
			if (string.IsNullOrEmpty(TravelDate))
				results = ExecuteQueryAsync<TransportVehcile>("exec [dbo].[Usp_Safety_GetTransportScanDetailsvech] "
				+ CommunityId + "," + VehicleId).Result.ToList();
			else
				results = ExecuteQueryAsync<TransportVehcile>("exec [dbo].[Usp_Safety_GetTransportScanDetailsvech] "
							+ CommunityId + "," + VehicleId + ",'" + TravelDate + "'").Result.ToList();

			return results;
		}

		#endregion

		#region Safety - SickNotesSubmission
		public async Task<List<Sicknotes>> GetSickNotesAsync(long Customerid, long CommunityId, long classid, string Fromdatesick, long Teacherid)
		{
			try
			{
				List<Sicknotes> results;
				if (string.IsNullOrEmpty(Fromdatesick))
					results = ExecuteQueryAsync<Sicknotes>("exec [dbo].[USP_Safety_SickNotes] " + Customerid + "," + CommunityId + ","
				   + classid + "," + Teacherid).Result.ToList();
				else
					results = ExecuteQueryAsync<Sicknotes>("exec [dbo].[USP_Safety_SickNotes] " + Customerid + "," + CommunityId + ","
					   + classid + "," + Teacherid + ",'" + Fromdatesick + "'").Result.ToList();

				return results;
			}
			catch (Exception ex)
			{
				return null;
			}

	}
		#endregion

		#region Safety- Attendance
		public async Task<List<AttendanceResult>> GetAttendanceRegistryAsync(AttendanceRegistryRequestDTO attendanceRegistryRequestDTO)
		{
			try
			{
				List<AttendanceResult> results;
				if (attendanceRegistryRequestDTO.STARTDATE == null || attendanceRegistryRequestDTO.STARTDATE.ToString() ==  "01-01-0001 00:00:00" || attendanceRegistryRequestDTO.STARTDATE.ToString() == "1/1/0001 12:00:00 AM")
				{
					attendanceRegistryRequestDTO.STARTDATE = DateTime.Now.Date;
				}
				//	results = ExecuteQueryAsync<AttendanceResult>("exec [dbo].[USP_Safety_GetCommunityAttendance]" 
				//		+ attendanceRegistryRequestDTO.Communityid + "," + attendanceRegistryRequestDTO.Classid  + "," + attendanceRegistryRequestDTO.Teacherid).Result.ToList();
				//else
				results = ExecuteQueryAsync<AttendanceResult>("exec [dbo].[USP_Safety_GetCommunityAttendance]" 
					+ attendanceRegistryRequestDTO.Communityid + "," + attendanceRegistryRequestDTO.Classid  + "," + attendanceRegistryRequestDTO.Teacherid + ",'" + attendanceRegistryRequestDTO.STARTDATE.ToString("MM-dd-yyyy HH:mm:ss") + "'").Result.ToList();

				 return results;
			}
			catch (Exception ex)
			{
				return null;
			}
		}
		#endregion






		#region "Safety"
		public Task<int> SafetyAsync(CommunityClasses safety)
		{
			throw new NotImplementedException();
		}
		public async Task<List<CommunityClasses>> GetaClassNameAsync(long Community)
		{
			IEnumerable<CommunityClasses> classes = QueryAll<CommunityClasses>().Where(e => e.IsActive == true);
			return classes.Where(e => (e.Code?.ToString() ?? "") == Community.ToString()).ToList();
		}
		public async Task<List<StaffClasses>> GetTeacherNameAsync(long Community)
		{
			var results = await ExecuteQueryAsync<StaffClasses>("exec [dbo].[USP_Safety_GetClassTeachers]" + " '" + Community + "'");
			if (results != null)
				return (List<StaffClasses>)results;
			else
				return null;
		}





		public long SaveQRCodeAttendence(string AttendanceScannerImage, long CommunityId)
		{
			Communities communitys = new Communities();

			communitys.AttendanceScannerImage = AttendanceScannerImage;
			communitys.Id = CommunityId;
			var fields = Field.Parse<Communities>(x => new
			{

				x.AttendanceScannerImage,
				x.Id
			});
			var updaterows = Update<Communities>(entity: communitys, fields: fields);
			return updaterows;
		}
		public async Task<List<Communities>> ShowQRCodeAsync(long id)
		{
			var results = QueryAll<Communities>().Where(e => e.Id == id && e.IsActive == true).ToList();
			if (results != null)
				return results;
			else
				return null;
		}

		

		
	


		public async Task<IEnumerable<dynamic>?> AttendanceSafety(string Mobileno, int PermissionId, long communityId)
		{
			var result = await ExecuteQueryAsync<dynamic>("Exec [dbo].[Usp_Safety_ScannerProfile] '" + Mobileno + "','" + PermissionId + "'," + communityId);
			return result;
		}

		public async Task<IEnumerable<dynamic>> GetManageUserSafety(long PermissionId, long communityId)
		{
			var result = await ExecuteQueryAsync<dynamic>("Exec [dbo].[USP_pLanner_UserManage] '" + PermissionId + "',"+ communityId);
			return result;
		}

		public async Task<int> DeleteUserManagementSafety(long Id)
		{
			var results = QueryAll<CustomerPermission>().Where(e => e.UserId == Id && e.IsActive == true && (e.PermissionId == 1 || e.PermissionId == 5)).ToList();
			foreach (CustomerPermission customerPermission in results)
			{
				customerPermission.IsActive = false;
				customerPermission.UpdateModifiedByAndDateTime();
				var fields = Field.Parse<CustomerPermission>(x => new
				{
					x.IsActive,x.ModifiedDate,x.ModifiedBy
				});
				var updaterow = Update<CustomerPermission>(entity: customerPermission, fields: fields);
			}

			return 1;
		}
		public async Task<int> UpdateAttendanceScannerAsycn(long id, bool DeniedOrGranted, int PermissionId)
		{
			try
			{
				var updatelist = QueryAll<CustomerPermission>().Where(e => e.UserId == id
				&& e.IsActive == true && (e.PermissionId == PermissionId)).ToList();
				CustomerPermission update = null;
				if (updatelist != null)
					 update = updatelist.FirstOrDefault();

				if (update != null)
					{
						update.DeniedOrGranted = DeniedOrGranted == true ? 1 : 0;
						update.UpdateModifiedByAndDateTime();
						var fields = Field.Parse<CustomerPermission>(x => new
						{
							x.DeniedOrGranted,
							x.ModifiedBy,
							x.ModifiedDate
						});
						var updaterow = await UpdateAsync<CustomerPermission>(entity: update, fields: fields);
						return updaterow;
					}
				else
				{
					update = new CustomerPermission();
					update.UserId = id;
					update.PermissionId = PermissionId;
					update.DeniedOrGranted = 1;
					update.FillDefaultValues();

					var x = await InsertAsync<CustomerPermission, long>(update);
					return 1;
				}
			}
			catch (Exception ex)
			{
				throw;
			}

		}

		public async Task<List<Vehicles>> PostVehiclesAsync()
        {
            return QueryAll<Vehicles>().Where(e => e.IsActive == true).ToList();
        }
        public async Task<List<CommunityTransportPass>> PostPriceDisplayAsync()
        {
            return QueryAll<CommunityTransportPass>().Where(e => e.IsActive == true).ToList();
        }   
		public async Task<List<TicketDays>> PostNewTicketAsync()
		{
			return QueryAll<TicketDays>().Where(e => e.IsActive == true).ToList();
		}


		

		//START API
		public async Task<int> AddVehicleList(Core.Entity.Vehicles vehicles)
		{
			var vehicle = await InsertAsync<Vehicles, int>(vehicles);
			return vehicle;
     	}
		public async Task<IEnumerable<Vehicles>?> GetVehicleList(long id, long CommunityId)
		{
			var vehiclelist = QueryAll<Vehicles?>().Where(x => x.Id == id && x.CommunityId ==CommunityId && x.IsActive== true).ToList();
			return (IEnumerable<Vehicles>)vehiclelist;
		}	
		public async Task<int> VehicleTransportonoff(Core.Entity.TransportVehcile transportvehicles)
		{
			var TransportVehcile1 = await InsertAsync<TransportVehcile, int>(transportvehicles);
			return TransportVehcile1;
		}
		public Task<IEnumerable<TransportVehcile>?> GetScanDetails(long CommunityId, long vehicleId, DateTime ScanOnDate, DateTime ScanOffDate, bool ScanOn, bool Scanoff)
		{
			throw new NotImplementedException();
		}
		
        #endregion

    }


}
