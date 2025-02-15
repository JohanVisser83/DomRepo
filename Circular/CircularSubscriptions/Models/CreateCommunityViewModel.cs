using Circular.Core.Entity;





namespace CircularSubscriptions.Models
{
    public class CreateCommunityViewModel
    {
        public CreateCommunityViewModel()
        {
            this.lstCountryName = new List<Country>();
            this.CommunityLogo = new List<CommunitySignUp>();
           
        }

        public IEnumerable<Country> lstCountryName { get; set; }

        public IEnumerable<CommunitySignUp> CommunityLogo { get; set; }
        public string  AId { get; set; }    

    }
}
