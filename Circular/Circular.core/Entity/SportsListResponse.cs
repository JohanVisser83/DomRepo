using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Core.Entity
{
    public class SportsListResponse
    {
        public SportsListResponse()
        {
            this.SportsGroups = new List<SportsGroupResponse>();
         
        }
        public List<SportsGroupResponse> SportsGroups { get; set; }
      

    }

        public class SportsGroupResponse
        {
            public SportsGroupResponse()
            {
                this.SportsList = new List<Sports>();
            }
            public DateTime SportsStartDate { get; set; }
            public List<Sports>? SportsList { get; set; }

        }
    }




