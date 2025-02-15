namespace Circular.Core.DTOs
{
	public class AppVersionsDTO
	{
		public string? Type { get; set; }
		public decimal? PreviousVersion { get; set; }
		public decimal? NewVersion { get; set; }
		public int? IsMandatory { get; set; }
	}
}
