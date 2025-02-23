﻿namespace Circular.Core.DTOs
{
    public class CustomerIDProofsDTO
    {
        public long CustomerId { get; set; }
        public long DocTypeId { get; set; }
        public string? IdentificationNumber { get; set; }
        public string? IDFrontSideImage { get; set; }
        public string? IDBackSideImage { get; set; }
        public string? FICADOC { get; set; }
        public bool IsFICAApproved { get; set; }

    }
}
