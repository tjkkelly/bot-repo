using MySql.Data.MySqlClient;  
using System;  
using System.Collections.Generic;  
using System.Threading.Tasks;

namespace TheCountBot.Models
{
    public class NumberStoreContext
    {
        public string ConnectionString { get; set; }  
  
        public NumberStoreContext(string connectionString)  
        {  
            this.ConnectionString = connectionString;  
        }  
  
        private MySqlConnection GetConnection()  
        {  
            return new MySqlConnection(ConnectionString);  
        }

        public async Task<string> GetHistoryAsync()
        {
            string result = "";
            using (MySqlConnection conn = GetConnection())
            {
                await conn.OpenAsync().ConfigureAwait(false);

                System.Console.WriteLine("Opened Database Connection");

                var selectCommand = new MySqlCommand( "SELECT * FROM NumberStore ORDER BY timestamp;" , conn );

                System.Data.Common.DbDataReader reader = await selectCommand.ExecuteReaderAsync().ConfigureAwait( false );

                while ( await reader.ReadAsync().ConfigureAwait( false ) )
                {
                    result += $"{reader.GetString(0)}  {reader.GetInt32(1)}  {reader.GetInt32(2)}  {reader.GetString(3)}\n";
                }

                await conn.CloseAsync().ConfigureAwait(false);

                System.Console.WriteLine("Closed Database Connection");
            }

            return result;
        }

        public async Task<bool> AddRecordAsync( NumberStore record )
        {
            using (MySqlConnection conn = GetConnection())
            {
                await conn.OpenAsync().ConfigureAwait(false);

                System.Console.WriteLine("Opened Database Connection");

                MySqlCommand insertCommand = new MySqlCommand($"INSERT INTO NumberStore VALUES (@username, @number, @correct, @timestamp);", conn);
                insertCommand.Parameters.AddWithValue("@username", record.Username);
                insertCommand.Parameters.AddWithValue("@number",  record.Number);
                insertCommand.Parameters.AddWithValue("@correct", record.Correct);
                insertCommand.Parameters.AddWithValue("@timestamp", record.Timestamp);

                int rowsAffected = await insertCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                
                await conn.CloseAsync().ConfigureAwait(false);

                System.Console.WriteLine("Closed Database Connection");
            }

            return true;
        }
    }
}