using System.Data;
using Microsoft.VisualBasic;
using MySqlConnector;
using Web_API_NET7.SqlHelper;

namespace API_JIGCONTROL.DataAccessLayer.SqlHelper
{
	public class SqlData
	{
		public async Task<bool> ExecuteNonQueryAsyncData(string Cmd)
		{
			bool Result = false;
			try
			{
				using (MySqlConnection conn = SqlContect.GetConnection())
				{
					var commandText = "";
					conn.Open();
					commandText = Cmd;
					MySqlCommand cmd = new MySqlCommand(commandText, conn);
					await cmd.ExecuteNonQueryAsync();
				}
				return Result = true;
			}
			catch { return Result; throw; }
		}

		public async Task<DataTable> QueryDataTable(string Cmd)
		{
			try
			{
				DataTable dt = new DataTable();
				using (MySqlConnection conn = SqlContect.GetConnection())
				{
					var commandText = "";
					conn.Open();
					commandText = Cmd;
					MySqlDataAdapter reader = new MySqlDataAdapter(commandText, conn);
				    reader.Fill(dt);
				}
				return dt;
			}
			catch { throw; }
		}
	}
}
