using System;
using System.Collections.Generic;
using System.Text;

namespace CanastaChampions.DataAccess.Models
{
    public class RoundModel
    {
        public long GameRoundID { get; set; } = -1;
        public long CompetitionID { get; set; }
        public long GameID { get; set; }

        /// <summary>
        /// A consecutive number, beginning at 1 for each Round
        /// </summary>
        public int RoundNumber { get; set; }

        public DateTime RoundStartDateTime { get; set; }
        public DateTime RoundEndDateTime { get; set; }

        public PlayerModel Dealer { get; set; }
        public long WinningTeamID { get; set; }
    }
}
