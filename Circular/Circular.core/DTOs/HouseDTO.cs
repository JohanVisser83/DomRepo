namespace Circular.Core.DTOs
{
    public class HouseDTO
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Desc { get; set; }
        public string? Code { get; set; }   
        public long? CommunityId { get; set; }
    }
}
