using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblTransportVehcile")]

public class TransportVehcile : BaseEntity
{
    public long? CustomerId { get; set; }
    public long? CommunityId { get; set; }
    public long? VehicleId { get; set; }
    public long? DriverId { get; set; }
    public string ScanOnDate { get; set; }
    public string ScanOffDate { get; set; }
    public string DriverName { get; set; }



	public string? FullName { get; set; }
	public string? VechilesName { get; set; }
	public override void ApplyKeys()
    {

    }
    
}


