using CanastaChampions.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace CanastaChampions.DataAccess.Services
{
    public class GameDataAccess : BaseDataAccess
    {
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

        public static int GetRoundNumber(long competitionID, long gameID)
        {
            int thisRoundNumber = 1;

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT ThisRoundNumber" +
                    " FROM vwGetRoundNumber" +
                    " WHERE CompetitionID = @competitionID" +
                    " AND GameID = @gameID";
                command.Parameters.AddWithValue("@competitionID", competitionID);
                command.Parameters.AddWithValue("@gameID", gameID);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        thisRoundNumber = reader.GetInt32(0);
                    }
                }
                _conn.Dispose();

                return thisRoundNumber;
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
        /// Insert the position of Players in a Game.
        /// </summary>
        /// <param name="playersInPosition"></param>
        public static void InsertPlayerPositions(List<GamePlayerModel> playersInPosition)
        {
            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                int i = 1;
                foreach (GamePlayerModel gp in playersInPosition)
                {
                    command.CommandText = "INSERT INTO GamePlayerPositions (CompetitionID, GameID, PositionNumber, PlayerID, DealerFlag)" +
                            " VALUES (@competitionID, @gameID, @positionNumber, @playerID, @dealerFlag)";
                    command.Parameters.AddWithValue("@competitionID", gp.CompetitionID);
                    command.Parameters.AddWithValue("@gameID", gp.GameID);
                    command.Parameters.AddWithValue("@positionNumber", i);
                    command.Parameters.AddWithValue("@playerID", gp.PlayerID);
                    command.Parameters.AddWithValue("@dealerFlag", i == 1 ? 1 : 0);
                    command.ExecuteNonQuery();
                    ++i;
                }
                _conn.Dispose();

            }
        }

        /// <summary>
        /// Checks to see if a Team already exists in a given Competition; if so, returns the TeamID.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="teamMember1ID"></param>
        /// <param name="teamMember2ID"></param>
        /// <returns></returns>
        public static long GetOrInsertTeamID(long competitionID, long teamMember1ID, long teamMember2ID)
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

            if (rValue == -1)
            {
                rValue = InsertTeam(competitionID, teamMember1ID, teamMember2ID);
            }

            _conn.Dispose();
            return rValue;
        }

        /// <summary>
        /// Insert a Team record.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="teamMember1ID"></param>
        /// <param name="teamMember2ID"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Insert a GameTeam record, which describes which Teams play.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="team1ID"></param>
        /// <param name="team2ID"></param>
        /// <param name="team3ID"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Return the current and next dealer in the game.
        /// </summary>
        /// <param name="gameID"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        /// <param name="gameRoundID"></param>
        /// <param name="teamID"></param>
        /// <param name="playerID"></param>
        /// <param name="penaltyCount"></param>
        public static void InsertScorePenalty(long competitionID, long gameID, long gameRoundID, long teamID, 
            long playerID, int penaltyCount = 1)
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

        public static int GetTeamPenaltyCount(long competitionID, long gameID, long roundID, long teamID)
        {
            int rValue = 0;
            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT SUM(PenaltyCount) AS PenaltyCount" +
                    " FROM GameRoundScoreDetails" +
                    " WHERE CompetitionID = @competitionID" +
                    " AND GameID = @gameID" +
                    " AND GameRoundID = @gameRoundID" +
                    " AND TeamID = @teamID";
                command.Parameters.AddWithValue("@competitionID", competitionID);
                command.Parameters.AddWithValue("@gameID", gameID);
                command.Parameters.AddWithValue("@gameRoundID", roundID);
                command.Parameters.AddWithValue("@teamID", teamID);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (reader.IsDBNull(0) == false)
                                rValue = reader.GetInt32(0);
                        }
                    }
                }
            }

            _conn.Dispose();
            return rValue;
        }

        /// <summary>
        /// Insert a Round score tally for a Team.
        /// Note: this doesn't include penalties, cutting bonus or finishing bonus which are added via their own methods.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        /// <param name="gameRoundID"></param>
        /// <param name="teamID"></param>
        /// <param name="naturalCanastaCount"></param>
        /// <param name="unnaturalCanastaCount"></param>
        /// <param name="redThreeCount"></param>
        /// <param name="pointsInHand"></param>
        public static void InsertRoundScoreTally(long competitionID, long gameID, long gameRoundID, long teamID,
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

        /// <summary>
        /// Insert a Round's cutting bonus to a specific Player.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        /// <param name="gameRoundID"></param>
        /// <param name="teamID"></param>
        /// <param name="playerID"></param>
        public static void InsertRoundCuttingBonus(long competitionID, long gameID, long gameRoundID, long teamID, long playerID)
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
    }
}
