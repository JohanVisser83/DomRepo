namespace Circular.Core.DTOs
{
    public class PollResultsDTO
    {
        public long PollId { get; set; }
        public long UserId { get; set; }
        public long SelectedOptionId { get; set; }

        public string? CustomerName { get; set; }

       

    }
}
