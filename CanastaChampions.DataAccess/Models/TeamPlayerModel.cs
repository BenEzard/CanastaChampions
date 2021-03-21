using System;
using System.Collections.Generic;
using System.Text;

namespace CanastaChampions.DataAccess.Models
{
    public class TeamPlayerModel
    {
        public long PlayerID { get; set; }
        public string PlayerName { get; set; }

        public long CompetitionID { get; set; }
        public long GameID { get; set; }
        public long TeamID { get; set; }
    }
}
