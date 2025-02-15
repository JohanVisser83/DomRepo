using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblCommunityTransportPass")]
public class CommunityTransportPass : BaseEntity
{
	public CommunityTransportPass()
	{
		if (QRCode == null)
			QRCode = new QR();
	}

	public long OrgId { get; set; }
    public string Title { get; set; }
    public decimal? Price { get; set; }
    public string? ExpiryDate { get; set; }
	public QR? QRCode { get; set; }

	public override void ApplyKeys()
    {

    }

  
}
