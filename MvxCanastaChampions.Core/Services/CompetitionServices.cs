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
        /// Create a new Competition, adding it to the in-memory collection.
        /// </summary>
        /// <param name="competitionModel"></param>
        /// <returns></returns>
        public static CompetitionModel CreateCompetition(CompetitionModel competitionModel)
            => CompetitionDataAccess.CreateCompetition(competitionModel);

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
        /// Insert a new Competition into the database.
        /// Once inserted, CompetitionID is populated.
        /// </summary>
        /// <param name="competition"></param>
        private static void InsertCompetition(CompetitionModel competition)
            => CompetitionDataAccess.InsertCompetition(competition);

        /// <summary>
        /// Get a list of registered Players.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <returns></returns>
        public static List<PlayerModel> GetRegisteredPlayers(long competitionID)
            => CompetitionDataAccess.GetRegisteredPlayers(competitionID);

        public static List<TeamFormationModel> GetPlayerRoster(long competitionID)
        {
            List<PlayerModel> players = CompetitionDataAccess.GetRegisteredPlayers(competitionID);

            List<TeamFormationModel> rValue = new List<TeamFormationModel>(players.Count);

            foreach (PlayerModel p in players)
            {
                TeamFormationModel tfm = new TeamFormationModel
                {
                    PlayerID = p.PlayerID,
                    PlayerName = p.PlayerName
                };
                rValue.Add(tfm);
            }

            return rValue;
        }
    }
}
