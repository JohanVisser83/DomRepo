using Circular.Core.Entity;
using Circular.Core.DTOs;
using Org.BouncyCastle.Tls.Crypto;

namespace CircularWeb.Models
{
    public class CommonModel: BaseModel
    {
        public CommonModel()
        {          
        }
        public IEnumerable<Customers> lstMobileno { get; set; }
        public IEnumerable<Tickets> lstTicketsale { get; set; }

    }
}

       
    

