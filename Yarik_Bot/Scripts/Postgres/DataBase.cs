using Npgsql;

namespace MainSpace
{
    public abstract class DataBase
    {
        public NpgsqlConnection Connection { get; protected set; }

        public DataBase(string connectionString)
        {
            Connection = new NpgsqlConnection(connectionString);
        }

        protected abstract void OpenConnection();
    }
}
