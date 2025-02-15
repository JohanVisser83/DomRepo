using Circular.Core.Entity;
using System.ComponentModel.DataAnnotations;

namespace Circular.Core.DTOs
{
    public class CommunityTransportPassDTO
    {
		public CommunityTransportPassDTO()
		{
			if (QRCode == null)
				QRCode = new QR();
		}
		public long OrgId { get; set; }
        public string Title { get; set; }
        public decimal? Price { get; set; }
        public string? ExpiryDate { get; set; }
		public QR? QRCode { get; set; }
	}

}
