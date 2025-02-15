namespace Circular.Core.DTOs
{
	public class CommunityGuidanceWellnessOptionsDTO : BaseEntityDTO
	{
        public long CommunityId { get; set; }
        public int WellnessOptionId { get; set; }
        public string? ButtonName { get; set; }
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
        public string? BookSessionHyperlink { get; set; }
        public string? Information { get; set; }
        public int IsVisible { get; set; }
    }
}
