namespace Circular.Core.DTOs
{
    public class UpdateUserTypeDTO
    {
        public long CustomerId { get; set; }
        public long userTypeId { get; set; }
        public long modifiedBy { get; set; }
    }
}
