using RostCont;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MainSpace
{
    public sealed class TelegramBot
    {
        private string? Token => Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");

        private TelegramBotClient? Client { get; set; }

        private readonly SwearPromptGenerator _promptGenerator;

        public TelegramBot(DIContainer container)
        {
            _promptGenerator = container.Resolve<SwearPromptGenerator>();
        }

        public async Task Start()
        {
            var cts = new CancellationTokenSource();
            Client = new TelegramBotClient(Token ?? string.Empty, cancellationToken: cts.Token);

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
            }
        }

        private bool CheckMessageCondition(Message msg, UpdateType type)
        {
            return (msg.Text.ToLower().Contains(Configuration.KEYWORD) || msg.ReplyToMessage?.Text != null) && type == UpdateType.Message;
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
