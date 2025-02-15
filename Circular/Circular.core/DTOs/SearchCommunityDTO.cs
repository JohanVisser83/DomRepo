namespace Circular.Core.DTOs
{
    public class SearchCommunityDTO
    {
        public string communityName {  get; set; }
        public long? communityId { get; set; }
        public long pagesize { get; set; }
        public long pagenumber { get; set; }
        public string? search { get; set; }
    }

    public class SearchUserDTO
    {
        public long communityId { get; set; }
        public string? searchText { get; set; }
    }

}
