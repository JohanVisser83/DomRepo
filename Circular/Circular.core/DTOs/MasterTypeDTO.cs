using System.ComponentModel;

namespace Circular.Core.DTOs
{
    public class MasterTypeDTO
    {
        public string masterType { get; set; }


        [DefaultValue(false)]
        public bool allRecords { get; set; }

        [DefaultValue(0)]
        public long customerId { get; set; }

    }


    public class MasterTypeHouseClassesDTO
    {
        public string masterType { get; set; }
        public long CommunityId { get; set; }

    }
}
