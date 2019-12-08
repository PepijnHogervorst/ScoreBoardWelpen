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

        private Tables.Table Groups;
        private Tables.Table Points;
        private static string[] tables = new string[] { "Groups", "Points" };

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

            InitializeDatabase();
        }

        public async void InitializeDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync(dbName, CreationCollisionOption.OpenIfExists);

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
            }
        }

        #region Groups methods
        public void AddPerson(int groupNr, string personName)
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
                    cmd.CommandText = Points.SelectCommand(columnFilter);

                    using (SqliteDataReader query = cmd.ExecuteReader())
                    {
                        while (query.Read())
                        {
                            entrie.GroupNr = query.GetInt16(0);
                            entrie.GroupPoints = query.GetInt32(1);
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

}

namespace ScoreBoardWelpen.Classes.Tables
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

