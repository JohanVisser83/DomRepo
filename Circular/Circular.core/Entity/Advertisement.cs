using Microsoft.AspNetCore.Http;
using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblAdvertisement")]

public class Advertisement : BaseEntity
{
    public Advertisement()
    {
        if (AdvertisementMediaList == null)
            AdvertisementMediaList = new List<AdvertisementMedia>();
    }

    public long? CommunityId { get; set; }
    public string Title { get; set; }
    public string Summary { get; set; }
    public string CoverAdvertisementMedia { get; set; }
    public string TopBannerThumbnail { get; set; }
    public string Url { get; set; }

      

    public List<AdvertisementMedia> AdvertisementMediaList { get; set; }

    public override void ApplyKeys()
    {
        if (AdvertisementMediaList != null)
        {
            foreach (var item in AdvertisementMediaList)
            {
                item.AdvertisementId = Id;
                item.GUID = new Guid(Guid.NewGuid().ToString());
            }
        }
    }
}



