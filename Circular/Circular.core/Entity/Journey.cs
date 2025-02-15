using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblJourney")]

public class Journey : BaseEntity
{
    public long? CommunityId { get; set; }
    public long? DriverId { get; set; }
    public string? Title { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal? Price { get; set; }
    public DateTime? Date { get; set; }
    public string? Description { get; set; }




    public override void ApplyKeys()
    {

    }
}