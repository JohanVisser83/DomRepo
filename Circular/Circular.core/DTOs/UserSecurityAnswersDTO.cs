namespace Circular.Core.DTOs
{
    public class UserSecurityAnswersDTO
    {
        public long QuestionId { get; set; }
        public long UserId { get; set; }
        public string? Answer { get; set; }
    }
}
