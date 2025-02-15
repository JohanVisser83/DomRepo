using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblCustomerDetails")]
public class CustomerDetails : BaseEntity
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
    public string? StaffName { get; set; }
    public long? HouseId { get; set; }
    public string? HouseName { get; set; }
    public long? ClassId { get; set; }
    public string? ClassName { get; set; }
    public bool? IsAccessCodeVerified { get; set; }
    public string? AlumniYear { get; set; }
    public string? Mobile { get; set; }
    public string? LinkedMembers { get; set; }
    public string? LinkedMemberMobiles { get; set; }
    public bool? IsBlocked { get; set; }
    public bool IsAdmin { get; set; } = false;
    public string? Name
    {
        get
        {
            return FirstName + " " + LastName;
        }
    }
    public string? SubscriptionStatus { get; set; }
    public int? SubscriptionStatusId { get; set; }
    public string? PaymentStatus { get; set; }
    public int? PaymentStatusId { get; set; }
    public string? MembershipType { get; set; }
    public int? MembershipTypeId { get; set; }
    public bool IsSignUpFlow { get; set; } = false;

    public string? UserType { get; set; }

    public string TypeofUser { get; set; }  
    public override void ApplyKeys()
    {

    }
}

public class UserContactList
{
    public long? CustomerId { get; set; }
    public long communityId { get; set; }
    public string? Name { get; set; }
    public string? mobile { get; set; }


    public long? Memberid { get; set; }
    public string? MemberName { get; set; }
    public Guid UserId { get; set; }
}

public class clsStorefrontCustomerDetails
{
    public string? Name { get; set; }
    public string? MobileNo { get; set; }
    public long OrderNo { get; set; }
    public long TotalItem { get; set; }
    public decimal TotalPrice { get; set; }
    public clsStorefrontCustomerDetails()
    {
        this.lstcustomerProductDetails = new List<CustomerProductDetails>();
    }
    public List<CustomerProductDetails> lstcustomerProductDetails { get; set; }
}

public class CustomerProductDetails
{
    public string? ProductName { get; set; }
    public long ProductId { get; set; }
    public decimal Amount { get; set; }
    public int? Quantity { get; set; }
}



public class HQAllMemberDetails
{
    public long Id { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public DateTime? DOB { get; set; }

    public string? CommunityName { get; set;}

    public string? MembershipType { get; set;}

    public decimal? WalletBalance { get; set; } 

    public long CommunityId { get; set; } 

}

public class HQCommunityTransactionDetails
{
    public long TrasactionId { get; set; }
    public long TransactionFromID { get; set; }
    public long TransactionToID { get; set; }
    public string TransactionFromName { get; set; }
    public string Mobile { get; set; }
    public string TransactionToName { get; set; }
    public string CommunityName { get; set; }
    public string TransactionDateFormat { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public string TransactionType { get; set; }

}


