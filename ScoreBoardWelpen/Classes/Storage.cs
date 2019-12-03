using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.IO;
using Windows.Storage;

namespace ScoreBoardWelpen.Classes
{
    public class Storage
    {
        private const string dbName = "storage.db";
        private static readonly string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbName);

        private static string[] tables = new string[] { "Groups", "Points" };

        public enum TableSelect
        {
            Groups = 0,
            Points,
        }

        public Storage()
        {
            InitializeDatabase();
        }

        public async void InitializeDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync(dbName, CreationCollisionOption.OpenIfExists);

            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                String tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS " + tables[(int)TableSelect.Groups] + " (Primary_Key INTEGER PRIMARY KEY, " +
                    "GroupNr INT, " +
                    "Name NVARCHAR(2048) NULL);";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();

                tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS " + tables[(int)TableSelect.Points] + " (Primary_Key INTEGER PRIMARY KEY, " +
                    "GroupNr INT, " +
                    "Points INT);";

                createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();
            }
        }

        #region Groups methods
        public static void AddPerson(int groupNr, string personName)
        {
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO " + tables[(int)TableSelect.Groups] + " VALUES (NULL, @Nr, @Entry);";
                insertCommand.Parameters.AddWithValue("@Nr", groupNr);
                insertCommand.Parameters.AddWithValue("@Entry", personName);

                insertCommand.ExecuteReader();

                db.Close();
            }

        }

        public static List<String> GetGroups(string columnFilter)
        {
            List<String> entries = new List<string>();
            string entrie = string.Empty;

            if (string.IsNullOrEmpty(columnFilter))
            {
                columnFilter = "*";
            }

            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT " + columnFilter + " from " + tables[(int)TableSelect.Groups], db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    entrie = query.GetString(0) + "|" + query.GetString(1) + "|" + query.GetString(2);
                    entries.Add(entrie);
                }

                db.Close();
            }

            return entries;
        }
        #endregion
    }
}
