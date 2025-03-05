using Npgsql;

namespace MainSpace
{
    public class UsersDB : DataBase
    {
        private readonly NpgsqlCommand _globalCommand;

        public UsersDB(string connectionString) : base(connectionString)
        {
            _globalCommand = new NpgsqlCommand(connectionString);

            OpenConnection();
        }

        protected override void OpenConnection()
        {
            Connection.Open();
            Logger.Instance.LogInfo($"[PostgreSQL] Database connection opened!");
        }

        public async Task<int> InsertToUserTable(DateTime firstSeen, DateTime lastSeen)
        {
            string stringCommand =
                "INSERT INTO users (first_seen, last_seen) " +
                $"VALUES ({firstSeen}, {lastSeen}) RETURNING id";

            _globalCommand.CommandText = stringCommand;
            var result = await _globalCommand.ExecuteScalarAsync();

            if (result != null)
                return (int)result;

            string exceptionString = typeof(UsersDB).ToString();
            Logger.Instance.LogFatal(exceptionString);
            throw new NullReferenceException(exceptionString);
        }

        public async Task<int> InsertUserPlatformsTable(Platform platform, int userId, string platformId)
        {
            string stringCommand =
                "INSERT INTO user_platforms (current_platform, user_id, platform_id) " +
                $"VALUES ({platform}, {userId}, {platformId}) RETURNING id";

            _globalCommand.CommandText = stringCommand;
            var result = await _globalCommand.ExecuteScalarAsync();

            if (result != null)
                return (int)result;

            string exceptionString = typeof(UsersDB).ToString();
            Logger.Instance.LogFatal(exceptionString);
            throw new NullReferenceException(exceptionString);
        }

        public async Task InsertToUsageTable(int userPlatformId, int inputTokens, int outputTokens, DateTime requestedAt)
        {
            string stringCommand = 
                "INSERT INTO usage (user_platform, input_tokens, output_tokens, requested_at) " +
                $"VALUES ({userPlatformId}, {inputTokens}, {outputTokens}, {requestedAt})";

            _globalCommand.CommandText = stringCommand;
            await _globalCommand.ExecuteNonQueryAsync();
        }

        //public async Task<string> GetCurrentUserPlatformId()
        //{

        //}

        public async Task<bool> CheckToUserExists(string userId, Platform platform)
        {
            string stringCommand = 
                $"SELECT EXISTS (SELECT 1 FROM WHERE current_platform = {platform} AND platform_id = {userId})";

            _globalCommand.CommandText = stringCommand;
            int executionResult = await _globalCommand.ExecuteNonQueryAsync();
            bool result = Convert.ToBoolean(executionResult);
            return result;
        }
    }
}
