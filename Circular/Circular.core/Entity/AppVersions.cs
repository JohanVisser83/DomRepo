using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblAppVersions")]
	public class AppVersions : BaseEntity
	{
		public string? Type { get; set; }
		public decimal? PreviousVersion { get; set; }
		public decimal? NewVersion { get; set; }
		public int? IsMandatory { get; set; }
		public override void ApplyKeys()
		{

		}
	}
