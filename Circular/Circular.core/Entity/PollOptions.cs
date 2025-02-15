using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblPollOptions")]
public class PollOptions : BaseEntity
{
    public PollOptions()
    {
        if (Results == null)
            Results = new List<PollResults>();
    }
    public long PollId { get; set; }
    public string OptionText { get; set; }

    public long AnswersCount { get; set; }

    public double AnswersPercentage { get; set; }


    public List<PollResults> Results { get; set; }



    public override void ApplyKeys()
    {
    }


}
