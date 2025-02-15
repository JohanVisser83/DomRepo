using RepoDb.Attributes;
namespace Circular.Core.Entity
{
    [Map("tblsicknotes")]
    public class Sicknotes : BaseEntity
    {
       
        public long? CommunityId { get; set; }
        public long? CustomerId { get; set; }   
        public long? ForCustomerId { get; set; }
        public DateTime? Fromdate { get; set; }
        public DateTime? Todate { get; set; }
        public string? Url { get; set; }
        public string? FullName { get; set; }
        public string? ForCustomerName { get; set; }
		
		public string SickFromDate { get; set; }
		public string SickToDate { get; set; }
		public long Teacherid { get; set; }
		public override void ApplyKeys()
        {

        }
    }
	public class SickNoteListResponse
	{
		public SickNoteListResponse()
		{
			this.SickNoteGroups = new List<SickNoteGroupResponse>();
		}
		public List<SickNoteGroupResponse> SickNoteGroups { get; set; }

	}
	public class SickNoteGroupResponse
	{
		public SickNoteGroupResponse()
		{
			this.SickNotesList = new List<Sicknotes>();
		}
		public DateTime? SickNoteDate { get; set; }
		public List<Sicknotes>? SickNotesList { get; set; }
	}

}
