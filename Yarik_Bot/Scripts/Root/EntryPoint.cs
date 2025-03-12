using Claudia;
using RostCont;

namespace MainSpace
{
    public class EntryPoint
    {
        private static string? API_KEY => Environment.GetEnvironmentVariable("YARIK_BOTCHAT_APIKEY");
        private static string? CONNECTION_STRING => Environment.GetEnvironmentVariable("YARIK_DB_CONN_STRING");

        private static async Task Main()
        {
            var usersDB = new UsersDB(CONNECTION_STRING ?? string.Empty);
            var promptGenerator = new PromptGenerator(new Anthropic { ApiKey = API_KEY ?? string.Empty });

            var diContainer = new DIContainer();
            diContainer.RegisterInstance(usersDB);
            diContainer.RegisterInstance(promptGenerator);

            var yarikDiscord = new YarikDiscord(diContainer);
            await yarikDiscord.Start();

            var yarikTelegram = new YarikTelegram(diContainer);
            await yarikTelegram.Start();

            await Task.Delay(-1);
        }
    }
}
