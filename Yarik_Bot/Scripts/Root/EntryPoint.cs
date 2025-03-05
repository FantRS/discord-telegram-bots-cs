using Claudia;
using RostCunt;

namespace MainSpace
{
    public class EntryPoint
    {
        private static string? API_KEY => Environment.GetEnvironmentVariable("YARIK_DISCORD_BOT_APIKEY");
        private static string ConnectionString => "Host=localhost;Port=5432;Username=postgres;Password=freesexpassword;Database=postgres;";

        private static async Task Main()
        {
            var usersDB = new UsersDB(ConnectionString);
            var promptGenerator = new PromptGenerator(new Anthropic { ApiKey = API_KEY ?? string.Empty });

            var diContainer = new DIContainer();
            diContainer.RegisterInstance(usersDB);
            diContainer.RegisterInstance(promptGenerator);

            var yarikDiscord = new YarikDiscord(diContainer);
            await yarikDiscord.Start();

            var yarikTelegram = new YarikTelegram(diContainer);
            await yarikTelegram.Start();

            Logger.Instance.LogInfo("Global logger");
            await Task.Delay(-1);
        }
    }
}
