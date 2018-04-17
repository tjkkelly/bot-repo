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

        public async Task<bool> AddRecordAsync( NumberStore record )
        {
            using (MySqlConnection conn = GetConnection())
            {
                await conn.OpenAsync().ConfigureAwait(false);
                // '{record.Username}', {record.Number}, {record.Correct.ToString().ToUpper()}, '{record.Timestamp}'
                MySqlCommand insertCommand = new MySqlCommand($"INSERT INTO NumberStore VALUES (@username, @number, @correct, @timestamp);", conn);
                insertCommand.Parameters.AddWithValue("@username", record.Username);
                insertCommand.Parameters.AddWithValue("@number",  record.Number);
                insertCommand.Parameters.AddWithValue("@correct", record.Correct);
                insertCommand.Parameters.AddWithValue("@timestamp", record.Timestamp);

                System.Console.WriteLine(insertCommand);


                int rowsAffected = await insertCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                
                await conn.CloseAsync().ConfigureAwait(false);
            }

            return true;
        }
    }
}