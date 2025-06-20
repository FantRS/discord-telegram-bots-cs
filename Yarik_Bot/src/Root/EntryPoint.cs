using Claudia;
using RostCont;

namespace MainSpace
{
    public class EntryPoint
    {
        private static string? API_KEY => Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");

        private static async Task Main()
        {
            var promptGenerator = new SwearPromptGenerator(new Anthropic { ApiKey = API_KEY ?? string.Empty });

            var diContainer = new DIContainer();
            diContainer.RegisterInstance(promptGenerator);

            var discordBot = new DiscordBot(diContainer);
            await discordBot.Start();

            var telegramBot = new TelegramBot(diContainer);
            await telegramBot.Start();

            await Task.Delay(-1);
        }
    }
}
