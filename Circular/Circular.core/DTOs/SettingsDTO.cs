namespace Circular.Core.DTOs
{
    public class SettingsDTO
    {
        public long? Id { get; set; } 
        public long? communityId { get; set; }
        public string? Key { get; set; }
        public string? Value { get; set; }
        public string? Description { get; set; }
    }
}
