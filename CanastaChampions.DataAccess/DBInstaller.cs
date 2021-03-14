using CanastaChampions.DataAccess.Services;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Threading;

namespace CanastaChampions.DataAccess
{
    public class DBInstaller : BaseDataAccess
    {
        /// <summary>
        /// The path where all of the SQL files that should be executed are.
        /// Their names should confer the run-order.
        /// </summary>
        public const string SQLFileLocation = @"D:\Development\CanastaChampions\CanastaChampions.DataAccess\Scripts";

        public int DelayBetweenInserts = 0;

        public DBInstaller(string dbFilePath, bool loadData)
        {
            System.Diagnostics.Debug.WriteLine($"Overwriting the file {dbFilePath}");

            SQLiteConnection.CreateFile(dbFilePath);

            CreateTables();
            CreateViews();

            if (loadData)
                InsertRecords();
        }

        /// <summary>
        /// Load all of the SQL tables in the specified location.
        /// </summary>
        private void CreateTables()
        {
            // Get a list of files at that location.
            var sqlFileList = Directory.EnumerateFiles(SQLFileLocation + "\\CreateTables", "*.sql");

            using (var connection = new SQLiteConnection(CONNECTION_STRING))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();

                    foreach (string filePath in sqlFileList)
                    {
                        System.Diagnostics.Debug.WriteLine($"Loading SQL file: {filePath}");
                        cmd.CommandText = File.ReadAllText(filePath);
                        cmd.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
            //        }
        }

        /// <summary>
        /// Load all of the SQL views in the specified location.
        /// </summary>
        private void CreateViews()
        {
            // Get a list of files at that location.
            var sqlFileList = Directory.EnumerateFiles(SQLFileLocation + "\\CreateViews", "*.sql");

            using (var connection = new SQLiteConnection(CONNECTION_STRING))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();

                    foreach (string filePath in sqlFileList)
                    {
                        System.Diagnostics.Debug.WriteLine($"Loading SQL file: {filePath}");
                        cmd.CommandText = File.ReadAllText(filePath);
                        cmd.ExecuteNonQuery();
                    }

                }
                connection.Close();
            }
        }

        /// <summary>
        /// Insert records
        /// </summary>
        private void InsertRecords()
        {
            // Get a list of files at that location.
            var sqlFileList = Directory.EnumerateFiles(SQLFileLocation + "\\Data", "*.sql");

            using (var connection = new SQLiteConnection(CONNECTION_STRING))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    connection.Open();

                    foreach (string filePath in sqlFileList)
                    {
                        System.Diagnostics.Debug.WriteLine($"Loading records: {filePath}");
                        string fileContents = File.ReadAllText(filePath);
                        string[] inserts = fileContents.Split(new string[] { "INSERT INTO " }, StringSplitOptions.None);
                        System.Diagnostics.Debug.WriteLine("Number of records: " + inserts.Length);
                        foreach (string token in inserts)
                        {
                            if (token.Length > 0)
                            {
                                string sql = "INSERT INTO " + token;
                                cmd.CommandText = sql;
                                //System.Diagnostics.Debug.WriteLine(sql);
                                Thread.Sleep(DelayBetweenInserts);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                connection.Close();
            }
        }

    }
}
