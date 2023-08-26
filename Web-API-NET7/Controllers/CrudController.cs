using API_JIGCONTROL.DataAccessLayer.SqlHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Collections;
using System.Data;
using System.Security.Cryptography;
using Web_API_NET7.Class;
using Web_API_NET7.SqlHelper;

namespace Web_API_NET7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrudController : ControllerBase
    {
        MySqlConnection conn = SqlContect.GetConnection();
        SqlData db = new SqlData();
        SqlContect con = new SqlContect();
        [HttpPost("EditRole"), Authorize(Roles = "admin")]
        public async Task<IActionResult> EditRole([FromBody] UserDto user)
        {
            try
            {
                string Cmd = $"UPDATE user SET ";
                //if (user.Username.Length > 0) { Cmd += $"Username ='{user.Username}',"; }
                if (user.Password.Length > 0)
                {
                    Cmd += "PasswordHash =@PasswordHash,";
                    Cmd += "PasswordSalt =@PasswordSalt,";
                }
                if (user.Role.Length > 0) { Cmd += $"Role ='{user.Role}',"; }
                Cmd = Cmd.Substring(0, Cmd.Length - 1);
                Cmd += $" WHERE Username ='{user.Username}'";
                using (MySqlCommand command = new MySqlCommand(Cmd, conn))
                {
                    conn.Open();
                    if (user.Password.Length > 0)
                    {
                        User us = await CheckUpdate(user);
                        command.Parameters.AddWithValue("@PasswordHash", us.PasswordHash);
                        command.Parameters.AddWithValue("@PasswordSalt", us.PasswordSalt);
                    }
                    command.ExecuteNonQuery();

                    Console.WriteLine("Data saved to the database.");
                }
                conn.Close();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<User> CheckUpdate(UserDto user)
        {
            User urs = new User();
            try
            {
                CreatePasswordHash(user.Password, out byte[] PasswordHash, out byte[] PasswordSalt);
                urs.PasswordHash = PasswordHash;
                urs.PasswordSalt = PasswordSalt;
                return urs;
            }
            catch { return new User { }; }
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        //--------------------------------------------------------- end edit role --------------------------------------------------------------

        public class ReceiveCar
        {
            public bool status { get; set; }
            public IEnumerable values { get; set; }
            public string Note { get; set; }
        }

        [HttpPost("CarReceive"), Authorize(Roles = "user,admin")]
        public async Task<ReceiveCar> CarReceive([FromBody] Carcare carcare)
        {
            Carcare cc = new Carcare();
            try
            {
                string DateNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                if (carcare == null)
                {
                    return new ReceiveCar { status = false, Note = "carcare null" };
                }

                string Cmd = $"INSERT INTO carcare(customer,car_type,date_receive,status) " +
                      $"VALUES('{carcare.customer}','{carcare.car_type}','{DateNow}','Working')";
                bool result = await db.ExecuteNonQueryAsyncData(Cmd);
                if (!result)
                {
                    return new ReceiveCar { status = false, Note = "Insert fail" };
                }
                return new ReceiveCar { status = true, Note = "carcare OK" };

            }
            catch
            {
                return new ReceiveCar { status = false, Note = "carcare catch" };
            }
        }


        [HttpPost("CarUpdate"), Authorize(Roles = "user,admin")]
        public async Task<ReceiveCar> CarUpdate([FromBody] Carcare carcare)
        {
            Carcare cc = new Carcare();
            try
            {
                string DateNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                if (carcare == null)
                {
                    return new ReceiveCar { status = false, Note = "carcare null" };
                }

                string Cmd = $"UPDATE  carcare SET " +
                    $"date_sent ='{DateNow}',status='close' " +
                    $"WHERE id = '{carcare.Id}' AND status ='Working'";

                bool result = await db.ExecuteNonQueryAsyncData(Cmd);
                if (!result)
                {
                    return new ReceiveCar { status = false, Note = "Update fail" };
                }
                return new ReceiveCar { status = true, Note = "carcare OK" };

            }
            catch
            {
                return new ReceiveCar { status = false, Note = "carcare catch" };
            }
        }

        
        [HttpGet("CarStatus")]
        public async Task<CarStatus> CarCareStatusToday()
        {
            try
            {
                string DateNow = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
                CarStatus cs = new CarStatus();
                string cmd = "SELECT car_status, COUNT(*) AS statuscar " +
                    "FROM carcare " +
                    $"WHERE  date_receive > '{DateNow}'  " +
                    "AND car_status IN ('close','Working') " +
                    "GROUP BY car_status;";
                DataTable dt = await db.QueryDataTable(cmd);
                if(dt.Rows.Count < 0)
                {
                    return new CarStatus { TodayClose = 0, WorkingStatus = 0 };
                }

                foreach (DataRow dr in dt.Rows)
                {
                    string Status = dr["car_status"].ToString();
                    if (Status == "close") { cs.TodayClose = int.Parse(dr["statuscar"].ToString()); };
                    if (Status == "Working") { cs.WorkingStatus = int.Parse(dr["statuscar"].ToString()); };
                }
                return cs;  

            }
            catch { return new CarStatus { TodayClose =0, WorkingStatus =0 }; }
        }
    }
}
