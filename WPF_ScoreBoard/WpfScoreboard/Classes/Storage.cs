using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.IO;

namespace WpfScoreboard.Classes
{
    public struct SettingNames
    {
        public const string StartDateSummerCamp = "StartDateSummerCamp";
        public const string CurrentDate = "CurrentDate";
        public const string ComPort = "CommPort";
    }

    public class Storage
    {
        #region Private properties
        private const string dbName = "storage.db";
        private static readonly string dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), dbName);

        private Tables.Table Groups;
        private Tables.Table Points;
        private Tables.Table Settings;
        #endregion

        #region Public properties
        public int[]          GroupPoints   = new int[Config.NrOfGroups];
        public List<string>[] GroupNames    = new List<String>[Config.NrOfGroups];
        public string CommPort = String.Empty;
        public DateTimeOffset CurrentDate = DateTimeOffset.MinValue;
        public DateTimeOffset StartDateSummerCamp = DateTimeOffset.MinValue;
        #endregion


        public enum TableSelect
        {
            Groups = 0,
            Points,
        }

        public Storage()
        {
            Groups = new Tables.Table("Groups");
            Groups.AddColumn(new Tables.Column("GroupNr", typeof(int), true, false));
            Groups.AddColumn(new Tables.Column("Name", typeof(string), true, true));

            Points = new Tables.Table("Points");
            Points.AddColumn(new Tables.Column("GroupNr", typeof(int), true, true));
            Points.AddColumn(new Tables.Column("Points", typeof(int), true, false));

            Settings = new Tables.Table("Settings");
            Settings.AddColumn(new Tables.Column("Name", typeof(string), true, true));
            Settings.AddColumn(new Tables.Column("Value", typeof(string), true, true));

            InitializeDatabase();

            // Init properties
            for (int i = 0; i < GroupNames.Length; i++)
            {
                GroupNames[i] = new List<string>();
            }
        }

        public void InitializeDatabase()
        {
            // Create directory if not existing
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                // Create groups table
                String tableCommand = Groups.CreateTableCommand();
                SqliteCommand createTable = new SqliteCommand(tableCommand, db);
                createTable.ExecuteReader();

                // Create points table
                tableCommand = Points.CreateTableCommand();
                createTable = new SqliteCommand(tableCommand, db);
                createTable.ExecuteReader();

                // Create settings table
                tableCommand = Settings.CreateTableCommand();
                createTable = new SqliteCommand(tableCommand, db);
                createTable.ExecuteReader();

            }
        }

        #region Groups methods
        public bool AddPerson(int groupNr, string personName)
        {
            try
            {
                using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
                {
                    db.Open();
                    using (SqliteCommand cmd = new SqliteCommand())
                    {
                        cmd.Connection = db;
                        cmd.CommandText = Groups.InsertRow(new string[] { groupNr.ToString(), personName });
                        cmd.ExecuteNonQuery();
                    }
                    db.Close();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
            
        }

        public List<Groups> GetGroups(string columnFilter)
        {
            List<Groups> entries = new List<Groups>();
            Groups entrie = new Groups();

            if (string.IsNullOrEmpty(columnFilter))
            {
                columnFilter = "*";
            }

            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                using (SqliteCommand cmd = new SqliteCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandText = Groups.SelectCommand(columnFilter);

                    SqliteDataReader reader = cmd.ExecuteReader();
                    while (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            entrie = new Groups();
                            entrie.GroupNr = reader.GetInt16(1);
                            entrie.Name = reader.GetString(2);
                            entries.Add(entrie);
                        }
                        reader.NextResult();
                    }
                }
                db.Close();
            }
            return entries;
        }

        public void RemovePerson(string personName)
        {
            if (string.IsNullOrEmpty(personName))
            {
                return;
            }
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                using (SqliteCommand cmd = new SqliteCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandText = Groups.DeleteCommand(
                        $"{Groups.Columns[1].Name} IS '{personName}'");
                    cmd.ExecuteNonQuery();
                }
                db.Close();
            }
        }
        #endregion

        #region Points methods
        public void ReplacePoints(int groupNr, int newPoints)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                using (SqliteCommand cmd = new SqliteCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandText = Points.ReplaceCommand(new string[] {
                        groupNr.ToString(), newPoints.ToString()
                    });
                    cmd.ExecuteNonQuery();
                }
                db.Close();
            }
        }

        public void AddPoints(int groupNr, int newPoints)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                using (SqliteCommand cmd = new SqliteCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandText = Points.InsertRow(new string[] {
                        groupNr.ToString(), newPoints.ToString()
                    });
                    cmd.ExecuteNonQuery();
                }
                db.Close();
            }

        }

        public List<Points> GetPoints(string columnFilter)
        {
            List<Points> entries = new List<Points>();
            Points entrie = new Points();

            if (String.IsNullOrEmpty(columnFilter))
            {
                columnFilter = "*";
            }

            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                using (SqliteCommand cmd = new SqliteCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandText = Points.SelectCommand(columnFilter);

                    using (SqliteDataReader query = cmd.ExecuteReader())
                    {
                        while (query.Read())
                        {
                            entrie = new Points();
                            entrie.GroupNr = query.GetInt16(1);
                            entrie.GroupPoints = query.GetInt32(2);
                            entries.Add(entrie);
                        }
                    }
                    cmd.ExecuteReader();
                }
                db.Close();
            }
            return entries;
        }
        #endregion

        #region Comm port methods
        public string GetCommPort()
        {
            string port;
            Setting setting = SettingGet(SettingNames.ComPort);
            if (setting == null)
            {
                // Replace data with new comm port 
                string[] ports = System.IO.Ports.SerialPort.GetPortNames();

                if (ports.Length > 0)
                {
                    port = ports[0];
                    this.SettingsReplace(SettingNames.ComPort, port);
                }
                else
                {
                    port = "";
                }
            }
            else
            {
                port = setting.Value;
            }

            return port;
        }
        #endregion

        #region Settings methods
        public void SettingsReplace(string settingName, string value)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                using (SqliteCommand cmd = new SqliteCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandText = Settings.ReplaceCommand(new string[] {
                        settingName, value
                    });
                    cmd.ExecuteNonQuery();
                }
                db.Close();
            }
        }

        public List<Setting> SettingsGet(string columnFilter)
        {
            List<Setting> entries = new List<Setting>();
            Setting entrie = new Setting();

            if (string.IsNullOrEmpty(columnFilter))
            {
                columnFilter = "*";
            }

            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                using (SqliteCommand cmd = new SqliteCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandText = Settings.SelectCommand(columnFilter);

                    using (SqliteDataReader query = cmd.ExecuteReader())
                    {
                        while (query.Read())
                        {
                            entrie = new Setting();
                            entrie.Name = query.GetString(1);
                            entrie.Value = query.GetString(2);
                            entries.Add(entrie);
                        }
                    }
                    cmd.ExecuteReader();
                }
                db.Close();
            }
            return entries;
        }

        public Setting SettingGet(string settingName)
        {
            Setting entrie = null;
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                using (SqliteCommand cmd = new SqliteCommand())
                {
                    cmd.Connection = db;
                    cmd.CommandText = Settings.SelectCommand("*", $"Name IS \"{settingName}\"");

                    using (SqliteDataReader query = cmd.ExecuteReader())
                    {
                        if (query.Read())
                        {
                            entrie = new Setting();
                            entrie.Name = query.GetString(1);
                            entrie.Value = query.GetString(2);
                        }
                    }
                    cmd.ExecuteReader();
                }
                db.Close();
            }
            return entrie;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Retrieve points / groups and settings from database
        /// save data to public properties in this class
        /// </summary>
        public void RetrieveData()
        {
            // Points 
            List<Points> pointsEntries = this.GetPoints("*");
            if (pointsEntries != null)
            {
                foreach (Points points in pointsEntries)
                {
                    GroupPoints[points.GroupNr - 1] = points.GroupPoints;
                }
            }

            // Group names
            List<Groups> grpEntries = this.GetGroups("*");
            if (grpEntries != null)
            {
                foreach (Groups groups in grpEntries)
                {
                    GroupNames[groups.GroupNr - 1].Add(groups.Name);
                }
            }

            // Settings
            List<Setting> settingEntries = this.SettingsGet("*");
            if (settingEntries != null)
            {
                foreach (Setting setting in settingEntries)
                {
                    this.UpdateSetting(setting);
                }
            }

            // Check settings and set default if needed
        }
        #endregion

        #region Private methods
        private void UpdateSetting(Setting setting)
        {
            switch (setting.Name)
            {
                case SettingNames.ComPort:
                    CommPort = setting.Value;
                    break;
                case SettingNames.CurrentDate:
                    CurrentDate = DateTimeOffset.Parse(setting.Value);
                    break;
                case SettingNames.StartDateSummerCamp:
                    StartDateSummerCamp = DateTimeOffset.Parse(setting.Value);
                    break;
                default:
                    break;
            }
        }
        #endregion
    }

    /// <summary>
    /// Data structure to hold group table data from queries
    /// </summary>
    public class Groups
    {
        private int mGroupNr;
        private string mName;

        public int GroupNr
        {
            get { return mGroupNr; }
            set { mGroupNr = value; }
        }

        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }
    }

    /// <summary>
    /// Data structure to hold group table data from queries
    /// </summary>
    public class Points
    {
        private int mGroupNr;
        private int mPoints;

        public int GroupNr
        {
            get { return mGroupNr; }
            set { mGroupNr = value; }
        }

        public int GroupPoints
        {
            get { return mPoints; }
            set { mPoints = value; }
        }
    }

    /// <summary>
    /// Data structure to hold group table data from queries
    /// </summary>
    public class Setting
    {
        private string mName;
        private string mValue;

        public string Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }
    }

}

