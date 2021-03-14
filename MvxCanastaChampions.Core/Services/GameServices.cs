using CanastaChampions.DataAccess.Models;
using CanastaChampions.DataAccess.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvxCanastaChampions.Core.Services
{
    public class GameServices
    {
        /// <summary>
        /// Add a Game to the Competition.
        /// Also inserts the Teams and Player positions.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="location"></param>
        /// <param name="gameStartDateTime"></param>
        /// <param name="gamePlayers"></param>
        /// <returns></returns>
        public static List<GamePlayerModel> CreateGame(long competitionID, string location, DateTime gameStartDateTime,
            List<GamePlayerModel> gamePlayers)
        {
            // --- Insert a record in the Game table 
            int nextGameNumber = GameDataAccess.GetNextConsecutiveGameNumber(competitionID);
            long gameID = GameDataAccess.InsertGame(competitionID, nextGameNumber, location, gameStartDateTime);

            // Update the GamePlayerModels with the newly allocated GameID
            gamePlayers.ForEach(u => u.GameID = gameID);

            // Split the gamePlayers into Teams
            IEnumerable<GamePlayerModel> team1 = gamePlayers.Where(x => x.TeamNumber == 1);
            IEnumerable<GamePlayerModel> team2 = gamePlayers.Where(x => x.TeamNumber == 2);
            IEnumerable<GamePlayerModel> team3 = gamePlayers.Where(x => x.TeamNumber == 3);

            /// Check to see if Teams need to be saved 
            /// (i.e. the first time that two players have played together in a Competition).
            long teamID = GameDataAccess.GetOrInsertTeamID(competitionID, team1.ElementAt(0).PlayerID, team1.ElementAt(1).PlayerID);
            if ((team1.ElementAt(0).GameTeamID == -1) || (team1.ElementAt(1).GameTeamID == -1))
                team1.ToList().ForEach(u => u.GameTeamID = teamID);

            teamID = GameDataAccess.GetOrInsertTeamID(competitionID, team2.ElementAt(0).PlayerID, team2.ElementAt(1).PlayerID);
            if ((team2.ElementAt(0).GameTeamID == -1) || (team2.ElementAt(1).GameTeamID == -1))
                team2.ToList().ForEach(u => u.GameTeamID = teamID);

            long? team3IDValue = null;
            if (team3.Count() > 0)
            {
                long returnValue = GameDataAccess.GetOrInsertTeamID(competitionID, team3.ElementAt(0).PlayerID, team3.ElementAt(1).PlayerID);
                if (returnValue != -1)
                {
                    team3.ToList().ForEach(u => u.GameTeamID = returnValue);
                    team3IDValue = returnValue;
                }
            }

            // Insert the GameTeam record which describes the Teams on a particular Game.
            GameDataAccess.InsertGameTeam(competitionID, 
                team1.ElementAt(0).GameTeamID, 
                team2.ElementAt(0).GameTeamID,
                team3IDValue);

            // --- Insert records in GamePlayerPositions
            GameDataAccess.InsertPlayerPositions(gamePlayers);

            // Prepare the return value
            List<GamePlayerModel> rValue = new List<GamePlayerModel>(gamePlayers.Count);
            rValue.AddRange(team1);
            rValue.AddRange(team2);
            if (team3.Count() > 0)
                rValue.AddRange(team3);

            return rValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameID"></param>
        /// <returns></returns>
        public static (PlayerModel currentDealer, PlayerModel nextDealer) GetDealer(long gameID)
            => GameDataAccess.GetDealer(gameID);

        /// <summary>
        /// Save two Players as a Team.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="teamMember1ID"></param>
        /// <param name="teamMember2ID"></param>
        public static void SaveTeam(long competitionID, long teamMember1ID, long teamMember2ID)
            => GameDataAccess.InsertTeam(competitionID, teamMember1ID, teamMember2ID);

        public static RoundModel StartRound(RoundModel round)
        {
            // --- Insert a GameRound record
            // Get the dealer
            (PlayerModel currentDealer, PlayerModel nextDealer) = GetDealer(round.GameID);
            round.Dealer = currentDealer;

            // Get the next round number.
            round.RoundNumber = GameDataAccess.GetNextConsecutiveRoundNumber(round.CompetitionID, round.GameID);
            
            // Insert the record into the GameRound table
            long gameRoundID = GameDataAccess.InsertGameRound(round.CompetitionID, round.GameID, round.RoundNumber, 
                round.RoundStartDateTime, round.Dealer.PlayerID);
            round.GameRoundID = gameRoundID;

            return round;
        }

    }
}
