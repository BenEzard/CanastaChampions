using System;
using System.Collections.Generic;
using System.Text;

namespace CanastaChampions.Data.Models
{
    public class GameModel
    {
        public long GameID { get; set; }
        public long CompetitionID { get; set; }
        public string Location { get; set; }
        public DateTime GameStartDateTime { get; set; }
        public DateTime GameEndDateTime { get; set; }

        public bool LogicallyDeleted { get; set; }
    }
}
