using CanastaChampions.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace CanastaChampions.Data.DataAccess
{
    public class DBMethods
    {
        private static string CONNECTION_STRING = @"DataSource=D:\Development\CanastaChampions\CanastaChampions.db;Version=3";
        private static SQLiteConnection _conn = null;

        /// <summary>
        /// Insert a new Competition into the database.
        /// Once inserted, CompetitionID is populated.
        /// </summary>
        /// <param name="competition"></param>
        public static void InsertCompetition(CompetitionModel competition)
        {
            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "INSERT INTO Competitions (CompetitionName, FixedTeams, RandomiseTeams)" +
                        " VALUES (@competitionName, @fixedTeams, @randomiseTeams)";
                command.Parameters.AddWithValue("@competitionName", competition.CompetitionName);
                command.Parameters.AddWithValue("@fixedTeams", competition.FixedTeams);
                command.Parameters.AddWithValue("@randomiseTeams", competition.RandomiseTeams);
                command.ExecuteNonQuery();
                competition.CompetitionID = _conn.LastInsertRowId;
            }

            _conn.Dispose();
        }

        /// <summary>
        /// Get the next consecutive Game number
        /// </summary>
        /// <param name="competitionID"></param>
        /// <returns></returns>
        public static int GetNextConsecutiveGameNumber(long competitionID)
        {
            int nextGameNumber = 1;

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT *" +
                    " FROM vwGetGameNumber" +
                    " WHERE CompetitionID = @competitionID";
                command.Parameters.AddWithValue("@competitionID", competitionID);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        nextGameNumber = reader.GetInt32(1);
                    }
                }
                _conn.Dispose();

                return nextGameNumber;
            }
        }

        /// <summary>
        /// Get the next consecutive Round number for a specific Competition and Game
        /// </summary>
        /// <param name="competitionID"></param>
        /// <returns></returns>
        public static int GetNextConsecutiveRoundNumber(long competitionID, long gameID)
        {
            int nextRoundNumber = 1;

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT NextRoundNumber" +
                    " FROM vwGetRoundNumber" +
                    " WHERE CompetitionID = @competitionID" +
                    " AND GameID = @gameID";
                command.Parameters.AddWithValue("@competitionID", competitionID);
                command.Parameters.AddWithValue("@gameID", gameID);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        nextRoundNumber = reader.GetInt32(0);
                    }
                }
                _conn.Dispose();

                return nextRoundNumber;
            }
        }

        /// <summary>
        /// Insert a new Game record.
        /// This method automatically calculates the next consecutive Game number within the Competition.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="nextGameNumber"></param>
        /// <param name="location"></param>
        /// <param name="gameStart"></param>
        /// <returns></returns>
        public static long InsertGame(long competitionID, int nextGameNumber, string location, DateTime gameStart)
        {
            long rValue = -1;

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

                using (SQLiteCommand command = _conn.CreateCommand())
                {

                    command.CommandText = "INSERT INTO Game (CompetitionID, CompetitionGameNumber, Location, GameStartDateTime)" +
                    " VALUES (@competitionID, @gameNumber, @location, @gameStart)";
                    command.Parameters.AddWithValue("@competitionID", competitionID);
                    command.Parameters.AddWithValue("@gameNumber", nextGameNumber);
                    command.Parameters.AddWithValue("@location", location);
                    command.Parameters.AddWithValue("@gameStart", gameStart);
                    command.ExecuteNonQuery();
                    rValue = _conn.LastInsertRowId;
                }
                _conn.Dispose();
                return rValue;
        }

        public static long InsertGameTeam(long competitionID, long team1ID, long team2ID, long? team3ID)
        {
            long rValue = -1;

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            string sql = "INSERT INTO GameTeams (CompetitionID, Team1ID, Team2ID, Team3ID)" +
                        " VALUES (@competitionID, @team1ID, @team2ID, @team3ID)";
            if (team3ID.HasValue == false)
            {
                sql = sql.Replace(", Team3ID", "").Replace(", @team3ID", "");
            }

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddWithValue("@competitionID", competitionID);
                command.Parameters.AddWithValue("@team1ID", team1ID);
                command.Parameters.AddWithValue("@team2ID", team2ID);

                if (team3ID.HasValue)
                {
                    command.Parameters.AddWithValue("@team3ID", team3ID.Value);
                }

                command.ExecuteNonQuery();
                rValue = _conn.LastInsertRowId;
            }

            _conn.Dispose();
            return rValue;
        }

        public static long InsertTeam(long competitionID, long teamMember1ID, long teamMember2ID)
        {
            long rValue = -1;

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "INSERT INTO Team (CompetitionID, TeamMember1ID, TeamMember2ID)" +
                        " VALUES (@competitionID, @teamMember1, @teamMember2)";
                command.Parameters.AddWithValue("@competitionID", competitionID);
                command.Parameters.AddWithValue("@teamMember1", teamMember1ID);
                command.Parameters.AddWithValue("@teamMember2", teamMember2ID);
                command.ExecuteNonQuery();
                rValue = _conn.LastInsertRowId;
            }

            _conn.Dispose();
            return rValue;
        }

        public static (PlayerModel currentDealer, PlayerModel nextDealer) GetDealer(long gameID)
        {
            PlayerModel currentDealer = null;
            PlayerModel nextDealer = null;

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT CurrentDealerPlayerID, CurrentDealer_Name, NextDealerPlayerID, NextDealer_Name" +
                    " FROM vwCurrentAndNextDealer" +
                    " WHERE GameID = @gameID";
                command.Parameters.AddWithValue("@gameID", gameID);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        currentDealer = new PlayerModel
                        {
                            PlayerID = reader.GetInt32(0),
                            PlayerName = reader.GetString(1)
                        };

                        nextDealer = new PlayerModel
                        {
                            PlayerID = reader.GetInt32(2),
                            PlayerName = reader.GetString(3)
                        };
                    }
                }
            }

            _conn.Dispose();
            return (currentDealer, nextDealer);
        }

        public static void PurgeDatabase()
        {
            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "DELETE FROM RegisteredPlayers";
                command.ExecuteNonQuery();
                command.CommandText = "DELETE FROM sqlite_sequence WHERE name='RegisteredPlayers'";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM GamePlayerPositions";
                command.ExecuteNonQuery();
                command.CommandText = "DELETE FROM sqlite_sequence WHERE name='GamePlayerPositions'";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM GameTeams";
                command.ExecuteNonQuery();
                command.CommandText = "DELETE FROM sqlite_sequence WHERE name='GameTeams'";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM GameRoundScoreDetails";
                command.ExecuteNonQuery();
                command.CommandText = "DELETE FROM sqlite_sequence WHERE name='GameRoundScoreDetails'";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM GameRound";
                command.ExecuteNonQuery();
                command.CommandText = "DELETE FROM sqlite_sequence WHERE name='GameRound'";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM Game";
                command.ExecuteNonQuery();
                command.CommandText = "DELETE FROM sqlite_sequence WHERE name='Game'";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM Team";
                command.ExecuteNonQuery();
                command.CommandText = "DELETE FROM sqlite_sequence WHERE name='Team'";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM Players";
                command.ExecuteNonQuery();
                command.CommandText = "DELETE FROM sqlite_sequence WHERE name='Players'";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM CompetitionStatistics";
                command.ExecuteNonQuery();
                command.CommandText = "DELETE FROM sqlite_sequence WHERE name='CompetitionStatistics'";
                command.ExecuteNonQuery();

                command.CommandText = "DELETE FROM Competitions";
                command.ExecuteNonQuery();
                command.CommandText = "DELETE FROM sqlite_sequence WHERE name='Competitions'";
                command.ExecuteNonQuery();

            }

            _conn.Dispose();

        }

        /// <summary>
        /// Insert a new GameRound record.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        /// <param name="gameRoundNumber"></param>
        /// <param name="startOfRoundDateTime"></param>
        /// <param name="dealerID"></param>
        /// <returns></returns>
        public static long InsertGameRound(long competitionID, long gameID, int gameRoundNumber, DateTime startOfRoundDateTime, long dealerID)
        {
            long rValue = -1;

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "INSERT INTO GameRound (CompetitionID, GameID, GameRoundNumber, StartOfRoundDateTime, DealerID)" +
                        " VALUES (@competitionID, @gameID, @gameRoundNumber, @startOfRound, @dealerID)";
                command.Parameters.AddWithValue("@competitionID", competitionID);
                command.Parameters.AddWithValue("@gameID", gameID);
                command.Parameters.AddWithValue("@gameRoundNumber", gameRoundNumber);
                command.Parameters.AddWithValue("@startOfRound", startOfRoundDateTime);
                command.Parameters.AddWithValue("@dealerID", dealerID);
                command.ExecuteNonQuery();
                rValue = _conn.LastInsertRowId;
            }

            _conn.Dispose();
            return rValue;
        }

        /// <summary>
        /// Record the End of the Round.
        /// </summary>
        /// <param name="gameRoundID"></param>
        public static void RecordEndOfRound(long gameRoundID, DateTime endOfRoundDateTime, long winningTeamID)
        {
            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            string sql = "UPDATE GameRound" +
                " SET EndOfRoundDateTime = @endOfRound," +
                " WinningTeamID = @winner" + 
                " WHERE GameRoundID = @gameRoundID";

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddWithValue("@endOfRound", endOfRoundDateTime);
                command.Parameters.AddWithValue("@gameRoundID", gameRoundID);
                command.Parameters.AddWithValue("@winner", winningTeamID);
                command.ExecuteNonQuery();
            }

            _conn.Dispose();
        }

        public static void InsertScorePenalty(long competitionID, long gameID, long gameRoundID, long teamID, long playerID, 
            int penaltyCount = 1)
        {
            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "INSERT INTO GameRoundScoreDetails (CompetitionID, GameID, GameRoundID, TeamID, PlayerID, PenaltyCount)" +
                        " VALUES (@competitionID, @gameID, @gameRoundID, @teamID, @playerID, @penaltyCount)";
                command.Parameters.AddWithValue("@competitionID", competitionID);
                command.Parameters.AddWithValue("@gameID", gameID);
                command.Parameters.AddWithValue("@gameRoundID", gameRoundID);
                command.Parameters.AddWithValue("@teamID", teamID);
                command.Parameters.AddWithValue("@playerID", playerID);
                command.Parameters.AddWithValue("@penaltyCount", penaltyCount);
                command.ExecuteNonQuery();
            }

            _conn.Dispose();
        }

        public static void InsertCuttingBonus(long competitionID, long gameID, long gameRoundID, long teamID, long playerID)
        {
            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "INSERT INTO GameRoundScoreDetails (CompetitionID, GameID, GameRoundID, TeamID, PlayerID, CuttingBonusCount)" +
                        " VALUES (@competitionID, @gameID, @gameRoundID, @teamID, @playerID, 1)";
                command.Parameters.AddWithValue("@competitionID", competitionID);
                command.Parameters.AddWithValue("@gameID", gameID);
                command.Parameters.AddWithValue("@gameRoundID", gameRoundID);
                command.Parameters.AddWithValue("@teamID", teamID);
                command.Parameters.AddWithValue("@playerID", playerID);
                command.ExecuteNonQuery();
            }

            _conn.Dispose();
        }

        /// <summary>
        /// Insert a Finishing Bonus.
        /// Applies to a Team, not to a Player.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        /// <param name="gameRoundID"></param>
        /// <param name="teamID"></param>
        public static void InsertFinishingBonus(long competitionID, long gameID, long gameRoundID, long teamID)
        {
            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "INSERT INTO GameRoundScoreDetails (CompetitionID, GameID, GameRoundID, TeamID, FinishingBonus)" +
                        " VALUES (@competitionID, @gameID, @gameRoundID, @teamID, 1)";
                command.Parameters.AddWithValue("@competitionID", competitionID);
                command.Parameters.AddWithValue("@gameID", gameID);
                command.Parameters.AddWithValue("@gameRoundID", gameRoundID);
                command.Parameters.AddWithValue("@teamID", teamID);
                command.ExecuteNonQuery();
            }

            _conn.Dispose();
        }

        public static void InsertScoreTally(long competitionID, long gameID, long gameRoundID, long teamID,
            int naturalCanastaCount, int unnaturalCanastaCount, int redThreeCount, int pointsInHand)
        {
            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "INSERT INTO GameRoundScoreDetails (CompetitionID, GameID, GameRoundID, TeamID," +
                    " NaturalCanastaCount, UnnaturalCanastaCount, RedThreeCount, PointsOnHand)" +
                        " VALUES (@competitionID, @gameID, @gameRoundID, @teamID, @natural, @unnatural, @redThree, @points)";
                command.Parameters.AddWithValue("@competitionID", competitionID);
                command.Parameters.AddWithValue("@gameID", gameID);
                command.Parameters.AddWithValue("@gameRoundID", gameRoundID);
                command.Parameters.AddWithValue("@teamID", teamID);
                command.Parameters.AddWithValue("@natural", naturalCanastaCount);
                command.Parameters.AddWithValue("@unnatural", unnaturalCanastaCount);
                command.Parameters.AddWithValue("@redThree", redThreeCount);
                command.Parameters.AddWithValue("@points", pointsInHand);
                command.ExecuteNonQuery();
            }

            _conn.Dispose();
        }


        public static void UpdateDealer(long gameID)
        {
            (PlayerModel currentDealer, PlayerModel nextDealer) = GetDealer(gameID);

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "UPDATE GamePlayerPositions" +
                    " SET DealerFlag = 0" +
                    " WHERE GameID = @gameID" +
                    " AND PlayerID = @currentDealer";
                command.Parameters.AddWithValue("@gameID", gameID);
                command.Parameters.AddWithValue("@currentDealer", currentDealer.PlayerID);

                command.ExecuteNonQuery();

                command.CommandText = "UPDATE GamePlayerPositions" +
                    " SET DealerFlag = 1" +
                    " WHERE GameID = @gameID" +
                    " AND PlayerID = @nextDealer";
                command.Parameters.AddWithValue("@gameID", gameID);
                command.Parameters.AddWithValue("@nextDealer", nextDealer.PlayerID);

                command.ExecuteNonQuery();

            }

            _conn.Dispose();
        }

        public static void InsertPlayer(PlayerModel player)
        {
            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "INSERT INTO Players (PlayerName)" +
                        " VALUES (@playerName)";
                command.Parameters.AddWithValue("@playerName", player.PlayerName);
                command.ExecuteNonQuery();
                player.PlayerID = _conn.LastInsertRowId;
            }

            _conn.Dispose();
        }

        /// <summary>
        /// Register a Player to a Competition.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="playerID"></param>
        /// <param name="regular"></param>
        public static void RegisterPlayerToCompetition(long competitionID, long playerID, bool regular)
        {
            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "INSERT INTO RegisteredPlayers (CompetitionID, PlayerID, regular)" +
                        " VALUES (@competitionID, @playerID, @regular)";
                command.Parameters.AddWithValue("@competitionID", competitionID);
                command.Parameters.AddWithValue("@playerID", playerID);
                command.Parameters.AddWithValue("@regular", regular);
                command.ExecuteNonQuery();
            }

            _conn.Dispose();
        }

        /// <summary>
        /// Checks to see if a player is registered to a Competition.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="playerID"></param>
        /// <param name="registeredPlayerID"></param>
        /// <returns></returns>
        public static bool IsRegisteredToCompetition(long competitionID, long playerID, out long registeredPlayerID)
        {
            registeredPlayerID = -1;
            bool rValue = false;
            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT RegisteredPlayersID" +
                    " FROM RegisteredPlayers" +
                    " WHERE CompetitionID = @competitionID" +
                    " AND PlayerID = @playerID";
                command.Parameters.AddWithValue("@competitionID", competitionID);
                command.Parameters.AddWithValue("@playerID", playerID);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rValue = true;
                        registeredPlayerID = reader.GetInt64(0);
                    }
                }
            }

            _conn.Dispose();
            return rValue;
        }

        /// <summary>
        /// Checks to see if a Team already exists in a given Competition; if so, returns the TeamID.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="teamMember1ID"></param>
        /// <param name="teamMember2ID"></param>
        /// <returns></returns>
        public static long GetTeamID(long competitionID, long teamMember1ID, long teamMember2ID)
        {
            long rValue = -1;
            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT TeamID" +
                    " FROM vwTeam" +
                    " WHERE CompetitionID = @competitionID" +
                    " AND (TeamMember1ID = @teamMember1 OR TeamMember2ID = @teamMember1)" +
                    " AND (TeamMember1ID = @teamMember2 OR TeamMember2ID = @teamMember2)";
                command.Parameters.AddWithValue("@competitionID", competitionID);
                command.Parameters.AddWithValue("@teamMember1", teamMember1ID);
                command.Parameters.AddWithValue("@teamMember2", teamMember2ID);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        rValue = reader.GetInt64(0);
                }
            }

            _conn.Dispose();
            return rValue;
        }

        public static long GetTeamID(long competitionID, string player1Name, string player2Name)
        {
            long rValue = -1;
            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT TeamID" +
                    " FROM vwTeam" +
                    " WHERE CompetitionID = @competitionID" +
                    " AND (Player1Name = @player1Name OR Player2Name = @player1Name)" +
                    " AND (Player1Name = @player2Name OR Player2Name = @player2Name)";
                command.Parameters.AddWithValue("@competitionID", competitionID);
                command.Parameters.AddWithValue("@player1Name", player1Name);
                command.Parameters.AddWithValue("@player2Name", player2Name);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        rValue = reader.GetInt64(0);
                }
            }

            _conn.Dispose();
            return rValue;
        }

        /// <summary>
        /// Based on the twoPplayer's names and the Competition, return the TeamModel.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="player1Name"></param>
        /// <param name="player2Name"></param>
        /// <returns></returns>
        public static TeamModel GetTeam(long competitionID, string player1Name, string player2Name)
        {
            TeamModel rValue = new TeamModel();

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT DISTINCT" +
                    " TeamID," +
                    " CompetitionID," +
                    " TeamMember1ID," +
                    " TeamMember2ID" +
                    " FROM vwTeam" +
                    " WHERE CompetitionID = @competitionID" +
                    " AND (Player1Name = @player1Name OR Player2Name = @player1Name)" +
                    " AND (Player1Name = @player2Name OR Player2Name = @player2Name)";
                command.Parameters.AddWithValue("@competitionID", competitionID);
                command.Parameters.AddWithValue("@player1Name", player1Name);
                command.Parameters.AddWithValue("@player2Name", player2Name);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rValue.TeamID = reader.GetInt64(0);
                        rValue.CompetitionID = reader.GetInt64(1);
                        rValue.TeamMember1ID = reader.GetInt64(2);
                        rValue.TeamMember2ID = reader.GetInt64(3);
                    }
                }
            }

            _conn.Dispose();
            return rValue;
        }

        /// <summary>
        /// Get a PlayerID based on their name, if not found returns -1
        /// </summary>
        /// <param name="playerName"></param>
        /// <returns></returns>
        public static long GetPlayerID(string playerName)
        {
            long rValue = -1;

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT PlayerID FROM Players WHERE PlayerName = @playerName";
                command.Parameters.AddWithValue("@playerName", playerName);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    rValue = reader.GetInt64(0);
                }
            }

            _conn.Dispose();

            return rValue;
        }

        public static void UpdateCompetition(CompetitionModel competition)
        {
            if (competition.CompetitionID == -1) throw new ArgumentException("Cannot call Update method while Competition does not exist in the database.");

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "UPDATE Competitions" +
                    " SET CompetitionName = @competitionName, " +
                    " FixedTeams = @fixedTeams, " +
                    " RandomiseTeams = @randomiseTeams, " +
                    " LogicallyDeleted = @logicallyDeleted " +
                    " WHERE CompetitionID = @competitionID";
                command.Parameters.AddWithValue("@competitionName", competition.CompetitionName);
                command.Parameters.AddWithValue("@fixedTeams", competition.FixedTeams);
                command.Parameters.AddWithValue("@randomiseTeams", competition.RandomiseTeams);
                command.Parameters.AddWithValue("@logicallyDeleted", competition.LogicallyDeleted);
                command.Parameters.AddWithValue("@competitionID", competition.CompetitionID);
                command.ExecuteNonQuery();
            }

            _conn.Dispose();
        }

        public static PlayerModel GetPlayerByName(string playerName)
        {
            PlayerModel rValue = null;

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT PlayerID, PlayerName FROM Players WHERE PlayerName = @playerName";
                command.Parameters.AddWithValue("@playerName", playerName);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    rValue = new PlayerModel();
                    rValue.PlayerID = reader.GetInt64(0);
                    rValue.PlayerName = reader.GetString(1);
                }
            }

            _conn.Dispose();

            return rValue;
        }

        public static CompetitionModel GetCompetitionByName(string competitionName)
        {
            CompetitionModel rValue = null;

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT CompetitionID, CompetitionName, FixedTeams, RandomiseTeams, LogicallyDeleted" +
                    " FROM Competitions" +
                    " WHERE CompetitionName = @competitionName";
                command.Parameters.AddWithValue("@competitionName", competitionName);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    rValue = new CompetitionModel();
                    rValue.CompetitionID = reader.GetInt64(0);
                    rValue.CompetitionName = reader.GetString(1);
                    rValue.FixedTeams = reader.GetBoolean(2);
                    rValue.RandomiseTeams = reader.GetBoolean(3);
                    rValue.LogicallyDeleted = reader.GetBoolean(4);
                }
            }

            _conn.Dispose();

            return rValue;
        }

        public static List<CompetitionModel> GetAllCompetitions(bool excludeDeleted = false)
        {
            List<CompetitionModel> rValue = new List<CompetitionModel>();

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                string baseSQL = "SELECT CompetitionID, CompetitionName, FixedTeams, RandomiseTeams, LogicallyDeleted" +
                    " FROM Competitions";

                if (excludeDeleted)
                {
                    baseSQL += " WHERE LogicallyDeleted = @logicallyDeleted";
                    command.CommandText = baseSQL;
                    command.Parameters.AddWithValue("@logicallyDeleted", true);
                }
                else
                {
                    command.CommandText = baseSQL;
                }

                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CompetitionModel c = new CompetitionModel();
                    c.CompetitionID = reader.GetInt64(0);
                    c.CompetitionName = reader.GetString(1);
                    c.FixedTeams = reader.GetBoolean(2);
                    c.RandomiseTeams = reader.GetBoolean(3);
                    c.LogicallyDeleted = reader.GetBoolean(4);
                    rValue.Add(c);
                }
            }

            _conn.Dispose();

            return rValue;
        }

        public static List<PlayerModel> GetAllPlayers(bool excludeDeleted = false)
        {
            var rValue = new List<PlayerModel>();

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                string baseSQL = "SELECT PlayerID, PlayerName, LogicallyDeleted" +
                    " FROM Players";

                if (excludeDeleted)
                {
                    baseSQL += " WHERE LogicallyDeleted = @logicallyDeleted";
                    command.CommandText = baseSQL;
                    command.Parameters.AddWithValue("@logicallyDeleted", true);
                }
                else
                {
                    command.CommandText = baseSQL;
                }

                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    PlayerModel p = new PlayerModel();
                    p.PlayerID = reader.GetInt64(0);
                    p.PlayerName = reader.GetString(1);
                    p.LogicallyDeleted = reader.GetBoolean(2);
                    rValue.Add(p);
                }
            }

            _conn.Dispose();

            return rValue;
        }

        /// <summary>
        /// Given a CompetitionName, returns the CompetitionID (if it exists in the database), or -1 if it doesn't.
        /// </summary>
        /// <param name="competitionName"></param>
        /// <returns></returns>
        public static long GetCompetitionID(string competitionName)
        {
            long competitionID = -1;

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT CompetitionID FROM Competitions WHERE CompetitionName = @competitionName";
                command.Parameters.AddWithValue("@competitionName", competitionName);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        competitionID = reader.GetInt64(0);
                }
            }

            _conn.Dispose();

            return competitionID;
        }

        /// <summary>
        /// Insert the position of Players in a Game.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        /// <param name="playersInPosition"></param>
        /// <param name="dealerNumber"></param>
        public static void InsertPlayerPositions(long competitionID, long gameID, PlayerModel[] playersInPosition, int dealerNumber)
        {
            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                for (int i = 1; i <= playersInPosition.Length; i++)
                {
                    command.CommandText = "INSERT INTO GamePlayerPositions (CompetitionID, GameID, PositionNumber, PlayerID, DealerFlag)" +
                            " VALUES (@competitionID, @gameID, @positionNumber, @playerID, @dealerFlag)";
                    command.Parameters.AddWithValue("@competitionID", competitionID);
                    command.Parameters.AddWithValue("@gameID", gameID);
                    command.Parameters.AddWithValue("@positionNumber", i);
                    command.Parameters.AddWithValue("@playerID", playersInPosition[i - 1].PlayerID);
                    command.Parameters.AddWithValue("@dealerFlag", i == dealerNumber ? 1 : 0);
                    command.ExecuteNonQuery();
                }
                _conn.Dispose();

            }
        }

        public static void AddAllTeamCombinations(long competitionID)
        {
            List<TeamModel> teamCombinations = new List<TeamModel>();

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            // Get all of the combinations
            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT *" +
                    " FROM vwAllPlayerCombinations" +
                    " WHERE CompetitionID = @competitionID";
                command.Parameters.AddWithValue("@competitionID", competitionID);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TeamModel team = new TeamModel
                        {
                            CompetitionID = competitionID,
                            TeamMember1ID = reader.GetInt64(1),
                            TeamMember2ID = reader.GetInt64(2)
                        };
                        teamCombinations.Add(team);
                    }
                }
            }
            _conn.Dispose();

            foreach (TeamModel t in teamCombinations)
            {
                if (DBMethods.GetTeamID(competitionID, t.TeamMember1ID, t.TeamMember2ID) == -1)
                    InsertTeam(t.CompetitionID, t.TeamMember1ID, t.TeamMember2ID);
            }
                

            teamCombinations.Clear();
        }
    }
}
