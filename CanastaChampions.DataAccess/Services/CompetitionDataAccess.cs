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
                    c.TeamsAreFixed = reader.GetBoolean(2);
                    c.TeamsAreRandomised = reader.GetBoolean(3);
                    c.LogicallyDeleted = reader.GetBoolean(4);
                    rValue.Add(c);
                }
            }

            _conn.Dispose();

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
                Console.WriteLine($"Competition {competitionModel.CompetitionName} already exists (CompetitionID={competitionID}); not creating.");
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
                command.Parameters.AddWithValue("@fixedTeams", competition.TeamsAreFixed);
                command.Parameters.AddWithValue("@randomiseTeams", competition.TeamsAreRandomised);
                command.ExecuteNonQuery();
                competition.CompetitionID = _conn.LastInsertRowId;
            }

            _conn.Dispose();
        }
    
        /// <summary>
        /// Insert a Player to the database.
        /// </summary>
        /// <param name="player"></param>
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

    }
}
