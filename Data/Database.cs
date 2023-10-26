using System.Data;
using System.Data.SQLite;
using System.Text.Json.Nodes;

public class DatabaseHelper
{
    private string connectionString;

    public DatabaseHelper(string dbFilePath)
    {
        connectionString = $"Data Source={dbFilePath};Version=3;";
    }

    public List<string> RetrieveStructure()
    {
        List<string> structure = new List<string>();

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            DataTable tablesSchema = connection.GetSchema("Tables");

            foreach (DataRow tableRow in tablesSchema.Rows)
            {
                string tableName = (string)tableRow["TABLE_NAME"];
                string tableStructure = RetrieveTableStructure(connection, tableName);
                structure.Add(tableStructure);
            }

            connection.Close();
        }

        return structure;
    }

    private string RetrieveTableStructure(SQLiteConnection connection, string tableName)
    {
        string structure = "";

        using (SQLiteCommand command = new SQLiteCommand($"SELECT sql FROM sqlite_master WHERE type='table' AND name='{tableName}'", connection))
        {
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    structure = reader.GetString(0);
                }
            }
        }
        return structure;
    }
    public DataSet ExecuteQuery(string query)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                {
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    return dataSet;
                }
            }
        }
    }
    
    public DataTable GetQueryResults(string query)
    {
        DataSet dataSet = ExecuteQuery(query);
        return dataSet.Tables[0];
    }
}