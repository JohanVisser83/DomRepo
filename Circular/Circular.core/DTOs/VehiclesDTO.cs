using Circular.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace Circular.Core.DTOs
{
    public class VehiclesDTO
    {
		public VehiclesDTO()
		{
			if (QRCode == null)
				QRCode = new QR();
		}
		public string? VechilesName { get; set; }
        public string? VechilesDesc { get; set; }
        public long? CommunityId { get; set; }
		public QR? QRCode { get; set; }


	}




}