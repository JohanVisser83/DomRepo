using System.ComponentModel.DataAnnotations;

namespace Circular.Core.DTOs
{
	public class PriceDisplayQRCodeModelDTO
	{
		[Display(Name = "Enter QRCode Text")]
		public string QRCodeText { get; set; }

	}
}
