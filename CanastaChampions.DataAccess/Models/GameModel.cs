using System;
using System.Collections.Generic;
using System.Text;

namespace CanastaChampions.DataAccess.Models
{
    public class GameModel
    {
        public long GameID { get; set; }
        public long CompetitionID { get; set; }
        public int CompetitionGameNumber { get; set; }
        public string Location { get; set; }
        public DateTime GameStartDateTime { get; set; }
        public DateTime GameEndDateTime { get; set; }

        public bool LogicallyDeleted { get; set; }
    }
}
