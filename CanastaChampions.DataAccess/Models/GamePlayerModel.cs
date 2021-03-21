using System;
using System.Collections.Generic;
using System.Text;

namespace CanastaChampions.DataAccess.Models
{
    public class GamePlayerModel
    {
        public long CompetitionID { get; set; }

        public long GameID { get; set; }

         public long TeamID { get; set; } = -1;

        public long PlayerID { get; set; }

        public string PlayerName { get; set; }

        /// <summary>
        /// A team number that is consecutive per game. i.e. Team 1, 2, 3.
        /// </summary>
        public int TeamNumber { get; set; }

        public GamePlayerModel()
        {

        }

        public GamePlayerModel(long playerID, string playerName)
        {
            PlayerID = playerID;
            PlayerName = playerName;
        }

        public GamePlayerModel(long competitionID, int teamNumber)
        {
            CompetitionID = competitionID;
            TeamNumber = teamNumber;
        }

        public GamePlayerModel(long playerID, string playerName, int teamNumber)
        {
            PlayerID = playerID;
            PlayerName = playerName;
            TeamNumber = teamNumber;
        }
    }
}
