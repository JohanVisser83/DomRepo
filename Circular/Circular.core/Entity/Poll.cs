using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblPoll")]
public class Poll : BaseEntity
{
    public Poll()
    {
        if (Options == null)
            Options = new List<PollOptions>();

       
    }

    public long? CommunityId { get; set; }
    public long? IsGroup { get; set; }
    public long? ReferenceId { get; set; }
    public string PollTitle { get; set; }
    public string? Question { get; set; }

    public string? Type { get; set; }

    public string? GroupName { get; set; }

    public string CommunityName { get; set; }
    public List<PollOptions> Options { get; set; }

    public long PollMemberCount { get; set; }
    public long PollResponseCount { get; set; }
    public long PollOutstandingCount { get; set; }

    
    public override void ApplyKeys()
    {

    }


}
