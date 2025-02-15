

namespace Circular.Core.DTOs
{
    public class SubscriptionBillingDTO
    {

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Address { get; set; }

        public string Apartment { get; set; }

        public string Country { get; set; }
        public string City { get; set; }
       
        public string PostalCode { get; set; }

        public string Mobile { get; set; }

        public string Note { get; set; }

        public string ChoosedPackage { get; set; }

        public long VistiorId { get; set; }

        public long CustomerId { get; set; }
    }
}
