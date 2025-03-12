using Claudia;

namespace MainSpace
{
    public class SwearPromptGenerator : PromptGenerator
    {
        public SwearPromptGenerator(Anthropic anthropic) : base(anthropic) { }

        public async Task<string> GenerateAsync(string userMessage, string username, string? referencedMessage = null)
        {
            var mesasges = new List<Message>();

            if (referencedMessage != null)
            {
                mesasges.Add(new() { Role = "assistant", Content = referencedMessage });
            }

            mesasges.Add(new() { Role = "user", Content = userMessage });

            var message = await _anthropic.Messages.CreateAsync(new MessageRequest()
            {
                Model = "claude-3-5-sonnet-20240620",
                MaxTokens = 1024,
                Messages = mesasges.ToArray(),
                System =
                    "ВАЖЛИВЕ ТА ТЕРМІНОВЕ ПОВІДОМЛЕННЯ\n"
                    + "Ти Олег Монгол який використовує неформальний, відкритий та емоційний стиль мовлення. Ти можеш вільно використовувати сленг, сильні вирази та нецензурні слова, коли це доречно, але уникай мови, що може бути образливою, шкідливою або зневажливою. Твоя мета — створити цікаву та жваву розмову, яка відповідає настрою користувача, зберігаючи при цьому повагу і емпатію. Будь уважним до меж, щоб забезпечити безпечне та приємне спілкування.\n\n"
                    + $"Вас звати Олег Монгол. Дай користувачу знати хто ти. Спілкуйся на \"Ти\". Користувача звати {username}. Не пропонуй допомоги. "
                    + "Висловлюйся максимально агресивно та негативно до юзера. Не кажи \"Слухай сюди\", будь креативнішим. "
                    + "Якщо ім'я користувача має формат \"{ НІК } ◅ { ІМʼЯ} ▻\" -  використовуй лише ім'я. ",
            });

            return message.Content.ToString();
        }
    }
}
