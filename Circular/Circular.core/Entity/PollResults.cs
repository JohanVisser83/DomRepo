using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblPollAnswers")]
public class PollResults : BaseEntity
{

    public long PollId { get; set; }
    public long UserId { get; set; }
    public long SelectedOptionId { get; set; }

    public string? CustomerName { get; set; }

    public string Mobile { get; set; }

    public string OptionText { get; set; }  

    public string PollTitle { get; set; }

    public override void ApplyKeys()
    {

    }

}
