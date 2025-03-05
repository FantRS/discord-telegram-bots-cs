using RostCunt;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MainSpace
{
    public sealed class YarikTelegram
    {
        private string? Token => Environment.GetEnvironmentVariable("YARIK_BOT_TG_TOKEN");

        private TelegramBotClient? Client { get; set; }

        private readonly PromptGenerator _promptGenerator;
        private readonly UsersDB _usersDB;
        private readonly Platform _platform = Platform.Telegram;

        public YarikTelegram(DIContainer container)
        {
            _promptGenerator = container.Resolve<PromptGenerator>();
            _usersDB = container.Resolve<UsersDB>();
        }

        public async Task Start()
        {
            Logger.Instance.LogInfo("Starting telegram...", LogPlace.Telegram);

            var cts = new CancellationTokenSource();
            Client = new TelegramBotClient(Token, cancellationToken: cts.Token);

            Client.OnError += OnError;
            Client.OnMessage += OnMessage;
            Client.OnUpdate += OnUpdate;

            await Task.CompletedTask;
        }

        private async Task OnError(Exception exception, HandleErrorSource source)
        {
            Console.WriteLine(exception);
            await Task.CompletedTask;
        }

        private async Task OnMessage(Message msg, UpdateType type)
        {
            if (CheckMessageCondition(msg, type))
            {
                string username = $"{msg.From?.FirstName} {msg.From?.LastName}";
                string responce = await _promptGenerator.GenerateAsync(msg.Text, username, msg.ReplyToMessage?.Text);

                await Client.SendMessage(msg.Chat, responce, replyParameters: msg);

                Logger.Instance.LogDebug($"{msg} / id : {msg.From.Id}", LogPlace.Telegram);
            }
        }

        private bool CheckMessageCondition(Message msg, UpdateType type)
        {
            return (msg.Text.ToLower().Contains("олег") || msg.ReplyToMessage?.Text != null) && type == UpdateType.Message;
        }

        private async Task OnUpdate(Update update)
        {
            if (update is { CallbackQuery: { } query })
            {
                await Client.AnswerCallbackQuery(query.Id, $"You picked {query.Data}");
                await Client.SendMessage(query.Message!.Chat, $"User {query.From} clicked on {query.Data}");
            }
        }
    }
}
