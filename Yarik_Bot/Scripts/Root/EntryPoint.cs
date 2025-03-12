using Claudia;
using RostCont;

namespace MainSpace
{
    public class EntryPoint
    {
        private static string? API_KEY => Environment.GetEnvironmentVariable("BOTCHAT_APIKEY");
        private static string? CONNECTION_STRING => Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        private static async Task Main()
        {
            var usersDB = new UsersDB(CONNECTION_STRING ?? string.Empty);
            var promptGenerator = new SwearPromptGenerator(new Anthropic { ApiKey = API_KEY ?? string.Empty });

            var diContainer = new DIContainer();
            diContainer.RegisterInstance(usersDB);
            diContainer.RegisterInstance(promptGenerator);

            var discordBot = new DiscordBot(diContainer);
            await discordBot.Start();

            var telegramBot = new TelegramBot(diContainer);
            await telegramBot.Start();

            await Task.Delay(-1);
        }
    }
}
