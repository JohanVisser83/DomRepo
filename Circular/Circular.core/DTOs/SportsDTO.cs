using System;
namespace Circular.Core.DTOs
{
    public class SportsDTO
    {
        public string SportPDF { get; set; }
        public string SportsName { get; set; }
        public DateTime SportsDate { get; set; }
		public string? CoverImage { get; set; }
		public long CommunityId { get; set; }
    }
}
