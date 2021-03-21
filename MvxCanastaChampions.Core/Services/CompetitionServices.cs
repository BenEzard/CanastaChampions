using System;
using System.Collections.Generic;
using CanastaChampions.DataAccess.Models;

namespace MvxCanastaChampions.Core.Services
{
    public class CompetitionServices
    {
        /// <summary>
        /// Get all Competitions from the database.
        /// </summary>
        /// <param name="excludeDeleted"></param>
        /// <returns></returns>
        public static List<CompetitionModel> GetAllCompetitions(bool excludeDeleted = false)
            => CompetitionDataAccess.GetAllCompetitions(excludeDeleted);

        /// <summary>
        /// Check to see if a Competition (based on Name) exists.
        /// </summary>
        /// <param name="competitionName"></param>
        /// <param name="competitionID"></param>
        /// <returns></returns>
        public static bool DoesCompetitionExist(string competitionName, out long competitionID)
        {
            bool rValue = false;

            competitionID = GetCompetitionID(competitionName);
            if (competitionID != -1)
            {
                rValue = true;

            }

            return rValue;
        }

        /// <summary>
        /// Given a CompetitionName, returns the CompetitionID (if it exists in the database), or -1 if it doesn't.
        /// </summary>
        /// <param name="competitionName"></param>
        /// <returns></returns>
        private static long GetCompetitionID(string competitionName)
            => CompetitionDataAccess.GetCompetitionID(competitionName);


        /// <summary>
        /// Create a new Competition.
        /// Before insertion, checks to see if it exists.
        /// </summary>
        /// <param name="competition"></param>
        /// <returns>Returns the CompetitionModel (with CompetitionID populated) or null if not created.</returns>
        public static CompetitionModel GetOrCreateCompetition(CompetitionModel competition)
        {
            CompetitionModel rValue = CompetitionDataAccess.GetCompetitionByName(competition.CompetitionName);

            if (rValue == null)
                rValue = CompetitionDataAccess.InsertCompetition(competition);

            return rValue;
        }

        /// <summary>
        /// Get a list of registered Players.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <returns></returns>
        public static List<PlayerModel> GetRegisteredPlayers(long competitionID)
            => CompetitionDataAccess.GetRegisteredPlayers(competitionID);

        /// <summary>
        /// Get a list of the players for a specific Competition.
        /// </summary>
        /// <param name="competition"></param>
        /// <returns></returns>
        public static List<GamePlayerModel> GetPlayerRoster(CompetitionModel competition)
        {
            List<GamePlayerModel> returnList = null;

            if (competition.TeamsAreFixed)
            {
                // If teams are fixed, then look for a regular team first.
                returnList = CompetitionDataAccess.GetRegularTeams(competition.CompetitionID);
            }
            
            // If a regular team is not found, then get all the players.
            if ((competition.TeamsAreRandomised) || (returnList == null))
            {
                List<PlayerModel> players = CompetitionDataAccess.GetRegisteredPlayers(competition.CompetitionID);
                returnList = new List<GamePlayerModel>(players.Count);
                foreach (PlayerModel p in players)
                {
                    GamePlayerModel tfm = new GamePlayerModel
                    {
                        CompetitionID = competition.CompetitionID,
                        PlayerID = p.PlayerID,
                        PlayerName = p.PlayerName
                    };
                    returnList.Add(tfm);
                }
            }

            return returnList;
        }

        /// <summary>
        /// Create a Player
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static PlayerModel GetOrCreatePlayer(PlayerModel player)
        {
            PlayerModel rValue = CompetitionDataAccess.GetPlayer(player.PlayerName);

            if (rValue == null)
                rValue = CompetitionDataAccess.InsertPlayer(player);

            return rValue;
        }

        public static void RegisterPlayer(long competitionID, long playerID, bool regular = true)
        {

            CompetitionDataAccess.InsertPlayerToCompetition(competitionID, playerID, regular);
        }

        public static void RegisterTeam(long competitionID, long player1ID, long player2ID)
            => CompetitionDataAccess.InsertTeam(competitionID, player1ID, player2ID);




    }
}
