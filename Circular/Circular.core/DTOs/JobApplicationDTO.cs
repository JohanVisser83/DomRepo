namespace Circular.Core.DTOs
{
    public class JobApplicationDTO
    {
        public long JobId { get; set; }
        public long CustomerID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string IDNumber { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string SalaryExpectation { get; set; }
        public string Portfolio { get; set; }
        public string CoverLetter { get; set; }
        public string CV { get; set; }
    }
}
