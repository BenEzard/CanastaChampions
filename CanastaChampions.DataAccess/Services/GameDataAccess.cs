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

        /// <summary>
        /// Get the RoundModel.
        /// The Dealer will only be populated by the PlayerID
        /// </summary>
        /// <param name="gameRoundID"></param>
        /// <returns></returns>
        public static RoundModel GetRound(long gameRoundID)
        {
            RoundModel round = new RoundModel();

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT GameRoundID, CompetitionID, GameID, GameRoundNumber, StartOfRoundDateTime," +
                    " EndOfRoundDateTime, DealerID, WinningTeamID" +
                    " FROM GameRound" +
                    " WHERE GameRoundID = @gameRoundID";
                command.Parameters.AddWithValue("@gameRoundID", gameRoundID);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        round.GameRoundID = reader.GetInt64(0);
                        round.CompetitionID = reader.GetInt64(1);
                        round.GameID = reader.GetInt64(2);
                        round.RoundNumber = reader.GetInt32(3);
                        round.RoundStartDateTime = reader.GetDateTime(4);

                        if (reader.IsDBNull(5) == false)
                            round.RoundEndDateTime = reader.GetDateTime(5);

                        round.Dealer = new GamePlayerModel(reader.GetInt64(6));

                        if (reader.IsDBNull(7) == false)
                            round.WinningTeamID = reader.GetInt64(7);
                    }
                }
                _conn.Dispose();

                return round;
            }
        }

        [Obsolete("Not sure if this is needed? Maybe for stats...")]
        public static List<RoundResultModel> GetResultsByTeam(long competitionID, long gameID)
        {
            List<RoundResultModel> results = new List<RoundResultModel>();

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT GameRoundID, GameRoundNumber, TeamID, TeamName, TotalPoints, WinningTeam, DealerName" +
                    " FROM vwPointsPerRound" +
                    " WHERE CompetitionID = @competitionID" +
                    " AND GameID = @gameID";
                command.Parameters.AddWithValue("@competitionID", competitionID);
                command.Parameters.AddWithValue("@gameID", gameID);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RoundResultModel roundResult = new RoundResultModel();
                        roundResult.GameRoundID = reader.GetInt64(0);
                        roundResult.RoundNumber = reader.GetInt32(1);
                        roundResult.Team1Score = reader.GetInt32(2);
                        roundResult.Team2Score = reader.GetInt32(3);
                        roundResult.Team3Score = reader.GetInt32(4);
                        roundResult.WinningTeamName = reader.GetString(5);
                        roundResult.DealerName = reader.GetString(6);
                        results.Add(roundResult);
                    }
                }
            }
            _conn.Dispose();

            return results;
        }

        public static List<RoundResultModel> GetResults(long competitionID, long gameID)
        {
            List<RoundResultModel> results = new List<RoundResultModel>();

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT GameRoundID, GameRoundNumber, Team1Score, Team2Score, Team3Score, WinningTeam, DealerName" +
                    " FROM vwRoundScoreSummary" +
                    " WHERE CompetitionID = @competitionID" +
                    " AND GameID = @gameID" +
                    " ORDER BY GameRoundNumber";
                command.Parameters.AddWithValue("@competitionID", competitionID);
                command.Parameters.AddWithValue("@gameID", gameID);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RoundResultModel roundResult = new RoundResultModel();
                        roundResult.GameRoundID = reader.GetInt64(0);
                        roundResult.RoundNumber = reader.GetInt32(1);
                        roundResult.Team1Score = reader.GetInt32(2);
                        roundResult.Team2Score = reader.GetInt32(3);
                        if (reader.IsDBNull(4) == false)
                            roundResult.Team3Score = reader.GetInt32(4);
                        if (reader.IsDBNull(5) == false)
                            roundResult.WinningTeamName = reader.GetString(5);
                        if (reader.IsDBNull(6) == false)
                            roundResult.DealerName = reader.GetString(6);
                        results.Add(roundResult);
                    }
                }
            }
            _conn.Dispose();

            return results;
        }

        /// <summary>
        /// A team number that is consecutive per game. i.e. Team 1, 2, 3.
        /// </summary>
        public int TeamNumber { get; set; }
        

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
        /// Get incomplete Game and Round details from a specified Competition.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <returns>Returns -1 for both Game and Round if they don't exist.</returns>
        public static (long gameID, long gameRoundID) GetIncompleteGameAndRoundDetails(long competitionID)
        {
            long gameID = -1;
            long gameRoundID = -1;

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT MostRecent_GameID, MostRecent_GameRoundID" +
                    " FROM vwIncompleteGameInfo" +
                    " WHERE CompetitionID = @competitionID";
                command.Parameters.AddWithValue("@competitionID", competitionID);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        gameID = reader.GetInt64(0);
                        gameRoundID = reader.GetInt64(1);
                    }
                }
                _conn.Dispose();

                return (gameID, gameRoundID);
            }
        }

        /// <summary>
        /// Get the GameModel information for a specified Game.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        /// <returns></returns>
        public static GameModel GetGame(long competitionID, long gameID)
        {
            GameModel gameModel = null;

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT GameID, CompetitionID, CompetitionGameNumber," +
                    " Location, GameStartDateTime, GameEndDateTime, LogicallyDeleted" +
                    " FROM Game" +
                    " WHERE CompetitionID = @competitionID" +
                    " AND GameID = @gameID";
                command.Parameters.AddWithValue("@competitionID", competitionID);
                command.Parameters.AddWithValue("@gameID", gameID);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        gameModel = new GameModel();
                        gameModel.GameID = gameID;
                        gameModel.CompetitionID = competitionID;
                        gameModel.CompetitionGameNumber = reader.GetInt32(2);

                        if (reader.IsDBNull(3) == false)
                            gameModel.Location = reader.GetString(3);

                        gameModel.GameStartDateTime = reader.GetDateTime(4);

                        if (reader.IsDBNull(5) == false)
                            gameModel.GameEndDateTime = reader.GetDateTime(5);

                        gameModel.LogicallyDeleted = reader.GetBoolean(6);
                    }
                }
                _conn.Dispose();

                return gameModel;
            }
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
        /// Get Team information for a specific Game.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        /// <returns></returns>
        public static List<TeamModel> GetTeams(long competitionID, long gameID)
        {
            List<TeamModel> teams = new List<TeamModel>();

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT GameTeamID," +
                    " Team1ID, Team1Player1ID, Team1Player2ID, Team1Name," +
                    " Team2ID, Team2Player1ID, Team2Player2ID, Team2Name," +
                    " Team3ID, Team3Player1ID, Team3Player2ID, Team3Name" +
                    " FROM vwGameTeam" +
                    " WHERE CompetitionID = @competitionID" +
                    " AND GameID = @gameID";
                command.Parameters.AddWithValue("@competitionID", competitionID);
                command.Parameters.AddWithValue("@gameID", gameID);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        long gameTeamID = reader.GetInt64(0);
                        TeamModel tm = new TeamModel
                        {
                            GameTeamID = gameTeamID,
                            TeamID = reader.GetInt64(1),
                            TeamPlayer1ID = reader.GetInt64(2),
                            TeamPlayer2ID = reader.GetInt64(3),
                            TeamName = reader.GetString(4)
                        };
                        teams.Add(tm);

                        tm = new TeamModel
                        {
                            GameTeamID = gameTeamID,
                            TeamID = reader.GetInt64(5),
                            TeamPlayer1ID = reader.GetInt64(6),
                            TeamPlayer2ID = reader.GetInt64(7),
                            TeamName = reader.GetString(8)
                        };
                        teams.Add(tm);

                        if (reader.IsDBNull(9) == false)
                        {
                            tm = new TeamModel
                            {
                                GameTeamID = gameTeamID,
                                TeamID = reader.GetInt64(9),
                                TeamPlayer1ID = reader.GetInt64(10),
                                TeamPlayer2ID = reader.GetInt64(11),
                                TeamName = reader.GetString(12)
                            };
                            teams.Add(tm);
                        }
                    }
                }
                _conn.Dispose();

                return teams;
            }
        }

        /// <summary>
        /// Get Players for a specific Game.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="gameID"></param>
        /// <returns></returns>
        public static List<GamePlayerModel> GetPlayers(long competitionID, long gameID)
        {
            List<GamePlayerModel> rValue = new List<GamePlayerModel>();

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT GameTeamID," +
                    " Team1ID, Team1Player1ID, Team1Player1Name, Team1Player2ID, Team1Player2Name, " +
                    " Team2ID, Team2Player1ID, Team2Player1Name, Team2Player2ID, Team2Player2Name, " +
                    " Team3ID, Team3Player1ID, Team3Player1Name, Team3Player2ID, Team3Player2Name " +
                    " FROM vwGameTeam" +
                    " WHERE CompetitionID = @competitionID" +
                    " AND GameID = @gameID";
                command.Parameters.AddWithValue("@competitionID", competitionID);
                command.Parameters.AddWithValue("@gameID", gameID);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Team 1
                        long teamID = reader.GetInt64(1);
                        // Player 1
                        GamePlayerModel gpm = new GamePlayerModel
                        {
                            CompetitionID = competitionID,
                            GameID = gameID,
                            TeamID = teamID,
                            PlayerID = reader.GetInt64(2),
                            PlayerName = reader.GetString(3),
                            TeamNumber = 1
                        };
                        rValue.Add(gpm);
                        // Player 2
                        gpm = new GamePlayerModel
                        {
                            CompetitionID = competitionID,
                            GameID = gameID,
                            TeamID = teamID,
                            PlayerID = reader.GetInt64(4),
                            PlayerName = reader.GetString(5),
                            TeamNumber = 1
                        };
                        rValue.Add(gpm);

                        // Team 2
                        teamID = reader.GetInt64(6);
                        // Player 1
                        gpm = new GamePlayerModel
                        {
                            CompetitionID = competitionID,
                            GameID = gameID,
                            TeamID = teamID,
                            PlayerID = reader.GetInt64(7),
                            PlayerName = reader.GetString(8),
                            TeamNumber = 2
                        };
                        rValue.Add(gpm);
                        // Player 2
                        gpm = new GamePlayerModel
                        {
                            CompetitionID = competitionID,
                            GameID = gameID,
                            TeamID = teamID,
                            PlayerID = reader.GetInt64(9),
                            PlayerName = reader.GetString(10),
                            TeamNumber = 2
                        };
                        rValue.Add(gpm);

                        if (reader.IsDBNull(11) == false)
                        {
                            // Team 3
                            teamID = reader.GetInt64(11);
                            // Player 1
                            gpm = new GamePlayerModel
                            {
                                CompetitionID = competitionID,
                                GameID = gameID,
                                TeamID = teamID,
                                PlayerID = reader.GetInt64(12),
                                PlayerName = reader.GetString(13),
                                TeamNumber = 3
                            };
                            rValue.Add(gpm);
                            // Player 2
                            gpm = new GamePlayerModel
                            {
                                CompetitionID = competitionID,
                                GameID = gameID,
                                TeamID = teamID,
                                PlayerID = reader.GetInt64(16),
                                PlayerName = reader.GetString(17),
                                TeamNumber = 3
                            };
                            rValue.Add(gpm);
                        }
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
        /// <param name="gameID"></param>
        /// <param name="team1ID"></param>
        /// <param name="team2ID"></param>
        /// <param name="team3ID"></param>
        /// <returns></returns>
        public static long InsertGameTeam(long competitionID, long gameID, long team1ID, long team2ID, long? team3ID)
        {
            long rValue = -1;

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            string sql = "INSERT INTO GameTeams (CompetitionID, GameID, Team1ID, Team2ID, Team3ID)" +
                        " VALUES (@competitionID, @gameID, @team1ID, @team2ID, @team3ID)";
            if (team3ID.HasValue == false)
            {
                sql = sql.Replace(", Team3ID", "").Replace(", @team3ID", "");
            }

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddWithValue("@competitionID", competitionID);
                command.Parameters.AddWithValue("@gameID", gameID);
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
        public static (GamePlayerModel currentDealer, GamePlayerModel nextDealer) GetDealer(long gameID)
        {
            GamePlayerModel currentDealer = null;
            GamePlayerModel nextDealer = null;

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT GameID, CurrentDealerPlayerID, CurrentDealer_Name, CurrentDealer_TeamNumber," +
                    " NextDealerPlayerID, NextDealer_Name, NextDealer_TeamNumber" +
                    " FROM vwCurrentAndNextDealer" +
                    " WHERE GameID = @gameID";
                command.Parameters.AddWithValue("@gameID", gameID);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        currentDealer = new GamePlayerModel
                        {
                            GameID = reader.GetInt32(0),
                            PlayerID = reader.GetInt32(1),
                            PlayerName = reader.GetString(2),
                            TeamNumber = reader.GetInt32(3)
                        };

                        nextDealer = new GamePlayerModel
                        {
                            GameID = reader.GetInt32(0),
                            PlayerID = reader.GetInt32(4),
                            PlayerName = reader.GetString(5),
                            TeamNumber = reader.GetInt32(6)
                        };
                    }
                }
            }

            _conn.Dispose();
            return (currentDealer, nextDealer);
        }

                /// <summary>
        /// Record the End of the Round.
        /// </summary>
        /// <param name="gameRoundID"></param>
        public static void UpdateRoundInformation(long gameRoundID, DateTime endOfRoundDateTime, long winningTeamID)
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
    }
}
