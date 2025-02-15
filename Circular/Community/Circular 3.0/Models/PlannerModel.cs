using Circular.Core.Entity;

namespace CircularWeb.Models
{
    public class PlannerModel : BaseModel
    {
        public PlannerModel()
        {
         
            this.PlannerTypes = new List<PlannerType>();
            this.BookingsDays= new List<BookingDays>();
            this.Bookings = new List<Bookings>();
            this.Groups = new List<Groups>();
            this.UpcomingEvents = new List<Event>();
            this.currencyModel = new CurrencyModel();
            this.PassedEvents = new List<Event>();
            this.Organizers = new List<CustomerDetails>();
        }
        public IEnumerable<PlannerType> PlannerTypes { get; set; }
        public IEnumerable<Bookings> Bookings { get; set; }
        public IEnumerable<BookingDays> BookingsDays { get; set; }
        public IEnumerable<Event> UpcomingEvents { get; set; }
        public IEnumerable<Event> PassedEvents { get; set; }
        public IEnumerable<Groups> Groups { get; set; }
        public CurrencyModel currencyModel { get; set; }
        public IEnumerable<CustomerDetails> Organizers { get; set; }


    }
}
