using System;
using System.Collections.Generic;
using System.Text;

namespace CanastaChampions.Data.Models
{
    public class TeamModel
    {
        public long TeamID { get; set; }
        public long CompetitionID { get; set; }
        public long TeamMember1ID { get; set; }
        public long TeamMember2ID { get; set; }
    }
}
