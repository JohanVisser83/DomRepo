using Circular.Core.DTOs;
using Circular.Core.Entity;
using Circular.Framework.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using RepoDb;
namespace Circular.Data.Repositories.Transport
{
	public class TransportRepository : DbRepository<SqlConnection>, ITransportRepository
	{
		private readonly IHelper _helper;
		IHttpContextAccessor _httpContextAccessor;
		public TransportRepository(string connectionString, IHelper helper, IHttpContextAccessor httpContextAccessor) : base(connectionString)
		{
			_helper = helper ?? throw new ArgumentNullException(nameof(helper));
			_httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
		}
		public async Task<List<TicketDays>> GetTicketsAsync(long CommunityId, long CustomerId, DateTime StartDate, long? TicketId)
		{
			var result = ExecuteQueryAsync<TicketDays>("Exec [dbo].[Usp_Safety_Transports_Tickets] " +
				"" + CommunityId + ",'" + StartDate.ToString("yyyy-MM-dd") + "'," + CustomerId + "," + TicketId).Result.ToList();

			string QRCodePath = Directory.GetCurrentDirectory() + "/Uploads/QRs/Transport/";
			var browsePath = _httpContextAccessor?.HttpContext.Request.Scheme + "://" + _httpContextAccessor?.HttpContext.Request.Host + "/Uploads/QRs/Transport/";

			foreach(var ticket in result)
			{
                if (ticket.BookedIn == 1)
                {
                    QRCodePath = Directory.GetCurrentDirectory() + "/Uploads/QRs/Transport/";
                    string filename = _helper.EncryptUsingSHA1Hashing(ticket.CustomerTicketId.ToString()) + ".png";
                    ticket.TicketQR.QRCode = _helper.GetQRCode(ticket.CustomerTicketId.ToString(), filename, ref QRCodePath);
                    ticket.TicketQR.QRPath = browsePath + filename;
                }
            }

			return result;

		}
		public async Task<int> BuyTicket(long TicketDayId, long customerId)
		{
			int result = await ExecuteScalarAsync<int>("Exec [dbo].[Usp_Transport_BuyTicket] " + TicketDayId + "," + customerId);
			return result;
		}
		public async Task<int> BuyFlexipass(FlexipassBuyRequest flexiRequest)
		{
			int result = await ExecuteScalarAsync<int>("Exec [dbo].[Usp_Safety_Transport_Flexipass] " + flexiRequest.CommunityId + "," + flexiRequest.CustomerId
				+","+ flexiRequest.LoggedInCustomerId + "," + flexiRequest.Amount + "," + flexiRequest.distance);
			return result;
		}

		public async Task<List<Vehicles>> GetTransportVehciles(long CommunityId)
		{
			var result = QueryAll<Vehicles>().Where(x => x.CommunityId == CommunityId && x.IsActive == true).ToList();
			return result;

        }

    }
}
