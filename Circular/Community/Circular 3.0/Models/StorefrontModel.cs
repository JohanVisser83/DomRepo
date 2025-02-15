using Circular.Core.Entity;
using Microsoft.Identity.Client;
namespace CircularWeb.Models
{
    public class StorefrontModel : BaseModel
    {
        public StorefrontModel()
        {
            this.StoreData = new List<CustomerStore>();
            this.ListProductData = new List<stockInventory>();
            this.ListCategoryData = new List<StoreProductCategory>();
            this.ListpreorderTimeList = new List<PreOrderTimeSlots>();
            this.Listofproducts = new List<Products>();
            this.currencyModel = new CurrencyModel();
            this.ListOrderDetails = new List<Order>();
            this.ListofSchedule = new List<StorefrontSchedule>();
            this.ListOfCart = new List<AddCart>();

        }

        public IEnumerable<CustomerStore> StoreData { get; set; }

        public IEnumerable<stockInventory> ListProductData { get; set; }
        public IEnumerable<StoreProductCategory> ListCategoryData { get; set; }

        public IEnumerable<PreOrderTimeSlots> ListpreorderTimeList { get; set; }
        public CurrencyModel currencyModel { get; set; }
        public IEnumerable<CustomerDetails> Organizers { get; set; }

        public IEnumerable<Products> Listofproducts { get; set; }

        public IEnumerable<Order> ListOrderDetails { get; set; }

        public IEnumerable<StorefrontSchedule> ListofSchedule { get; set; }

        public IEnumerable<AddCart> ListOfCart { get; set; }


    }
}
