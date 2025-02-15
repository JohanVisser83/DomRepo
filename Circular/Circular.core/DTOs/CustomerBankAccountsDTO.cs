namespace Circular.Core.DTOs
{
    public class CustomerBankAccountsIdDTO
    {
        public long customerBankAccountId { get; set; }
    }
    public class CustomerBankAccountsDTO
    {
        public long CommunityId { get; set; }
        public long CustomerId { get; set; }
        public long? StoreId { get; set; }
        public long BankId { get; set; }
        public long BankAccountTypeId { get; set; }
        public string AccountNumber { get; set; }
        public string BranchCode { get; set; }
        public string HolderName { get; set; }
        public string? NickName { get; set; }
		public string? IBANRoutingNumber { get; set; }

		
		public int AccountOwnerTypeId { get; set; }

        public string BankName { get; set; }
        public string BankAccountType { get; set; }
        public string BankLogo { get; set; }
        public long BankCountryId { get; set; }
        public long countryId { get; set; }


        //public string? Country { get; set; }
       
    }
}
