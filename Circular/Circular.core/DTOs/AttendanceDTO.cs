namespace Circular.Core.DTOs
{
    public class AttendanceDTO
    {
        public long? CommunityId { get; set; }
        public long? StudentId { get; set; }
        public long? ClassId { get; set; }
        public DateTime? Date { get; set; }
        public bool? Ispresent { get; set; }
        public long? Teacherid { get; set; }
    }
}
