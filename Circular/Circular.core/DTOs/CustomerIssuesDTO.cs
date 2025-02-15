namespace Circular.Core.DTOs
{
    public class CustomerIssuesDTO
    {
        public long? CustomerId { get; set; }
        public string Name { get; set; }
        public long CommunityId { get; set; }

        public string? Mobile { get; set; }
       
        public string? Attachment { get; set; }
        public string DeviceName { get; set; }
        public string IssueDescription { get; set; }
    }

	public class QRScanRequest
	{
		public string? Type { get; set; }
		public long? LoggedInCustomerId { get; set; }
		public long? ReferenceId { get; set; }
		public long? OptionalReferenceId { get; set; }
		public long? AdditionalReferenceId { get; set; }
		public string? OptionalFirstParameter { get; set; }
		public string? OptionalSecondParameter { get; set; }
		public string? OptionalThirdParameter { get; set; }

		public decimal? OptionalFourthParameter { get; set; }
		public decimal? OptionalFifthParameter { get; set; }
		public decimal? OptionalSixthParameter { get; set; }

		public bool? OptionalSeventhParameter { get; set; }
		public bool? OptionalEighthParameter { get; set; }
		public bool? OptionalNinethParameter { get; set; }
	}
}
