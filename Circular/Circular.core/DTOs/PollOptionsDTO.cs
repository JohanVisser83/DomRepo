namespace Circular.Core.DTOs
{
    public class PollOptionsDTO
    {
        public PollOptionsDTO()
        {
            if(Results == null)
                this.Results = new List<PollResultsDTO>();
        }
        public long PollId { get; set; }
        public string OptionText { get; set; }

        public long AnswersCount { get; set; }
        public double AnswersPercentage { get; set; }




        public List<PollResultsDTO> Results { get; set; }


    }
}
