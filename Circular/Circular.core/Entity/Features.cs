using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("mtblFeatures")]

public class Features : BaseEntity
{
	public string? FeatureName { get; set; }
	public string? IconImage { get; set; }
	public string? BackgroundColor { get; set; }
	public string? code { get; set; }
	public string? ParentCode { get; set; }
	public string? SequenceNo { get; set; }

    public override void ApplyKeys()
    {

    }
}
