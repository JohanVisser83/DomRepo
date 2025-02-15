namespace Circular.Core.DTOs
{
    public class BaseEntityDTO
    {
        public long Id { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        //public DateTime CreatedDateOnly { get; set;}
        //public DateTime ModifiedDateOnly { get; set; }
    }
}
