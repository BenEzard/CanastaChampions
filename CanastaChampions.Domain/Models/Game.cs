using CanastaChampions.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CanastaChampions.Domain.Models
{
    public class Game
    {
        public long CompetitionID { get; set; }
        public long GameID { get; set; }
        public string Location { get; set; }
        public DateTime GameStartDateTime { get; set; }
        public List<TeamModel> Teams { get; set; }
        public PlayerModel[] PlayerOrder { get; set; }

        public Game(long competitionID, string location, DateTime gameStartDateTime, List<TeamModel> teams,
            PlayerModel[] playersInPositionOrder)
        {
            CompetitionID = competitionID;
            Location = location;
            GameStartDateTime = gameStartDateTime;
            Teams = teams;
            PlayerOrder = playersInPositionOrder;

        }
    }
}