namespace WpfScoreboard.Classes.Tables
{
    public class Table
    {
        private List<Column> mColumns;
        public List<Column> Columns
        {
            get { return mColumns; }
        }
        private string mTableName;
        public string TableName
        {
            get { return mTableName;}
            set { mTableName = value; }
        }
        private readonly string CreateTableText =
            "CREATE TABLE IF NOT EXISTS {0} ";
        private readonly string CreatePrimaryKey =
            "Primary_Key INTEGER PRIMARY KEY";

        #region Constructors
        public Table()
        {
            mTableName = "";
            mColumns = new List<Column>();
        }

        public Table(string tableName)
        {
            mTableName = tableName;
            mColumns = new List<Column>();
        }
        #endregion

        public void AddColumn(Column newColumn)
        {
            mColumns.Add(newColumn);
        }

        public string CreateTableCommand()
        {
            string retVal = String.Format(CreateTableText, mTableName);
            retVal += '(' + CreatePrimaryKey;
            foreach (Column column in mColumns)
            {
                retVal += ", " + column.ColumnToString();
            }
            retVal += ");";
            return retVal;
        }

        public string InsertRow(string[] data)
        {
            int dataPointer = 0;
            Type columnType = null;
            string columnNames = string.Empty;
            string values = string.Empty; 

            if (data == null) return "";

            foreach (Column column in mColumns)
            {
                //Break on data entries
                if (dataPointer >= data.Length) break;

                // Get column names from table
                if(!string.IsNullOrEmpty(columnNames))
                {
                    columnNames += ", ";
                }
                columnNames += column.Name;
                columnType = column.Type;
                // Convert string array to values 
                if (!string.IsNullOrEmpty(values))
                {
                    values += ", ";
                }
                if(columnType == typeof(string))
                {
                    values += $"'{data[dataPointer]}'";
                }
                else
                {
                    values += data[dataPointer];
                }
                
                dataPointer++;
            }
            return $"INSERT INTO {mTableName} ({columnNames}) VALUES ({values});";
        }

