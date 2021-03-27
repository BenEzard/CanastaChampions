using System;
using System.Collections.Generic;
using System.Text;

namespace CanastaChampions.DataAccess.Models
{
    public class RoundResultModel
    {
        public long GameRoundID { get; set; }
        public int RoundNumber { get; set; }
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
        public int Team3Score { get; set; }
        public string WinningTeamName { get; set; }
        public string DealerName { get; set; }
    }
}
