using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblAttendance")] 
public class Attendance : BaseEntity
{
    public long? CommunityId { get; set; }
    public long? CustomerId { get; set; }
    public long? ClassId { get; set; }
    public DateTime? Date { get; set; }
    public long? Ispresent { get; set; }
    public long? Teacherid { get; set; }
    public string? CustomerName { get; set; }
    public string? ClassName { get; set; }
    public string? TeacherName { get; set; }
    public override void ApplyKeys()
    {

    }
}

public class AttendanceResult : BaseEntity
{
	public long? CommunityId { get; set; }
	public long? CustomerId { get; set; }
	public long? ClassId { get; set; }
	public DateTime Date { get; set; }
	public long? Ispresent { get; set; }
	public long? Teacherid { get; set; }
	public string? CustomerName { get; set; }
	public string? ClassName { get; set; }
	public string? TeacherName { get; set; }
	public override void ApplyKeys()
	{

	}
}