        #region SELECT COMMANDS
        public string SelectCommand()
        {
            return $"SELECT * FROM {mTableName};";
        }
        public string SelectCommand(string filter)
        {
            return $"SELECT {filter} FROM {mTableName};";
        }
        public string SelectCommand(string filter, string where)
        {
            return $"SELECT {filter} FROM {mTableName} WHERE {where};";
        }

        public string SelectCommand(string filter, string where, string orderby, string limit)
        {
            string retVal = $"SELECT {filter} FROM {mTableName}";
            retVal += (string.IsNullOrEmpty(where) ? $" WHERE {where}" : "");
            retVal += (string.IsNullOrEmpty(orderby) ? $" ORDER BY {where}" : "");
            retVal += (string.IsNullOrEmpty(limit) ? $" LIMIT {where}" : "");
            retVal += ";";
            return retVal;
        }
        #endregion

        #region DELETE COMMANDS
        /// <summary>
        /// DELETES ALL ROWS FROM TABLE
        /// </summary>
        /// <returns></returns>
        public string DeleteCommand()
        {
            return $"DELETE FROM {mTableName};";
        }
        public string DeleteCommand(string where)
        {
            return $"DELETE FROM {mTableName} WHERE {where};";
        }
        #endregion

        #region REPLACE COMMANDS
        public string ReplaceCommand(string[] data)
        {
            int dataPointer = 0;
            Type columnType = null;
            string columnNames = string.Empty;
            string values = string.Empty;

            if (data == null) return "";

            foreach (Column column in mColumns)
            {
                //Break on data entries
                if (dataPointer >= data.Length) break;

                // Get column names from table
                if (!string.IsNullOrEmpty(columnNames))
                {
                    columnNames += ", ";
                }
                columnNames += column.Name;
                columnType = column.Type;

                // Convert string array to values 
                if (!string.IsNullOrEmpty(values))
                {
                    values += ", ";
                }
                if (columnType == typeof(string))
                {
                    values += $"'{data[dataPointer]}'";
                }
                else
                {
                    values += data[dataPointer];
                }

                dataPointer++;
            }

            return $"REPLACE INTO {mTableName} ({columnNames}) VALUES ({values});";
        }
        #endregion
    }

    public class Column
    {
        private Type mType;
        private String mName;
        private bool mNotNull;
        private bool mUnique;

        public string Name
        {
            get { return mName; }
        }
        public Type Type
        {
            get { return mType; }
        }

        #region Constructors
        public Column(string name, Type type)
        {
            mType = type;
            mName = name;
            mNotNull = false;
            mUnique = false;
        }

        public Column(string name, Type type, bool notNull)
        {
            mType = type;
            mName = name;
            mNotNull = notNull;
            mUnique = false;
        }

        public Column(string name, Type type, bool notNull, bool unique)
        {
            mType = type;
            mName = name;
            mNotNull = notNull;
            mUnique = unique;
        }
        #endregion

        public string ColumnToString()
        {
            string retVal = string.Empty;
            // Determine column type in sql terms
            if (mType == typeof(int) || mType == typeof(uint))
            {
                retVal = mName + " INT";
            }
            else if (mType == typeof(string))
            {
                retVal = mName + " TEXT";
            }

            // Add extra column info
            if (mNotNull)
            {
                retVal += " NOT NULL";
            }
            if (mUnique)
            {
                retVal += " UNIQUE";
            }
            return retVal;
        }
    }
}

