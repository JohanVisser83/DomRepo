using Circular.Core.Entity;

namespace CircularWeb.Models
{
    public class FinanceModel : BaseModel
    {
        public FinanceModel()
        {
            this.lstCountry = new List<Country>();
            this.lstMasterBanks = new List<Banks>();
            this.Ewallet = new Ewallet();
       
            this.lstbankDetails = new List<CustomerBankAccounts>();
            this.lstwithdrawalFeatures = new List<WithdrawalFeature>();
            this.currencyModel = new CurrencyModel();

        }
        public IEnumerable<Country> lstCountry { get; set; }
        public IEnumerable<dynamic> lstbankDetails { get; set; }
        public IEnumerable<Banks> lstMasterBanks { get; set; }
        public IEnumerable<WithdrawalFeature> lstwithdrawalFeatures { get; set; }
        public Settings lstsettings { get; set; }
        public CurrencyModel currencyModel { get; set; }    
        public Ewallet Ewallet { get; set; }    

    }

    public class search
    {
        public string? Search { get; set; }
    }
}
