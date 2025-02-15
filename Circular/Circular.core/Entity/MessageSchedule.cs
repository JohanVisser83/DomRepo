using Microsoft.AspNetCore.Http;
using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblMessageSchedule")]
public class MessageSchedule : BaseEntity
{

    public long FromId { get; set; }
    public long ToId { get; set; }

    public long CommunityId { get; set; }   
    public string? MessageExchangeCode { get; set; }
    public long MessageTypeId { get; set; }
    public string? Message { get; set; }
    public string? MessageMedia { get; set; }
    public IFormFile? Mediafile { get; set; }

    public string? MessageMediaThumbnail { get; set; }
    public long? ReferenceId { get; set; }

    public DateTime Schedule { get; set; }

    public DateTime? ScheduleTime { get; set; }

    public override void ApplyKeys()
    {

    }
}
