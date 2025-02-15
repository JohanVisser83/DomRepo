using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circular.Core.Entity
{
    public class CoachListResponse
    {
        public CoachListResponse()
        {

            this.SportsCoach = new List<CoachGroupResponse>();
        }
        public List<CoachGroupResponse> SportsCoach { get; set; }

    }

    public class CoachGroupResponse
    {
        public CoachGroupResponse()
        {
            this.SportsCoachList = new List<SportsTeamMember>();
        }
        public long CoachId { get; set; }
        public List<SportsTeamMember>? SportsCoachList { get; set; }
       
    }
}




