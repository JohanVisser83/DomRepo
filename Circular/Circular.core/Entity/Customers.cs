using RepoDb.Attributes;
namespace Circular.Core.Entity;
[Map("tblCustomers")]
public class Customers : BaseEntity
{
    public Customers()
    {
        if (CustomerCommunities == null)
            CustomerCommunities = new List<CustomerCommunity>();
        if (CustomerGroups == null)
            CustomerGroups = new List<CustomerGroups>();
        if(BankAccounts == null)
            BankAccounts = new List<CustomerBankAccounts> ();
        if (QRCode == null)
            QRCode = new QR();
        if (privateSpaces == null)
            privateSpaces = new List<PrivateSpace>();

    }
    public Guid UserId { get; set; }
    public string? CountryCode { get; set; }
    public string Mobile { get; set; }
    public string? PrimaryEmail { get; set; }
    public string CurrentTimeZone { get; set; } 

    public bool IsPasswordSet { get; set; } = true;
    public bool IsPasscodeSet { get; set; } = true;

    public string? Passcode { get; set; }
    public string? Password { get; set; }
    public string? CustomerCode { get; set; }
    public bool IsExternalSignUp { get; set; }
    public bool IsBlocked { get; set; }
    public long UnreadNotifications { get; set; } = 0;
    public decimal WalletBalance { get; set;} = 0.00m;
    public string CountryFlag { get; set; }

    public long UnreadMessages { get; set; } = 0;


    public string Environment { get; set; }
    public string SubscriptionStatus { get; set; }

    public QR QRCode { get; set; }
    public CustomerCommunity PrimaryCommunity
    {
        get
        {
            if (CustomerCommunities != null)
            {
                return (CustomerCommunities.Where(c => c.IsPrimary == true).FirstOrDefault()) ?? null;
            }
            else
                return null;
        }
    }

    public bool IsAdmin { get {

            if (CustomerCommunities != null)
            {
                return (CustomerCommunities.Where(c => c.IsPrimary == true).FirstOrDefault())?.IsAdmin??false;
            }
            else
                return false;
        } }

    public bool IsPaymentRestricted
	{
		get
		{
			if (CustomerCommunities != null && CustomerCommunities.Count > 0)
			{
                if(CustomerCommunities.Where(c => c.IsPrimary == true).Count() > 0)
				return (CustomerCommunities.Where(c => c.IsPrimary == true).FirstOrDefault().IsPaymentRestricted);
                else
                    return false;
			}
			else
				return false;
		}
	}

	public bool IsEWalletOff
	{
		get
		{
				return false;
		}
	}

	public List<CustomerCommunity>? CustomerCommunities { get; set; }
    public CustomerDetails? CustomerDetails { get; set; }
    public List<CustomerGroups>? CustomerGroups { get; set; }

    public List<CustomerBankAccounts> BankAccounts { get; set; }
	public List<AppVersions> AppVersions { get; set; }

	public AdminConfigurations AdminConfigurations { get; set; }

    public List<PrivateSpace> privateSpaces { get; set; }


    public override void ApplyKeys()
    {

    }
    public void FillInitialValues()
    {
        IsExternalSignUp = (IsExternalSignUp == null) ? false : IsExternalSignUp;
        CustomerCode = (CustomerCode == null || CustomerCode == "") ? Guid.NewGuid().ToString() : CustomerCode;
    }
    public Customers FillInitialValues(String Default = "")
    {
        IsExternalSignUp = (IsExternalSignUp == null) ? false : IsExternalSignUp;
        CustomerCode = (CustomerCode == null || CustomerCode == "") ? Guid.NewGuid().ToString() : CustomerCode;
        return this;
    }
}

public class CustomerInfo
{
    public CustomerInfo()
    {
    }

    public int IsPasswordSet { get; set; } = 0;
    public string CountryFlag { get; set; }

    public decimal WalletBalance { get; set; } = 0.00m;

    public long UnreadNotifications { get; set; } = 0;

    public long UnreadMessages { get; set; } = 0;

}

