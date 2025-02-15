namespace Circular.Core.DTOs
{
    public class LinkedMembersDTO
    {
        public long FromCustId { get; set; }
        public long ToCustId { get; set; }
        public long LinkingStatusId { get; set; } = 100;

        public long LinkedMemberId { get; set; }
        public long RequestToFirstName { get; set; }
        public long RequestToLastName { get; set; }
    }

}
