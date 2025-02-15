namespace Circular.Core.DTOs
{
    public class PollDTO
    {
        public PollDTO() 
        {
            if (Options == null)
                Options = new List<PollOptionsDTO>();
            
        }
        public long? id { get; set; }
        public long? CommunityId { get; set; }
        public bool? IsGroup { get; set; }
        public long ReferenceId { get; set; }
        public string PollTitle { get; set; }
        public string? Question { get; set; }

        public string? Type { get; set; }

        public string? GroupName { get; set; }
        public List<PollOptionsDTO> Options { get; set; }

        public long PollMemberCount { get; set; }
        public long PollResponseCount { get; set; }
        public long PollOutstandingCount { get; set; }


    }
}
