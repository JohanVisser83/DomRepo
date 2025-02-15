using Microsoft.AspNetCore.Http;

namespace Circular.Core.DTOs
{
    public class CommunityTeamProfileDTO : BaseEntityDTO
    {
        public long? CommunityId { get; set; }
        public string Name { get; set; }
        public string? Position { get; set; }
        public string? Email { get; set; }
        public string? Contact { get; set; }

        public IFormFile? Mediafile { get; set; }
        public IFormFile? ProfileImg { get; set; }
        public string ProfileImage { get; set; }

        public string ProfileName { get; set; }
        public string About { get; set; }
        public string Image { get; set; }
    }
}

