using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblSports")]
public class Sports : BaseEntity
{
	public long Id { get; set; }
    public string SportsName { get; set; }
    public DateTime SportsDate { get; set; }
    public string SportPDF { get; set; }
	public string? CoverImage { get; set; }
	public long CommunityId { get; set; }
    
    public override void ApplyKeys()
    {

    }
}
