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

        public TeamFormationModel()
        {

        }

        public TeamFormationModel(long playerID, string playerName)
        {
            PlayerID = playerID;
            PlayerName = playerName;
        }        
        
        public TeamFormationModel(long playerID, string playerName, int teamNumber)
        {
            PlayerID = playerID;
            PlayerName = playerName;
            TeamNumber = teamNumber;
        }
    }
}
