using CanastaChampions.Data.DataAccess;
using CanastaChampions.Data.Models;
using CanastaChampions.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CanastaChampions.Domain.Services
{
    public class GameRepository
    {
        /// <summary>
        /// Add a Game to the Competition.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="location"></param>
        /// <param name="gameStartDateTime"></param>
        /// <param name="teams"></param>
        /// <param name="playersInPositionOrder"></param>
        /// <param name="dealerNumber"></param>
        /// <returns></returns>
        public long CreateGame(long competitionID, string location, DateTime gameStartDateTime, List<TeamModel> teams,
            PlayerModel[] playersInPositionOrder, int dealerNumber)
        {
            // --- Insert a record in the Game table 
            int nextGameNumber = DBMethods.GetNextConsecutiveGameNumber(competitionID);
            long gameID = DBMethods.InsertGame(competitionID, nextGameNumber, location, gameStartDateTime);

            // --- Insert a record in the GameTeam table
            long? team3IDValue = null;
            if (teams.Count == 3)
            {
                long returnValue = DBMethods.GetTeamID(competitionID, teams[2].TeamMember1ID, teams[2].TeamMember2ID);
                if (returnValue == -1)
                    team3IDValue = null;
            }

            DBMethods.InsertGameTeam(competitionID,
                DBMethods.GetTeamID(competitionID, teams[0].TeamMember1ID, teams[0].TeamMember2ID),
                DBMethods.GetTeamID(competitionID, teams[1].TeamMember1ID, teams[1].TeamMember2ID),
                team3IDValue
            );

            // --- Insert x records in GamePlayerPositions
            // TODO Assumption is that playerIDsInPositioning[0] WILL be dealer.
            DBMethods.InsertPlayerPositions(competitionID, gameID, playersInPositionOrder, dealerNumber);


            return gameID;
        }

        public long StartRound(long competitionID, long gameID, DateTime startDateTime)
        {
            // --- Insert a GameRound record
            // Get the dealer
            (PlayerModel currentDealer, PlayerModel nextDealer) = DBMethods.GetDealer(gameID);

            int gameRoundNumber = DBMethods.GetNextConsecutiveRoundNumber(competitionID, gameID);
            long gameRoundID = DBMethods.InsertGameRound(competitionID, gameID, gameRoundNumber, startDateTime, currentDealer.PlayerID);

            return gameRoundID;
        }

        public void EndRound(long competitionID, long gameID, long gameRoundID, DateTime endOfRoundDateTime, long winnerTeamID)
        {
            DBMethods.RecordEndOfRound(gameRoundID, endOfRoundDateTime, winnerTeamID);

            DBMethods.InsertFinishingBonus(competitionID, gameID, gameRoundID, winnerTeamID);

            DBMethods.UpdateDealer(gameID);
        }

        public void InsertScoreTally(long competitionID, long gameID, long gameRoundID, long teamID,
            int naturalCanastaCount, int unnaturalCanastaCount, int redThreeCount, int pointsInHand)
        {
            DBMethods.InsertScoreTally(competitionID, gameID, gameRoundID, teamID,
                naturalCanastaCount, unnaturalCanastaCount, redThreeCount, pointsInHand);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        /// <param name="gameRoundID"></param>
        /// <param name="teamID"></param>
        /// <param name="playerID"></param>
        /// <param name="penaltyCount"></param>
        public void InsertPenalty(long competitionID, long gameID, long gameRoundID, long teamID, 
            long playerID, int penaltyCount = 1)
        {
            DBMethods.InsertScorePenalty(competitionID, gameID, gameRoundID, teamID, playerID, penaltyCount);
        }

        /// <summary>
        /// Record that a Cutting Bonus should be received.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        /// <param name="gameRoundID"></param>
        /// <param name="team"></param>
        /// <param name="playerID"></param>
        public void AddCuttingBonus(long competitionID, long gameID, long gameRoundID, TeamModel team, long playerID)
        {
            // check to ensure playerID is part of team
            if ((playerID != team.TeamMember1ID) && (playerID != team.TeamMember2ID)) 
                throw new ArgumentException($"Specified Player (PlayerID={playerID}) is not a member of" +
                    $" Team (TeamID={team.TeamID}) which contains Players ({team.TeamMember1ID},{team.TeamMember2ID})");

            DBMethods.InsertCuttingBonus(competitionID, gameID, gameRoundID, team.TeamID, playerID);
        }

        
    }
}
