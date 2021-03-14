using CanastaChampions.Data.DataAccess;
using CanastaChampions.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CanastaChampions.Domain.Services
{
    public class CompetitionRepository
    {
        public List<CompetitionModel> Competitions = new List<CompetitionModel>();

        public CompetitionRepository(bool loadAllCompetitions = true)
        {
            if (loadAllCompetitions)
                Competitions = DBMethods.GetAllCompetitions();
        }

        /// <summary>
        /// Create a new Competition, adding it to the in-memory collection.
        /// </summary>
        /// <param name="competitionName"></param>
        /// <param name="fixedTeams">Are the same Players always together?</param>
        /// <param name="randomiseTeams">Are the Players randomly selected each Game?</param>
        /// <returns></returns>
        public CompetitionModel CreateCompetition(string competitionName, bool fixedTeams, bool randomiseTeams)
        {
            if ((fixedTeams) && (randomiseTeams)) throw new ArgumentException("FixedTeams and RandomiseTeams are mutually exclusive; both cannot be true.");

            CompetitionModel competition = new CompetitionModel
            {
                CompetitionName = competitionName,
                FixedTeams = fixedTeams,
                RandomiseTeams = randomiseTeams,
                LogicallyDeleted = false
            };

            if (DoesCompetitionExist(competitionName, out long competitionID) == false)
            {
                DBMethods.InsertCompetition(competition);
                Competitions.Add(competition);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Competition {competitionName} already exists (CompetitionID={competitionID}); not creating.");
                competition.CompetitionID = competitionID;
            }

            return competition;
        }

        public void UpdateCompetition(CompetitionModel competition)
        {
            DBMethods.UpdateCompetition(competition);
        }

        /// <summary>
        /// Check to see if a Competition (based on Name) exists.
        /// </summary>
        /// <param name="competitionName"></param>
        /// <param name="competitionID"></param>
        /// <returns></returns>
        public bool DoesCompetitionExist(string competitionName, out long competitionID)
        {
            bool rValue = false;

            competitionID = DBMethods.GetCompetitionID(competitionName);
            if (competitionID != -1)
            {
                rValue = true;

            }

            return rValue;
        }

       
        /// <summary>
        /// Register a Player to a Competition.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="playerID"></param>
        /// <param name="regular"></param>
        public void RegisterPlayer(long competitionID, long playerID, bool regular = true)
        {
            if (competitionID == -1) throw new ArgumentException("Competition must be added before calling RegisterPlayer.");
            if (playerID == -1) throw new ArgumentException("Player must be added before calling RegisterPlayer.");

            if (DBMethods.IsRegisteredToCompetition(competitionID, playerID, out long registeredPlayerID) == false)
            {
                DBMethods.RegisterPlayerToCompetition(competitionID, playerID, regular);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Player is already registered (RegisteredPlayerID={registeredPlayerID}; not registering.");
            }
        }

        /// <summary>
        /// Insert a Team for a Competition (note: not a Game).
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="teamMember1ID"></param>
        /// <param name="teamMember2ID"></param>
        /// <returns></returns>
        public long InsertTeam(long competitionID, long teamMember1ID, long teamMember2ID)
        {
            if (teamMember1ID == -1)  throw new ArgumentException("Team member #1 is not registered");
            if (teamMember2ID == -1) throw new ArgumentException("Team member #2 is not registered");

            long rValue = -1;

            if (DBMethods.GetTeamID(competitionID, teamMember1ID, teamMember2ID) == -1)
            {
                rValue = DBMethods.InsertTeam(competitionID, teamMember1ID, teamMember2ID);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Team combination of {teamMember1ID} and {teamMember2ID} already exists; not creating.");
            }
            
            return rValue;
        }

        /// <summary>
        /// Return a TeamModel for the specified people and Competition.
        /// </summary>
        /// <param name="competition"></param>
        /// <param name="player1Name"></param>
        /// <param name="player2Name"></param>
        /// <returns></returns>
        public TeamModel GetTeam(long competitionID, string player1Name, string player2Name)
        {
            return DBMethods.GetTeam(competitionID, player1Name, player2Name);
        }

        /// <summary>
        /// Add a Team for all Combinations of non-deleted Players.
        /// </summary>
        /// <param name="competitionID"></param>
        public void AddAllTeamCombinations(long competitionID)
        {
            DBMethods.AddAllTeamCombinations(competitionID);
        }

        /// <summary>
        /// Return a Competition based on the Competition Name.
        /// </summary>
        /// <param name="competitionName"></param>
        /// <returns></returns>
        public CompetitionModel GetCompetition(string competitionName)
        {
            // TODO review
            if (Competitions.Count == 0)
                DBMethods.GetAllCompetitions();

            return Competitions.Where(x => x.CompetitionName == competitionName).FirstOrDefault();
        }

    }
}
