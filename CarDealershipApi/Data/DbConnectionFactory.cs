using Microsoft.Data.Sqlite;

namespace CarDealershipApi.Data
{
    public class DbConnectionFactory (string connectionString)
    {
        public SqliteConnection Create => new SqliteConnection(connectionString);
    }
}
