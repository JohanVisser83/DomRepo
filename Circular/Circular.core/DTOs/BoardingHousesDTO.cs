namespace Circular.Core.DTOs
{
    public class BoardingHousesDTO
    {
        public string? Name { get; set; }
        public long Position { get; set; }
        public string? About { get; set; }
        public string? ProfileImage { get; set; }
        public string? Email { get; set; }
        public bool IsExcecutive { get; set; }
        public long? OrgId { get; set; }
    }
}
