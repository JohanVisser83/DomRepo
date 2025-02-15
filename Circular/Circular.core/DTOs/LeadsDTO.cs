namespace Circular.Core.DTOs
{
    public class LeadsDTO
    {
        public string? Name { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? LeadType { get; set; }
        public string? AccessCode { get; set; }
        public bool? IsAccessCodeUsed { get; set; }
        public long? Communityid { get; set; }
        public string? EmailId { get; set; }
    }
}
