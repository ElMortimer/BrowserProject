using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data.SQLite;
using System.IO;
using System.Data.Common;


namespace Browser
{
    internal class SQLiteHandler
    {
        private SQLiteHandler() { }
        private static SQLiteHandler _instance;
        private static readonly object _lock = new object(); //sincronizeaza threadurile ?

        private SQLiteConnection connection;

        public static SQLiteHandler GetInstance()
        {
            if( _instance == null )
            {
                lock (_lock)
                {
                    //primul thread care ajunge aici creeaza instanta
                    if(_instance == null)
                    {
                        _instance = new SQLiteHandler();
                    }
                }
            }
            return _instance; 
        }

        public void CoonectToDb(string path)
        {
            if (connection != null)
                return;
            if (!File.Exists(path)) 
            { 
                SQLiteConnection.CreateFile(path);
            }
            string connectionString = $"Data Source={path};Version=3;";
            connection = new SQLiteConnection(connectionString);
            connection.Open();
        }

        public void DisconnectFromDb()
        {
            if(connection != null)
            {
                connection.Close();
            }
        }

        public async Task<List<string>> GetAllKeywords()
        {
            List<string> keywords = new List<string>();

            string query = "SELECT Keyword FROM Keywords ORDER BY Keyword";

            using (var cmd = new SQLiteCommand(query, connection))
            using (DbDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    keywords.Add(reader.GetString(0));
                }
            }

            return keywords;
        }

            public void DeleteKeyword(string k)
            {
                string query = "DELETE FROM Keywords WHERE Keyword = @k";
                using (var cmd = new SQLiteCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@k", k);
                cmd.ExecuteNonQuery();
            }
            }

        public bool KeywordExists(string keyword)
        {
            string query = "SELECT keyword FROM Keywords WHERE keyword = @k";
            using (var cmd = new SQLiteCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@k", keyword);
                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read();
                }
            }
        }

        public void AddKeyword(string keyword)
        {
            string query = "INSERT INTO Keywords(keyword) VALUES(@k)";
            using (var cmd = new SQLiteCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@k", keyword);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
