using RepoDb.Attributes;

namespace Circular.Core.Entity;

[Map("tblUserSecurityAnswers")]

public class UserSecurityAnswers : BaseEntity
{
    public long QuestionId { get; set; }
    public long UserId { get; set; }
    public string? Answer { get; set; }



    public override void ApplyKeys()
    {

    }
}
