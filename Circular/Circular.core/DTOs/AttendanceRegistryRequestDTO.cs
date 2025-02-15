namespace Circular.Core.DTOs
{
	public class AttendanceRegistryRequestDTO
	{
		public long Communityid { get; set; }
		public long Classid { get; set; }
		public DateTime STARTDATE { get; set; }
		public long Teacherid { get; set; }
       
    }
}
