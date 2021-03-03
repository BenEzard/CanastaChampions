using CanastaChampions.Data.DataAccess;
using CanastaChampions.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace CanastaChampions.Domain.Services
{
    public class PlayerRepository
    {
        public List<PlayerModel> Players = new List<PlayerModel>();

        public PlayerRepository(bool loadAllCompetitions = true)
        {
            if (loadAllCompetitions)
                Players = DBMethods.GetAllPlayers();

        }

        /// <summary>
        /// Create a Player record.
        /// </summary>
        /// <param name="playerName"></param>
        /// <returns></returns>
        public PlayerModel AddPlayer(string playerName)
        {
            PlayerModel player = new PlayerModel
            {
                PlayerName = playerName
            };

            if (DoesPlayerExist(playerName, out long playerID) == false)
            {
                DBMethods.InsertPlayer(player);
                Players.Add(player);
            } 
            else
            {
                System.Console.WriteLine($"Player {playerName} already exists (PlayerID={playerID}); not creating.");
            }

            return player;
        }

        /// <summary>
        /// Check to see if a Player exists based on their name.
        /// </summary>
        /// <param name="playerName"></param>
        /// <returns></returns>
        public bool DoesPlayerExist(string playerName, out long playerID)
        {
            bool rValue = false;

            playerID = DBMethods.GetPlayerID(playerName);
            if (playerID != -1)
                rValue = true;

            return rValue;
        }

        /// <summary>
        /// Return a Player based on their name.
        /// </summary>
        /// <param name="playerName"></param>
        /// <returns></returns>
        public PlayerModel GetPlayerByName(string playerName)
        {
            //return DBMethods.GetPlayerByName(playerName);
            // TODO review
            if (Players.Count == 0)
                DBMethods.GetAllPlayers();

            return Players.Where(x => x.PlayerName == playerName).FirstOrDefault();

        }
    }
}
