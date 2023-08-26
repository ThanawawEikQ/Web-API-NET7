using API_JIGCONTROL.DataAccessLayer.SqlHelper;
using MySqlConnector;
using System.Data;
using Web_API_NET7.Class;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web_API_NET7.SqlHelper
{
    public class SqlAuth
    {
        SqlData db = new SqlData();
        MySqlConnection conn;
        public SqlAuth()
        {
            conn = SqlContect.GetConnection();
            conn.Open();
        }
        public async Task<bool> RegisterAcc(User user)
        {
            try
            {
                string sqlQuery = "INSERT INTO user (Username, PasswordHash, PasswordSalt,Role) VALUES (@user, @passHash, @passSalt,'user')";
                using (MySqlCommand command = new MySqlCommand(sqlQuery, conn))
                {
                    command.Parameters.AddWithValue("@user", user.Username);
                    command.Parameters.AddWithValue("@passHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@passSalt", user.PasswordSalt);
                    command.ExecuteNonQuery();

                    Console.WriteLine("Data saved to the database.");
                }
                conn.Close();
                return true;
            }
            catch { return false; }
          

        }
       

        public async Task<User> Getuser(UserDto user)
        {
            try
            {
                User us = new User();
                var commandText = "SELECT * FROM user WHERE Username = '" + user.Username + "'  ";
                MySqlCommand cmd = new MySqlCommand(commandText, conn);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        us.Username = (string)reader["Username"];
                        us.Role = (string)reader["Role"];
                        us.PasswordHash = (byte[])reader["PasswordHash"];
                        us.PasswordSalt = (byte[])reader["PasswordSalt"];
                    }
                }
                return us;
            }
            catch 
            {
                return new User { };
            }
        }
       
    }
}
