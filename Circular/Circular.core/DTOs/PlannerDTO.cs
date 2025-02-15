using Microsoft.AspNetCore.Http;
namespace Circular.Core.DTOs
{
    public class PlannerDTO
    {
        public long PlannerTypeId { get; set; }
        public long CommunityId { get; set; }
        public string? title { get; set; }
        public string? Description { get; set; }
        public string? IsArchived { get; set; }
        public IFormFile? Mediafile { get; set; }

        public string? Media { get; set; }
        public string? HyperLink { get; set; }
        public decimal? Price { get; set; }
        public bool IsBought { get; set; }
        public string PlannerTypeName { get; set; }

    }

    public class PlannerAddDTO
    {
        public long PlannerTypeId { get; set; }
        public long CommunityId { get; set; }
        public string? title { get; set; }
        public string? Description { get; set; }
        public string? IsArchived { get; set; }

        public string? Media { get; set; }
        public string? HyperLink { get; set; }
        public decimal? Price { get; set; }
        public bool IsBought { get; set; }
        public string PlannerTypeName { get; set; }

    }
    public class PlannerRequestDTO 
    {
        public long PlannerId { get; set; }
        public string PlannerType { get; set; }
        public long loggedInUserId { get; set; }
       
    }
}
