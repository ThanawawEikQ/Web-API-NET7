using MySqlConnector;

namespace Web_API_NET7.SqlHelper
{
	public class SqlContect
	{
		public static string conStr;
		public static MySqlConnection GetConnection()
		{
			try
			{
				MySqlConnection connection = new MySqlConnection(conStr);
				return connection;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
	}
}
