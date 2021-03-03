using System;
using System.Collections.Generic;
using System.Text;

namespace CanastaChampions.DataAccess.Models
{
    public class CompetitionModel
    {
        public long CompetitionID { get; set; }
        public string CompetitionName { get; set; }
        public bool TeamsAreFixed { get; set; }
        public bool TeamsAreRandomised { get; set; }
        public bool LogicallyDeleted { get; set; }
    }
}
