using Circular.Core.Entity;
namespace CircularWeb.Models
{
    public class AccountModel : BaseModel
    {
        public AccountModel() 
        {
            this.ActiveAccount = new List<CollectionAggregate>();
            this.ClosedAccount = new List<CollectionAggregate>();
            this.Organizers = new List<CustomerDetails>();
            this.Groups = new List<Groups>();
            this.lstdeleteaccount = new List<CollectionAggregate>();
            this.currencyModel = new CurrencyModel();

        }
       
        public IEnumerable <CollectionAggregate> ActiveAccount { get;set; }

        public IEnumerable<CollectionAggregate> ClosedAccount { get; set; }

        public IEnumerable<CustomerDetails> Organizers { get; set; }

        public IEnumerable<Groups> Groups { get; set;}

        public IEnumerable<CollectionAggregate> lstdeleteaccount { get; set; }

        public CurrencyModel currencyModel { get; set; }


    }

}
