using System.ComponentModel.DataAnnotations;

namespace Circular.Core.DTOs
{
	public class QRCodeModelDTO
	{
		[Display(Name = "Enter QRCode Text")]
		public string QRCodeText { get; set; }
	}
}
