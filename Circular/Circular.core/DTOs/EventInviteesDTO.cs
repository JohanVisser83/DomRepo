﻿namespace Circular.Core.DTOs
{
    public class EventInviteesDTO
    {
        public long? EventId { get; set; }
        public long? Communityid { get; set; }
        public string? InviteeType { get; set; }
        public long? InviteeId { get; set; }
        public bool? IsPaid { get; set; }
        public bool? Ischeckedin { get; set; }
        public decimal? Amount { get; set; }
        public long? paidbyid { get; set; }
        public decimal? CheckinLatitude { get; set; }
        public decimal? CheckInLongitude { get; set; }
       
        public DateTime? CheckedinTime { get; set; }
        public long? TransactionId { get; set; }
       
    }
}
