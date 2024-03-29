﻿using CanastaChampions.DataAccess.Models;
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
            if ((team1.ElementAt(0).TeamID == -1) || (team1.ElementAt(1).TeamID == -1))
                team1.ToList().ForEach(u => u.TeamID = teamID);

            teamID = GameDataAccess.GetOrInsertTeamID(competitionID, team2.ElementAt(0).PlayerID, team2.ElementAt(1).PlayerID);
            if ((team2.ElementAt(0).TeamID == -1) || (team2.ElementAt(1).TeamID == -1))
                team2.ToList().ForEach(u => u.TeamID = teamID);

            long? team3IDValue = null;
            if (team3.Count() > 0)
            {
                long returnValue = GameDataAccess.GetOrInsertTeamID(competitionID, team3.ElementAt(0).PlayerID, team3.ElementAt(1).PlayerID);
                if (returnValue != -1)
                {
                    team3.ToList().ForEach(u => u.TeamID = returnValue);
                    team3IDValue = returnValue;
                }
            }

            // Insert the GameTeam record which describes the Teams on a particular Game.
            GameDataAccess.InsertGameTeam(competitionID, gameID, 
                team1.ElementAt(0).TeamID, 
                team2.ElementAt(0).TeamID,
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
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        /// <returns></returns>
        public static List<GamePlayerModel> GetPlayers(long competitionID, long gameID)
        {
            List<GamePlayerModel> players = new List<GamePlayerModel>();



            return players;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        public static void AddPlayerPenalty(GamePlayerModel player, long roundID)
        {
            System.Diagnostics.Debug.WriteLine($"==> Penalty for {player.PlayerName}, Player ID # {player.PlayerID} in game ID {player.GameID}, gameTeamID {player.TeamID}, team # {player.TeamNumber}");
            GameDataAccess.InsertScorePenalty(player.CompetitionID, player.GameID, roundID, player.TeamID, player.PlayerID);
        }

        public static int GetTeamPenaltyCount(long competitionID, long gameID, long roundID, long teamID)
            => GameDataAccess.GetTeamPenaltyCount(competitionID, gameID, roundID, teamID);

        /// <summary>
        /// Check for an unfinished Game in the specified Competition.
        /// If there is one, it will return a gameID, and either a RoundID (specifying a Round was underway) or 0, 
        /// if no Round was underway.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        /// <param name="gameRoundID"></param>
        /// <returns></returns>
        public static bool CheckForUnfinishedGame(long competitionID, out long gameID, out long gameRoundID)
        {
            bool unfinishedGame = false;
                
            (gameID, gameRoundID) = GameDataAccess.GetIncompleteGameAndRoundDetails(competitionID);
            if (gameID > -1)
                unfinishedGame = true;

            return unfinishedGame;
        }

        public static int GetRoundNumber(long competitionID, long gameID)
            => GameDataAccess.GetRoundNumber(competitionID, gameID);

        /// <summary>
        /// Get the current and next dealer.
        /// </summary>
        /// <param name="gameID"></param>
        /// <returns></returns>
        public static (GamePlayerModel currentDealer, GamePlayerModel nextDealer) GetDealer(long gameID)
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
            (GamePlayerModel currentDealer, GamePlayerModel nextDealer) = GetDealer(round.GameID);
            round.Dealer = currentDealer;

            // Get the next round number.
            round.RoundNumber = GameDataAccess.GetNextConsecutiveRoundNumber(round.CompetitionID, round.GameID);
            
            // Insert the record into the GameRound table
            long gameRoundID = GameDataAccess.InsertGameRound(round.CompetitionID, round.GameID, round.RoundNumber, 
                round.RoundStartDateTime, round.Dealer.PlayerID);
            round.GameRoundID = gameRoundID;

            return round;
        }

        /// <summary>
        /// Load a specified RoundModel.
        /// </summary>
        /// <param name="gameRoundID"></param>
        /// <returns></returns>
        public static RoundModel LoadRound(long gameRoundID)
            => GameDataAccess.GetRound(gameRoundID);

        /// <summary>
        /// Get the results of the game by Team.
        /// NOTE: Not sure if this is useful as-is???
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        /// <returns></returns>
        public static List<RoundResultModel> GetResultsByTeam(long competitionID, long gameID)
            => GameDataAccess.GetResultsByTeam(competitionID, gameID);

        /// <summary>
        /// Get the results of the game.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        /// <returns></returns>
        public static List<RoundResultModel> GetResults(long competitionID, long gameID)
            => GameDataAccess.GetResults(competitionID, gameID);

        /// <summary>
        /// Load a Game.
        /// </summary>
        /// <param name="gameID"></param>
        /// <param name="competitionID"></param>
        /// <returns></returns>
        public static GameModel LoadGame(long competitionID, long gameID)
            => GameDataAccess.GetGame(competitionID, gameID);

        /// <summary>
        /// Add a Team's Round score.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        /// <param name="gameRoundID"></param>
        /// <param name="teamID"></param>
        /// <param name="naturalCanastaCount"></param>
        /// <param name="unnaturalCanastaCount"></param>
        /// <param name="redThreeCount"></param>
        /// <param name="pointsInHand"></param>
        public static void AddTeamRoundScore(long competitionID, long gameID, long gameRoundID, long teamID,
            int naturalCanastaCount, int unnaturalCanastaCount, int redThreeCount, int pointsInHand)
            => GameDataAccess.InsertRoundScoreTally(competitionID, gameID, gameRoundID, teamID,
                naturalCanastaCount, unnaturalCanastaCount, redThreeCount, pointsInHand);

        /// <summary>
        /// Add a Round Cutting Bonus to a particular Player.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        /// <param name="gameRoundID"></param>
        /// <param name="teamID"></param>
        /// <param name="playerID"></param>
        public static void AddRoundCuttingBonus(long competitionID, long gameID, long gameRoundID, long teamID, long playerID)
            => GameDataAccess.InsertRoundCuttingBonus(competitionID, gameID, gameRoundID, teamID, playerID);

        /// <summary>
        /// Add a Finishing Bonus for a particular Team.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        /// <param name="gameRoundID"></param>
        /// <param name="teamID"></param>
        public static void AddFinishingBonus(long competitionID, long gameID, long gameRoundID, long teamID)
            => GameDataAccess.InsertFinishingBonus(competitionID, gameID, gameRoundID, teamID);

        /// <summary>
        /// Get a list of the Teams playing a Game.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        /// <returns></returns>
        public static List<TeamModel> GetTeams(long competitionID, long gameID)
            => GameDataAccess.GetTeams(competitionID, gameID);

        /// <summary>
        /// Finalises the Game Round.
        /// Inserts the end of round time and who won.
        /// Also moves on the dealer token.
        /// </summary>
        /// <param name="gameRound"></param>
        public static void FinaliseRound(RoundModel gameRound)
        {
            GameDataAccess.UpdateRoundInformation(gameRound.GameRoundID, gameRound.RoundEndDateTime, gameRound.WinningTeamID);
            GameDataAccess.UpdateDealer(gameRound.CompetitionID, gameRound.GameID);
        }

        /// <summary>
        /// Forceably close any open Games in the Competition.
        /// </summary>
        /// <param name="competitionID"></param>
        public static void ForceablyCloseAnyOpenGames(long competitionID)
            => GameDataAccess.ForceEndAnyOpenGames(competitionID);

        /// <summary>
        /// Finalise a Game.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        /// <param name="endOfGameDateTime"></param>
        public static void FinaliseGame(long competitionID, long gameID, DateTime endOfGameDateTime)
        {
            GameDataAccess.UpdateGameInformation(competitionID, gameID, endOfGameDateTime);
            GameDataAccess.ClearDealerInfo(competitionID, gameID);
        }

        public static List<GamePlayerModel> GetCurrentPlayers(long competitionID, long gameID)
            => GameDataAccess.GetPlayers(competitionID, gameID);

        /// <summary>
        /// Delete all records associated with a specified Game.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        public static void DeleteGame(long competitionID, long gameID)
        {
            GameDataAccess.DeleteGame(competitionID, gameID);

            // TODO recalculate statistics in CompetitionStatistics

        }

        
    }
}
