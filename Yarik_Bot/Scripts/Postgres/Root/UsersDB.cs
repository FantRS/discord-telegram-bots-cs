using Npgsql;

namespace MainSpace
{
    public class UsersDB : DataBase
    {
        public UsersDB(string connectionString) : base(connectionString)
        {
            Connection.Open();
            Logger.Instance.LogInfo($"[PostgreSQL] Database connection opened!");
        }

        public async Task<int> InsertToUserTable(DateTime firstSeen, DateTime lastSeen)
        {
            string stringCommand =
                "INSERT INTO users (first_seen, last_seen) " +
                $"VALUES (@firstSeen, @lastSeen) RETURNING id";

            var command = new NpgsqlCommand(stringCommand, Connection);
            command.Parameters.AddWithValue(parameterName: "firstSeen", value: firstSeen);
            command.Parameters.AddWithValue(parameterName: "lastSeen", value: lastSeen);

            var executionResult = await command.ExecuteScalarAsync();
            int result = Convert.ToInt32(executionResult);

            return result;
        }

        public async Task<int> InsertUserPlatformsTable(Platform platform, int userId, string platformId)
        {
            string stringCommand =
                "INSERT INTO user_platforms (current_platform, user_id, platform_id) " +
                "VALUES (@currentPlatform::platform, @userId, @platformId) RETURNING id";

            var command = new NpgsqlCommand(stringCommand, Connection);
            command.Parameters.AddWithValue(parameterName: "currentPlatform", value: platform.ToString().ToLower());
            command.Parameters.AddWithValue(parameterName: "userId", value: userId);
            command.Parameters.AddWithValue(parameterName: "platformId", value: platformId);

            var executionResult = await command.ExecuteScalarAsync();
            int result = Convert.ToInt32(executionResult);

            return result;
        }

        public async Task InsertToUsageTable(int userPlatformId, int inputTokens, int outputTokens, DateTime requestedAt)
        {
            string stringCommand =
                "INSERT INTO usage (user_platform, input_tokens, output_tokens, requested_at) " +
                $"VALUES (@userPlatformId, @inputTokens, @outputTokens, @requestedAt)";

            var command = new NpgsqlCommand(stringCommand, Connection);
            command.Parameters.AddWithValue(parameterName: "userPlatformId", value: userPlatformId);
            command.Parameters.AddWithValue(parameterName: "inputTokens", value: inputTokens);
            command.Parameters.AddWithValue(parameterName: "outputTokens", value: outputTokens);
            command.Parameters.AddWithValue(parameterName: "requestedAt", value: requestedAt);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<int> GetCurrentUserPlatformId(string userId, Platform platform)
        {
            string stringCommand =
                "SELECT id FROM user_platforms WHERE platform_id = @userId AND current_platform = @platform::platform";

            var command = new NpgsqlCommand(stringCommand, Connection);
            command.Parameters.AddWithValue(parameterName: "userId", value: userId);
            command.Parameters.AddWithValue(parameterName: "platform", value: platform.ToString().ToLower());

            var executionResult = await command.ExecuteScalarAsync();
            int result = Convert.ToInt32(executionResult);

            return result;
        }

        public async Task<bool> CheckToUserExists(string userId, Platform platform)
        {
            string stringCommand =
                $"SELECT EXISTS (SELECT 1 FROM user_platforms WHERE current_platform = @platform::platform AND platform_id = @userId)";

            var command = new NpgsqlCommand(stringCommand, Connection);
            command.Parameters.AddWithValue(parameterName: "platform", platform.ToString().ToLower());
            command.Parameters.AddWithValue(parameterName: "userId", userId);

            var executionResult = await command.ExecuteScalarAsync();
            bool result = Convert.ToBoolean(executionResult);

            return result;
        }
    }
}
