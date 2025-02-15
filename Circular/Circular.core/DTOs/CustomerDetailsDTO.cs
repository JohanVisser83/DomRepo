namespace Circular.Core.DTOs
{
    public class CustomerDetailsDTO
    {
        public long CustomerId { get; set; }
        public string? IDNumber { get; set; }
        public long CustomerTypeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public DateTime? DOB { get; set; }
        public string? ProfilePic { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public bool? IsTermsAccepted { get; set; }
        public long? UsertypeId { get; set; }
        public string? StudentNumber { get; set; }
        public long? staffId { get; set; }
        public long? HouseId { get; set; }
        public long? ClassId { get; set; }
        public bool? IsAccessCodeVerified { get; set; }
        public string? AlumniYear { get; set; }
        public string? SubscriptionStatus { get; set; }
        public int? SubscriptionStatusId { get; set; }
        public string? PaymentStatus { get; set; }
        public int? PaymentStatusId { get; set; }
        public string? MembershipType { get; set; }
        public int? MembershipTypeId { get; set; }

        public string? UserType { get; set; }

        public string? ClassName { get; set; }
        public string? Mobile { get; set; }
        public string? LinkedMembers { get; set; }
        public bool? IsBlocked { get; set; }

        public string? LinkedMemberMobiles { get; set; }

        public string? Name
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }


    }

    public class UserContactListDTO
    {

        public long communityId { get; set; }
        public string? Name { get; set; }

        public string? mobile { get; set; }

    }

    public class CustomerDetailsBasicDTO
    {
        public long CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DOB { get; set; }
        public string? ProfilePic { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? IDNumber { get; set; }
        public long? staffId { get; set; }
        public long? HouseId { get; set; }
        public long? ClassId { get; set; }
        public bool IsSignUpFlow { get; set; } = false;

    }
}
