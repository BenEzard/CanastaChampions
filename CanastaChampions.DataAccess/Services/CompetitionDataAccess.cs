using CanastaChampions.DataAccess.Models;
using CanastaChampions.DataAccess.Services;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace MvxCanastaChampions.Core.Services
{
    public class CompetitionDataAccess : BaseDataAccess
    {
        /// <summary>
        /// Get all Competitions from the database.
        /// </summary>
        /// <param name="excludeDeleted"></param>
        /// <returns></returns>
        public static List<CompetitionModel> GetAllCompetitions(bool excludeDeleted = false)
        {
            List<CompetitionModel> rValue = new List<CompetitionModel>();

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            try
            {

                using (SQLiteCommand command = _conn.CreateCommand())
                {
                    string baseSQL = "SELECT CompetitionID, CompetitionName, CompetitionImage, FixedTeams, RandomiseTeams, LogicallyDeleted" +
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
                        c.CompetitionImage = reader.GetString(2);
                        c.TeamsAreFixed = reader.GetBoolean(3);
                        c.TeamsAreRandomised = reader.GetBoolean(4);
                        c.LogicallyDeleted = reader.GetBoolean(5);
                        rValue.Add(c);
                    }
                }

                _conn.Dispose();
            }
            catch (SQLiteException e)
            {
                _conn.Dispose();
            }

            return rValue;
        }

        /// <summary>
        /// Get a list of the Players registered for a certain Competition.
        /// Note: Does not return deleted players.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <returns></returns>
        public static List<PlayerModel> GetRegisteredPlayers(long competitionID)
        {
            List<PlayerModel> rValue = new List<PlayerModel>();

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT PlayerID, PlayerName" +
                    " FROM vwRegisteredPlayers" +
                    " WHERE CompetitionID = @competitionID";
                    command.Parameters.AddWithValue("@competitionID", competitionID);

                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    PlayerModel pm = new PlayerModel();
                    pm.PlayerID = reader.GetInt64(0);
                    pm.PlayerName = reader.GetString(1);
                    rValue.Add(pm);
                }
            }

            _conn.Dispose();

            return rValue;
        }

        /// <summary>
        /// Return Team and Player information about Competitions with regular teams.
        /// Records are sorted so that by default the players are not sitting next to Team mates. 
        /// (The user may still need to adjust seating positions).
        /// (Note that GamePlayerModel.TeamNumber is a consecutive number and NOT a database ID).
        /// </summary>
        /// <param name="competitionID"></param>
        /// <returns></returns>
        public static List<GamePlayerModel> GetRegularTeams(long competitionID)
        {
            List<GamePlayerModel> rValue = null;

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT CompetitionID," +
                    " Team1ID, Team1_Member1ID, Team1_Member1_Name, Team1_Member2ID, Team1_Member2_Name, " +
                    " Team2ID, Team2_Member1ID, Team2_Member1_Name, Team2_Member2ID, Team2_Member2_Name, " +
                    " Team3ID, Team3_Member1ID, Team3_Member1_Name, Team3_Member2ID, Team3_Member2_Name " +
                    " FROM vwRegularTeams" +
                    " WHERE CompetitionID = @competitionID";
                command.Parameters.AddWithValue("@competitionID", competitionID);

                SQLiteDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    rValue = new List<GamePlayerModel>();
                    while (reader.Read())
                    {
                        // Team 1, Player 1
                        long gameTeamID = reader.GetInt64(1);
                        GamePlayerModel gpm = new GamePlayerModel(competitionID, 1);
                        gpm.TeamID = gameTeamID;
                        gpm.PlayerID = reader.GetInt64(2);
                        gpm.PlayerName = reader.GetString(3);
                        rValue.Add(gpm);


                        // Team 2, Player 1
                        gameTeamID = reader.GetInt64(6);
                        gpm = new GamePlayerModel(competitionID, 2);
                        gpm.TeamID = gameTeamID;
                        gpm.PlayerID = reader.GetInt64(7);
                        gpm.PlayerName = reader.GetString(8);
                        rValue.Add(gpm);

                        // Team 3, Player 1
                        if (reader.IsDBNull(11) == false)
                        {
                            // Team 3
                            gameTeamID = reader.GetInt64(11);
                            gpm = new GamePlayerModel(competitionID, 3);
                            gpm.TeamID = gameTeamID;
                            gpm.PlayerID = reader.GetInt64(12);
                            gpm.PlayerName = reader.GetString(13);
                            rValue.Add(gpm);
                        }

                        // Team 1, Player 2
                        gpm = new GamePlayerModel(competitionID, 1);
                        gpm.TeamID = gameTeamID;
                        gpm.PlayerID = reader.GetInt64(4);
                        gpm.PlayerName = reader.GetString(5);
                        rValue.Add(gpm);

                        // Team 2, Player 2
                        gpm = new GamePlayerModel(competitionID, 2);
                        gpm.TeamID = gameTeamID;
                        gpm.PlayerID = reader.GetInt64(9);
                        gpm.PlayerName = reader.GetString(10);
                        rValue.Add(gpm);

                        // Team 3, Player 2
                        if (reader.IsDBNull(11) == false)
                        {
                            gpm = new GamePlayerModel(competitionID, 3);
                            gpm.TeamID = gameTeamID;
                            gpm.PlayerID = reader.GetInt64(15);
                            gpm.PlayerName = reader.GetString(16);
                            rValue.Add(gpm);
                        }
                    }
                }
            }

            _conn.Dispose();

            return rValue;
        }

        /// <summary>
        /// Create a new Competition, adding it to the in-memory collection.
        /// </summary>
        /// <param name="competitionModel"></param>
        /// <returns></returns>
        public static CompetitionModel CreateCompetition(CompetitionModel competitionModel)
        {
            if ((competitionModel.TeamsAreFixed) && (competitionModel.TeamsAreRandomised)) throw new ArgumentException("FixedTeams and RandomiseTeams are mutually exclusive; both cannot be true.");

            if (DoesCompetitionExist(competitionModel.CompetitionName, out long competitionID) == false)
            {
                InsertCompetition(competitionModel);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Competition {competitionModel.CompetitionName} already exists (CompetitionID={competitionID}); not creating.");
                competitionModel.CompetitionID = competitionID;
            }

            return competitionModel;
        }

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
        /// Get Competition by Competition name.
        /// </summary>
        /// <param name="competitionName"></param>
        /// <returns></returns>
        public static CompetitionModel GetCompetitionByName(string competitionName)
        {
            CompetitionModel rValue = null;

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT CompetitionID, CompetitionName, CompetitionImage, FixedTeams, RandomiseTeams, LogicallyDeleted" +
                    " FROM Competitions" +
                    " WHERE CompetitionName = @competitionName";
                command.Parameters.AddWithValue("@competitionName", competitionName);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    rValue = new CompetitionModel();
                    rValue.CompetitionID = reader.GetInt64(0);
                    rValue.CompetitionName = reader.GetString(1);
                    rValue.CompetitionImage = reader.GetString(2);
                    rValue.TeamsAreFixed = reader.GetBoolean(3);
                    rValue.TeamsAreRandomised = reader.GetBoolean(4);
                    rValue.LogicallyDeleted = reader.GetBoolean(5);
                }
            }

            _conn.Dispose();

            return rValue;
        }

        /// <summary>
        /// Check to see if a Competition (based on Name) exists.
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public static bool DoesPlayerExist(string playerName, out long playerID)
        {
            bool rValue = false;

            playerID = GetPlayerID(playerName);
            if (playerID != -1)
                rValue = true;

            return rValue;
        }

        /// <summary>
        /// Get Player based on the player name.
        /// </summary>
        /// <param name="playerName"></param>
        /// <returns></returns>
        public static PlayerModel GetPlayer(string playerName)
        {
            PlayerModel player = null;

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT" +
                    " PlayerID," +
                    " PlayerName," +
                    " LogicallyDeleted" +
                    " FROM Players" +
                    " WHERE PlayerName = @playerName";
                command.Parameters.AddWithValue("@playerName", playerName);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        player = new PlayerModel();
                        player.PlayerID = reader.GetInt64(0);
                        player.PlayerName = reader.GetString(1);
                        player.LogicallyDeleted = reader.GetBoolean(2);
                    }
                }
            }

            _conn.Dispose();

            return player;
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
        /// Given a Player's name, returns the PlayerID (if it exists in the database), or -1 if it doesn't.
        /// </summary>
        /// <param name="playerName"></param>
        /// <returns></returns>
        public static long GetPlayerID(string playerName)
        {
            long playerID = -1;

            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "SELECT PlayerID FROM Players WHERE PlayerName = @playerName";
                command.Parameters.AddWithValue("@playerName", playerName);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        playerID = reader.GetInt64(0);
                }
            }

            _conn.Dispose();

            return playerID;
        }

        /// <summary>
        /// Insert a new Competition into the database.
        /// Once inserted, CompetitionID is populated.
        /// </summary>
        /// <param name="competition"></param>
        public static CompetitionModel InsertCompetition(CompetitionModel competition)
        {
            _conn = new SQLiteConnection(CONNECTION_STRING);
            _conn.Open();

            using (SQLiteCommand command = _conn.CreateCommand())
            {
                command.CommandText = "INSERT INTO Competitions (CompetitionName, CompetitionImage, FixedTeams, RandomiseTeams)" +
                        " VALUES (@competitionName, @competitionImage, @fixedTeams, @randomiseTeams)";
                command.Parameters.AddWithValue("@competitionName", competition.CompetitionName);
                command.Parameters.AddWithValue("@competitionImage", competition.CompetitionImage);
                command.Parameters.AddWithValue("@fixedTeams", competition.TeamsAreFixed);
                command.Parameters.AddWithValue("@randomiseTeams", competition.TeamsAreRandomised);
                command.ExecuteNonQuery();
                competition.CompetitionID = _conn.LastInsertRowId;
            }

            _conn.Dispose();

            return competition;
        }
    
        /// <summary>
        /// Insert a Player to the database.
        /// </summary>
        /// <param name="player"></param>
        public static PlayerModel InsertPlayer(PlayerModel player)
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

            return player;
        }

        /// <summary>
        /// Register a Player to a Competition.
        /// </summary>
        /// <param name="competitionID"></param>
        /// <param name="playerID"></param>
        /// <param name="regular"></param>
        public static void InsertPlayerToCompetition(long competitionID, long playerID, bool regular)
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

    }
}
