using Microsoft.AspNetCore.Http;
namespace Circular.Core.DTOs
{
    public class CommunityGuidanceWellnessDTO : BaseEntityDTO
    {                    
		public long CommunityId { get; set; }
		public string? CoverImage { get; set; }
		public string? IncidentEmail { get; set; }
		public string? Overview { get; set; }
		public string? LandingPage { get; set; }		
        public IFormFile? Image { get; set; }
    }
}
