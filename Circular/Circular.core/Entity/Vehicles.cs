using RepoDb.Attributes;
namespace Circular.Core.Entity;

[Map("tblVehicles")]

public class Vehicles : BaseEntity
{      
    public Vehicles() 
    {
		if (QRCode == null)
			QRCode = new QR();
	}
    public string? VechilesName { get; set; }
    public string? VechilesDesc { get; set; }
    public long? CommunityId { get; set; }
    public QR? QRCode { get; set; }
    public override void ApplyKeys()
    {

    }
 

}
