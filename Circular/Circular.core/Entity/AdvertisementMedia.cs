using RepoDb.Attributes;

namespace Circular.Core.Entity;
[Map("tblAdvertisementMedia")]

    public class AdvertisementMedia : BaseEntity
    {
        public long? AdvertisementId { get; set; }

        public string Title { get; set; }
        public string Summary { get; set; }

        public string  CustomHeading { get; set; }
        public string Url { get; set; }

        public string Media { get; set; }

        public string MediaType { get; set; }

        public override void ApplyKeys()
        {

        }
    }

