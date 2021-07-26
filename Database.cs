using System.Data;
using System.Data.SqlClient;

namespace Ecommerce_API.Database
{
    public class Database : IDatabase
    {
        private static string connectionString = null;
        public IDbConnection Open()
        {
            if (connectionString == null)
            {
                connectionString = Config.GetInstance().DBConnectionString;
            }

            var con = new SqlConnection(connectionString);
            con.Open();
            return con;
        }
    }
}