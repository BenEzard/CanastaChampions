using System;
using System.Collections.Generic;
using System.Text;

namespace CanastaChampions.DataAccess.Models
{
    public class TeamFormationModel
    {
        public long PlayerID { get; set; }
        public string PlayerName { get; set; }
        public int TeamNumber { get; set; }
    }
}
