﻿namespace Circular.Core.DTOs
{
    public class CommunityBankDTO
    {
        public long? BankId { get; set; }
        public string AccountNumber { get; set; }
        public long? AccountTypeId { get; set; }
        public string? Location { get; set; }
        public bool IsPrimary { get; set; }
        public long? CommunityId { get; set; }
        public string? AccountHolderName { get; set; }
        public string? BranchCode { get; set; }
        public string? PreferredReference { get; set; }
        public string? Email { get; set; }
        public string? CircularContract { get; set; }
        public string? NickName { get; set; }
    }
}
