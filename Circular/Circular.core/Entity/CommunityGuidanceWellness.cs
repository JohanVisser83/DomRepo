using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblCommunityGuidanceWellness")]
public class CommunityGuidanceWellness : BaseEntity
{    
	public CommunityGuidanceWellness()
	{
		wellnessOptions = new List<CommunityGuidanceWellnessOptions>();
	}
	public long CommunityId { get; set; }
	public string? CoverImage { get; set; }
	public string? IncidentEmail { get; set; }
	public string? Overview { get; set; }
	public string? LandingPage { get; set; }

	public List<CommunityGuidanceWellnessOptions> wellnessOptions { get; set; }
	public override void ApplyKeys()
    {
        
    }
}


