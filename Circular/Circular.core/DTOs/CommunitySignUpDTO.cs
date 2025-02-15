using Circular.Core.Entity;
using Microsoft.AspNetCore.Http;


namespace Circular.Core.DTOs
{
    public class CommunitySignUpDTO
    {

        public long CustomerId { get; set; }
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime? DOB { get; set; }

        public string? Country { get; set; }

        public string? CommunityName { get; set; }

        public string? Currency { get; set; }

        public string? CommunityLogo { get; set; }

        public IFormFile? Logo { get; set; }

        public string? AffiliateCode { get; set; }

        public string? CountryCode { get; set; }

        public string? Description { get; set; }

        public string? Mobile { get; set; }

        public bool? IsTermAccepted { get; set; }

        public bool? Signupflow { get; set; }

        public long MemberCount { get; set; }

        public decimal MonthlySubsFee { get; set; }

        public decimal MonthlySupportsFee { get; set; }

        public decimal OnceOffFee { get; set; }

        public decimal TotalMonthlySubsFee { get; set; }

        public string CurrencyToken { get; set; }

        public string Email { get; set; }

        public string CountryName { get; set; }


    }
}
